import { useCallback, useRef, useState, type ChangeEvent, type DragEvent } from "react";
import { Alert, Button, Code, Container, Group, Loader, Paper, Stack, Text, Title } from "@mantine/core";
import { ZodError } from "zod";
import {
  EXPECTED_SCHEMA,
  SchemaVersionMismatchError,
  parseManifest,
  parseScenarioReport,
} from "../schema";
import type {
  ManifestEntryT,
  ManifestT,
  ScenarioReportT,
  UploadedReports,
} from "../types";

// Minimal subset of the (non-standard) FileSystem API we use to walk dropped folders. Typing
// these explicitly so we avoid `any` while keeping the surface small.
interface FileSystemEntryLike {
  isFile: boolean;
  isDirectory: boolean;
  name: string;
  fullPath: string;
}
interface FileSystemFileEntryLike extends FileSystemEntryLike {
  isFile: true;
  file(cb: (file: File) => void, err?: (e: unknown) => void): void;
}
interface FileSystemDirectoryEntryLike extends FileSystemEntryLike {
  isDirectory: true;
  createReader(): FileSystemDirectoryReaderLike;
}
interface FileSystemDirectoryReaderLike {
  readEntries(
    cb: (entries: FileSystemEntryLike[]) => void,
    err?: (e: unknown) => void,
  ): void;
}

interface NamedFile {
  /** Path the file came in with (e.g. "json-reports/manifest.json" or just "foo.json"). */
  path: string;
  file: File;
}

interface UploadZoneProps {
  onUploaded: (uploaded: UploadedReports) => void;
}

