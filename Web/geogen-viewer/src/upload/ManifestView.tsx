import { useMemo, useState } from "react";
import { Badge, Button, Card, Container, Group, Stack, Table, Text, Title } from "@mantine/core";
import type { ManifestEntryT, UploadedReports } from "../types";

type SortKey = "name" | "proved" | "unproved" | "traceEvents" | "elapsedMs";
type SortDir = "asc" | "desc";

interface ManifestViewProps {
  uploaded: UploadedReports;
  onSelect: (name: string) => void;
  onClear?: () => void;
}

/**
 * The index page shown after a successful upload. Lists every scenario the user dropped, with
 * sortable columns. Each row is clickable to navigate into that scenario's report.
 */
export function ManifestView({ uploaded, onSelect, onClear }: ManifestViewProps) {
  const { manifest, scenarios } = uploaded;
  const [sortKey, setSortKey] = useState<SortKey>("name");
  const [sortDir, setSortDir] = useState<SortDir>("asc");

  const generatedDisplay = useMemo(() => formatGeneratedAt(manifest.generatedAt), [manifest.generatedAt]);

  const sortedRows = useMemo(() => {
    // Only show scenarios for which we actually have a parsed report; the manifest may list
    // scenarios that weren't part of the upload, and clicking those would be a dead end.
    const rows = manifest.scenarios.filter((entry) => scenarios.has(entry.name));
    return [...rows].sort((a, b) => compare(a, b, sortKey, sortDir));
  }, [manifest.scenarios, scenarios, sortKey, sortDir]);

  const missing = manifest.scenarios.length - sortedRows.length;

  const handleHeaderClick = (key: SortKey) => () => {
    if (sortKey === key) {
      setSortDir(sortDir === "asc" ? "desc" : "asc");
    } else {
      setSortKey(key);
      setSortDir(key === "name" ? "asc" : "desc");
    }
  };

  return (
    <Container size="lg" py="xl">
      <Card padding={0}>
        {/* Header banner. Solid indigo — the gradient was visually distracting; one flat color
            reads cleaner. */}
        <div
          style={{
            background: "var(--mantine-color-indigo-6)",
            padding: "1.5rem 1.5rem 1.25rem",
            color: "white",
          }}
        >
          <Group justify="space-between" align="flex-start" wrap="nowrap">
            <Stack gap={6}>
              <Title order={1} c="white" fz="h2">
                GeoGen Prover — Reports
              </Title>
              <Text c="white" opacity={0.85} size="sm">
                {sortedRows.length} scenario{sortedRows.length === 1 ? "" : "s"}
                {missing > 0 ? ` (${missing} missing from upload)` : ""}
                {" · "}generated {generatedDisplay}
              </Text>
            </Stack>
            {onClear ? (
              <Button variant="white" color="dark" size="xs" onClick={onClear}>
                ✕ Clear upload
              </Button>
            ) : null}
          </Group>
        </div>

        <Table
          highlightOnHover
          stickyHeader
          verticalSpacing="sm"
          horizontalSpacing="md"
          // Don't use `striped` here — combined with the colored header banner it becomes too busy.
          // Hover + accent badge per row carries enough visual structure.
        >
          <Table.Thead>
            <Table.Tr>
              <SortHeader label="Scenario" active={sortKey === "name"} dir={sortDir} onClick={handleHeaderClick("name")} />
              <SortHeader label="Proved" active={sortKey === "proved"} dir={sortDir} onClick={handleHeaderClick("proved")} align="right" />
              <SortHeader label="Unproved" active={sortKey === "unproved"} dir={sortDir} onClick={handleHeaderClick("unproved")} align="right" />
              <SortHeader label="Trace" active={sortKey === "traceEvents"} dir={sortDir} onClick={handleHeaderClick("traceEvents")} align="right" />
              <SortHeader label="Elapsed (ms)" active={sortKey === "elapsedMs"} dir={sortDir} onClick={handleHeaderClick("elapsedMs")} align="right" />
            </Table.Tr>
          </Table.Thead>
          <Table.Tbody>
            {sortedRows.map((entry) => (
              <Table.Tr
                key={entry.name}
                onClick={() => onSelect(entry.name)}
                style={{ cursor: "pointer" }}
              >
                <Table.Td>
                  <Text fw={500}>{entry.name}</Text>
                </Table.Td>
                <Table.Td ta="right">
                  <Badge variant="light" color="teal" size="sm" radius="sm">
                    {entry.stats.proved}
                  </Badge>
                </Table.Td>
                <Table.Td ta="right">
                  <Badge
                    variant="light"
                    color={entry.stats.unproved > 0 ? "red" : "gray"}
                    size="sm"
                    radius="sm"
                  >
                    {entry.stats.unproved}
                  </Badge>
                </Table.Td>
                <Table.Td ta="right">
                  <Text size="sm" ff="monospace" c="dimmed">
                    {entry.stats.traceEvents}
                  </Text>
                </Table.Td>
                <Table.Td ta="right">
                  <Text size="sm" ff="monospace" c="dimmed">
                    {entry.stats.elapsedMs}
                  </Text>
                </Table.Td>
              </Table.Tr>
            ))}
          </Table.Tbody>
        </Table>
      </Card>
    </Container>
  );
}

function compare(a: ManifestEntryT, b: ManifestEntryT, key: SortKey, dir: SortDir): number {
  const sign = dir === "asc" ? 1 : -1;
  switch (key) {
    case "name":
      return sign * a.name.localeCompare(b.name);
    case "proved":
      return sign * (a.stats.proved - b.stats.proved);
    case "unproved":
      return sign * (a.stats.unproved - b.stats.unproved);
    case "traceEvents":
      return sign * (a.stats.traceEvents - b.stats.traceEvents);
    case "elapsedMs":
      return sign * (a.stats.elapsedMs - b.stats.elapsedMs);
  }
}

interface SortHeaderProps {
  label: string;
  active: boolean;
  dir: SortDir;
  onClick: () => void;
  align?: "left" | "right";
}

function SortHeader({ label, active, dir, onClick, align = "left" }: SortHeaderProps) {
  return (
    <Table.Th onClick={onClick} ta={align} style={{ cursor: "pointer", userSelect: "none" }}>
      {label}
      {active ? (dir === "asc" ? " ▲" : " ▼") : null}
    </Table.Th>
  );
}

function formatGeneratedAt(iso: string): string {
  const date = new Date(iso);
  if (Number.isNaN(date.getTime())) return iso;
  return date.toLocaleString();
}
