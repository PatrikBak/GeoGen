import { Card, Paper, SimpleGrid, Stack, Text } from "@mantine/core";
import type { ScenarioReportT } from "../types";

interface Props {
  report: ScenarioReportT;
}

/**
 * Top-of-report summary. Stats are split into discrete pill cards so each one has visual weight
 * of its own, rather than a flat row of text values. The rule breakdown spans the full width
 * underneath since it's typically a long string.
 */
export function HeaderSummary({ report }: Props) {
  const proved = Object.keys(report.proofs).length;
  const unproved = report.unprovedTheoremKeys.length;
  const traceCount = report.trace.length;
  const breakdown =
    report.ruleBreakdown.length === 0
      ? "(no trace captured)"
      : report.ruleBreakdown.map((b) => `${b.rule}: ${b.count}`).join(" · ");

  return (
    <Card padding="lg" mb="md">
      <SimpleGrid cols={{ base: 2, sm: 4 }} spacing="md">
        <StatTile label="Proved" value={proved} accent="teal" />
        <StatTile label="Unproved" value={unproved} accent={unproved > 0 ? "red" : "gray"} />
        <StatTile label="Trace events" value={traceCount} accent="indigo" />
        <StatTile label="Wall time" value={`${report.elapsedMs} ms`} accent="gray" />
      </SimpleGrid>
      <Stack gap={4} mt="md">
        <Text size="xs" c="dimmed" tt="uppercase" fw={700} lts={0.5}>
          Rule-type breakdown
        </Text>
        <Text size="sm" ff="monospace">
          {breakdown}
        </Text>
      </Stack>
    </Card>
  );
}

interface StatTileProps {
  label: string;
  value: string | number;
  /** Mantine color name driving the bottom-edge accent. */
  accent: "teal" | "red" | "indigo" | "gray";
}

function StatTile({ label, value, accent }: StatTileProps) {
  return (
    <Paper
      p="md"
      withBorder
      radius="md"
      style={{
        // A 2px colored bar at the bottom — subtle enough not to overwhelm, distinctive enough
        // that proved/unproved/trace/time read as different things at a glance.
        borderBottom: `2px solid var(--mantine-color-${accent}-${accent === "gray" ? "4" : "5"})`,
        backgroundColor: "var(--mantine-color-body)",
      }}
    >
      <Text size="xs" c="dimmed" tt="uppercase" fw={700} lts={0.5}>
        {label}
      </Text>
      <Text size="xl" fw={700} mt={4}>
        {value}
      </Text>
    </Paper>
  );
}
