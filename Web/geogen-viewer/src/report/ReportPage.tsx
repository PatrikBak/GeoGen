import { Button, Container, Group, Paper, Title } from "@mantine/core";
import type { ScenarioReportT } from "../types";
import { useStatusText } from "../highlight/HighlightContext";
import { HeaderSummary } from "./HeaderSummary";
import { ConfigurationSection } from "./ConfigurationSection";
import { ToProveOverview } from "./ToProveOverview";
import { ProvedTheorems } from "./ProvedTheorems";
import { UnprovedTheorems } from "./UnprovedTheorems";
import { TraceTable } from "./TraceTable";
import { DiagramSvg } from "./Diagram/DiagramSvg";
import { ColorLegend } from "./Diagram/ColorLegend";

interface Props {
  report: ScenarioReportT;
  onBack: () => void;
}

export function ReportPage({ report, onBack }: Props) {
  // Note: status text is intentionally NOT subscribed here. Subscribing in this top-level
  // component would re-render every section on every hover (status changes on every hover).
  // Instead, <DiagramStatus /> isolates the subscription so only the status text re-renders.

  return (
    <Container size="xl" py="md">
      <Paper
        shadow="sm"
        radius="md"
        p="md"
        mb="md"
        // Sticky page chrome: the back button and title remain in view as the user scrolls
        // through long proof trees and trace tables. A semi-translucent fill plus backdrop blur
        // gives the modern frosted-glass effect when content scrolls under it.
        style={{
          position: "sticky",
          top: 0,
          zIndex: 10,
          background: "rgba(255, 255, 255, 0.92)",
          backdropFilter: "saturate(180%) blur(8px)",
          WebkitBackdropFilter: "saturate(180%) blur(8px)",
        }}
      >
        <Group justify="space-between" align="center" wrap="nowrap">
          <Group gap="sm" wrap="nowrap">
            <Button variant="subtle" color="gray" onClick={onBack} px="xs">
              ← Back
            </Button>
            <Title order={2} size="h3" lineClamp={1}>
              {report.name}
            </Title>
          </Group>
        </Group>
      </Paper>

      <HeaderSummary report={report} />

      {/*
        Two-column area: every text section on the left, sticky diagram on the right. The
        diagram aside is `position: sticky` so it stays visible as the user scrolls past the
        configuration, theorems, and trace — letting them hover trace rows and watch the
        relevant objects light up on the diagram in real time.

        Section order inside main-content reflects the user's reading flow:
          1. Configuration (introduced at the top of this block)
          2. Unproved theorems first — the "what's still open" question is most actionable
          3. Proved theorems — the closed work, expandable for proof inspection
          4. To-prove overview — full theorem set as reference
          5. Inference trace — the per-event ledger of what the prover did
      */}
      <div className="main-layout">
        <div className="main-content">
          <ConfigurationSection configuration={report.configuration} />
          <UnprovedTheorems report={report} />
          <ProvedTheorems report={report} />
          <ToProveOverview theorems={report.theorems} />
          <TraceTable report={report} />
        </div>

        <aside className="diagram-aside">
          <div className="diagram-sticky">
            <h2>Diagram</h2>
            <DiagramSvg diagram={report.diagram} />
            <DiagramStatus />
            <ColorLegend report={report} />
          </div>
        </aside>
      </div>
    </Container>
  );
}

/**
 * Isolated subscriber for the imperative HighlightController's status text. Lives in its own
 * component so that hover-driven status changes only re-render this small div — not the parent
 * ReportPage with all its section children.
 */
function DiagramStatus() {
  const status = useStatusText();
  return <div className="diagram-status">{status}</div>;
}