export function UploadZone({ onUploaded }: UploadZoneProps) {
  const [dragOver, setDragOver] = useState(false);
  const [busy, setBusy] = useState(false);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const [errorIsSchemaMismatch, setErrorIsSchemaMismatch] = useState(false);
  const [warnings, setWarnings] = useState<string[]>([]);
  const fileInputRef = useRef<HTMLInputElement | null>(null);
  const folderInputRef = useRef<HTMLInputElement | null>(null);

  const reset = useCallback(() => {
    setErrorMessage(null);
    setErrorIsSchemaMismatch(false);
    setWarnings([]);
  }, []);

  const handleFiles = useCallback(
    async (files: NamedFile[]) => {
      reset();
      if (files.length === 0) {
        setErrorMessage("No files were dropped. Drop a folder of JSON reports or pick files via the buttons.");
        return;
      }
      setBusy(true);
      try {
        const result = await buildUploadedReports(files);
        if (result.warnings.length > 0) setWarnings(result.warnings);
        onUploaded(result.uploaded);
      } catch (err) {
        if (err instanceof SchemaVersionMismatchError) {
          setErrorIsSchemaMismatch(true);
          setErrorMessage(err.message);
        } else if (err instanceof ZodError) {
          setErrorMessage(`Schema validation failed: ${err.issues.map((i) => `${i.path.join(".") || "(root)"}: ${i.message}`).join("; ")}`);
        } else if (err instanceof Error) {
          setErrorMessage(err.message);
        } else {
          setErrorMessage(String(err));
        }
      } finally {
        setBusy(false);
      }
    },
    [onUploaded, reset],
  );

  const onDrop = useCallback(
    async (e: DragEvent<HTMLDivElement>) => {
      e.preventDefault();
      setDragOver(false);
      const items = e.dataTransfer.items;
      const collected: NamedFile[] = [];
      // Prefer the FileSystem entry API when available — it walks folders. Fall back to plain
      // `files` (no folder traversal) when it isn't.
      const entries: FileSystemEntryLike[] = [];
      if (items && items.length > 0) {
        for (let i = 0; i < items.length; i++) {
          const item = items[i];
          // webkitGetAsEntry is non-standard but supported in every browser we care about.
          const getEntry = (item as unknown as { webkitGetAsEntry?: () => FileSystemEntryLike | null }).webkitGetAsEntry;
          if (typeof getEntry === "function") {
            const entry = getEntry.call(item);
            if (entry) entries.push(entry);
          }
        }
      }
      if (entries.length > 0) {
        for (const entry of entries) await collectFromEntry(entry, collected);
      } else if (e.dataTransfer.files) {
        for (let i = 0; i < e.dataTransfer.files.length; i++) {
          const f = e.dataTransfer.files[i];
          collected.push({ path: f.name, file: f });
        }
      }
      const jsons = collected.filter((nf) => nf.path.toLowerCase().endsWith(".json"));
      await handleFiles(jsons);
    },
    [handleFiles],
  );

  const onPickFiles = useCallback(
    async (e: ChangeEvent<HTMLInputElement>) => {
      const files = e.target.files;
      if (!files) return;
      const collected: NamedFile[] = [];
      for (let i = 0; i < files.length; i++) {
        const f = files[i];
        // webkitRelativePath is set when the input has webkitdirectory; otherwise just the name.
        const path = (f as File & { webkitRelativePath?: string }).webkitRelativePath || f.name;
        if (!path.toLowerCase().endsWith(".json")) continue;
        collected.push({ path, file: f });
      }
      // Reset the input so the same selection can re-trigger after a "Try again".
      e.target.value = "";
      await handleFiles(collected);
    },
    [handleFiles],
  );

  return (
    <Container size="md" py={48}>
      <Stack gap="lg">
        {/* Page-level identity, separate from the drop area so the title stays visible during a
            schema-mismatch error. */}
        <Stack gap={4} align="center" mb="sm">
          <Title order={1} ta="center" fz={36} fw={700}>
            GeoGen Prover — Report Viewer
          </Title>
          <Text c="dimmed" ta="center" size="sm">
            A stateless web viewer for the JSON reports produced by the integration test.
          </Text>
        </Stack>

        <Paper
          shadow="sm"
          radius="lg"
          p="xl"
          ta="center"
          // Dashed border + drag-over emphasis. Mantine doesn't expose a native dropzone in
          // @mantine/core (it's in @mantine/dropzone, an extra package). We keep the drop
          // handlers manual.
          style={{
            borderStyle: "dashed",
            borderWidth: 2,
            borderColor: dragOver
              ? "var(--mantine-color-indigo-5)"
              : "var(--mantine-color-gray-3)",
            background: dragOver
              ? "linear-gradient(135deg, var(--mantine-color-indigo-0), var(--mantine-color-violet-0))"
              : "var(--mantine-color-body)",
            transition:
              "background 160ms ease, border-color 160ms ease, transform 160ms ease",
            transform: dragOver ? "scale(1.005)" : undefined,
          }}
          onDragEnter={(e) => {
            e.preventDefault();
            setDragOver(true);
          }}
          onDragOver={(e) => {
            e.preventDefault();
            setDragOver(true);
          }}
          onDragLeave={() => setDragOver(false)}
          onDrop={onDrop}
        >
          <Stack gap="md" align="center">
            <Text size="xl" fw={600}>
              Drop GeoGen JSON reports here
            </Text>
            <Text size="sm" c="dimmed" maw={520}>
              Drop the <Code>json-reports</Code> folder produced by the integration test, or any
              combination of <Code>manifest.json</Code> + <Code>scenarios/*.json</Code>. A single
              scenario file works too.
            </Text>
            <Group gap="sm">
              <Button
                size="md"
                variant="gradient"
                gradient={{ from: "indigo", to: "violet", deg: 135 }}
                onClick={() => fileInputRef.current?.click()}
                disabled={busy}
              >
                Pick files…
              </Button>
              <Button
                size="md"
                variant="default"
                onClick={() => folderInputRef.current?.click()}
                disabled={busy}
              >
                Pick folder…
              </Button>
            </Group>
            <input
              ref={fileInputRef}
              type="file"
              accept="application/json,.json"
              multiple
              onChange={onPickFiles}
              style={{ display: "none" }}
            />
            <input
              ref={folderInputRef}
              type="file"
              // webkitdirectory is non-standard JSX; cast props to allow it without disabling TS strictness.
              {...({ webkitdirectory: "", directory: "" } as Record<string, string>)}
              multiple
              onChange={onPickFiles}
              style={{ display: "none" }}
            />
            <Text size="xs" c="dimmed">
              Schema version: <Code>{EXPECTED_SCHEMA}</Code>
            </Text>
            {busy ? (
              <Group gap="xs">
                <Loader size="sm" />
                <Text size="sm" c="dimmed">
                  Parsing…
                </Text>
              </Group>
            ) : null}
          </Stack>
        </Paper>

        {errorMessage ? (
          <Alert
            color="red"
            title={errorIsSchemaMismatch ? "Schema version mismatch" : "Upload failed"}
            withCloseButton
            onClose={reset}
          >
            <Stack gap="xs">
              <Text size="sm">{errorMessage}</Text>
              <Button size="xs" variant="light" color="red" onClick={reset}>
                Try again
              </Button>
            </Stack>
          </Alert>
        ) : null}
        {warnings.length > 0 ? (
          <Alert color="yellow" title="Warnings">
            <Stack gap={4}>
              {warnings.map((w, i) => (
                <Text key={i} size="sm">
                  {w}
                </Text>
              ))}
            </Stack>
          </Alert>
        ) : null}
      </Stack>
    </Container>
  );
}

// ---------- helpers ----------

async function collectFromEntry(entry: FileSystemEntryLike, into: NamedFile[]): Promise<void> {
  if (entry.isFile) {
    const fileEntry = entry as FileSystemFileEntryLike;
    const file = await new Promise<File>((resolve, reject) => {
      fileEntry.file(resolve, reject);
    });
    into.push({ path: entry.fullPath.replace(/^\//, "") || file.name, file });
    return;
  }
  if (entry.isDirectory) {
    const dirEntry = entry as FileSystemDirectoryEntryLike;
    const reader = dirEntry.createReader();
    // readEntries returns at most ~100 entries per call — keep calling until we get an empty batch.
    while (true) {
      const batch = await new Promise<FileSystemEntryLike[]>((resolve, reject) => {
        reader.readEntries(resolve, reject);
      });
      if (batch.length === 0) break;
      for (const child of batch) await collectFromEntry(child, into);
    }
  }
}

interface BuildResult {
  uploaded: UploadedReports;
  warnings: string[];
}

async function buildUploadedReports(files: NamedFile[]): Promise<BuildResult> {
  const warnings: string[] = [];
  const manifestFile = files.find((nf) => /(?:^|\/)manifest\.json$/i.test(nf.path));

  // Path-1: a manifest is present. Use it as the source of truth.
  if (manifestFile) {
    const raw = await readJson(manifestFile);
    const manifest = parseManifest(raw, manifestFile.path);
    const scenarios = new Map<string, ScenarioReportT>();
    for (const entry of manifest.scenarios) {
      const match = files.find(
        (nf) => nf !== manifestFile && nf.path.endsWith(entry.file),
      );
      if (!match) {
        warnings.push(`Manifest references "${entry.file}" but no matching file was uploaded.`);
        continue;
      }
      const scenarioRaw = await readJson(match);
      const scenario = parseScenarioReport(scenarioRaw, match.path);
      scenarios.set(scenario.name, scenario);
    }
    if (scenarios.size === 0) {
      throw new Error(
        "Manifest was uploaded but none of the referenced scenario files could be matched. Drop the whole reports folder.",
      );
    }
    return { uploaded: { manifest, scenarios }, warnings };
  }

  // Path-2: no manifest — try parsing every JSON as a scenario report. If at least one parses,
  // synthesize a manifest. Per-file errors become warnings (skip and continue).
  const scenarios = new Map<string, ScenarioReportT>();
  const synthesizedEntries: ManifestEntryT[] = [];
  for (const nf of files) {
    try {
      const raw = await readJson(nf);
      const report = parseScenarioReport(raw, nf.path);
      scenarios.set(report.name, report);
      synthesizedEntries.push({
        name: report.name,
        file: `scenarios/${report.name}.json`,
        stats: {
          proved: Object.keys(report.proofs).length,
          unproved: report.unprovedTheoremKeys.length,
          traceEvents: report.trace.length,
          elapsedMs: report.elapsedMs,
        },
      });
    } catch (err) {
      // SchemaVersionMismatchError is fatal: that's a configuration problem, not a stray file.
      if (err instanceof SchemaVersionMismatchError) throw err;
      warnings.push(`${nf.path}: ${err instanceof Error ? err.message : String(err)}`);
    }
  }
  if (scenarios.size === 0) {
    throw new Error(
      "No parseable scenario reports found in the upload. Drop a manifest.json + scenarios/ folder produced by the GeoGen integration test.",
    );
  }
  const manifest: ManifestT = {
    schema: EXPECTED_SCHEMA,
    generatedAt: new Date().toISOString(),
    scenarios: synthesizedEntries,
  };
  return { uploaded: { manifest, scenarios }, warnings };
}

async function readJson(nf: NamedFile): Promise<unknown> {
  const text = await nf.file.text();
  try {
    return JSON.parse(text) as unknown;
  } catch (err) {
    throw new Error(
      `Failed to parse ${nf.path} as JSON: ${err instanceof Error ? err.message : String(err)}`,
    );
  }
}
