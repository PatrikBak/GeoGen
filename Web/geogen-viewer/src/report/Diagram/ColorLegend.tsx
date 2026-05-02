import { useMemo } from "react";
import type { ScenarioReportT } from "../../types";
import { colorForType } from "./colors";

interface Props {
  report: ScenarioReportT;
}

/**
 * Render a small legend showing only the theorem types actually present in this scenario.
 * Hides itself when nothing's there.
 */
export function ColorLegend({ report }: Props) {
  const types = useMemo(() => {
    const set = new Set<string>();
    for (const t of report.theorems) set.add(t.type);
    return [...set].sort();
  }, [report.theorems]);

  if (types.length === 0) return null;

  return (
    <div className="color-legend">
      <strong>Color key:</strong>{" "}
      {types.map((t) => (
        <span key={t} className="color-legend-item">
          <span className="color-legend-swatch" style={{ background: colorForType(t) }} />
          {t}
        </span>
      ))}
    </div>
  );
}
