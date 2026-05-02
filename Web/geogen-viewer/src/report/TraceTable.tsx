import { memo, useMemo, useState } from "react";
import type { ScenarioReportT, TheoremT, TraceEventT } from "../types";
import { useRowHighlight } from "../highlight/useHighlight";
import { badgeClassFor } from "./badge";

interface Props {
  report: ScenarioReportT;
}

type SortKey = "sequence" | "rule" | "theorem" | "assumptions";
type SortDir = "asc" | "desc";

/**
 * Inference-trace table. Plain `<table>` + `<tr>` (not Mantine's components) because the trace
 * regularly hits 200+ rows; Mantine wrappers added enough per-row React work to feel sluggish.
 * Default-collapsed — rows aren't built or mounted until first opened, keeping initial load light.
 */
export function TraceTable({ report }: Props) {
  const theoremByKey = useMemo(
    () => new Map(report.theorems.map((t) => [t.key, t])),
    [report.theorems],
  );

  const [sortBy, setSortBy] = useState<SortKey>("sequence");
  const [sortDir, setSortDir] = useState<SortDir>("asc");
  const [open, setOpen] = useState(false);

  const toggleSort = (key: SortKey) => {
    if (key === sortBy) setSortDir(sortDir === "asc" ? "desc" : "asc");
    else {
      setSortBy(key);
      setSortDir("asc");
    }
  };

  const rows = useMemo(() => {
    if (!open) return [];
    const list = report.trace.map((ev) => {
      const theorem = theoremByKey.get(ev.theoremKey);
      const theoremText = theorem?.text ?? ev.theoremKey;
      const assumptionTexts = ev.assumptionTheoremKeys.map(
        (k) => theoremByKey.get(k)?.text ?? k,
      );
      return { ev, theorem, theoremText, assumptionTexts };
    });
    list.sort((a, b) => compareEvents(a.ev, b.ev, sortBy, theoremByKey));
    if (sortDir === "desc") list.reverse();
    return list;
  }, [open, report.trace, theoremByKey, sortBy, sortDir]);

  if (report.trace.length === 0) {
    return (
      <section className="trace-section collapsible-section">
        <h2>Inference trace</h2>
        <p className="muted">No trace recorded.</p>
      </section>
    );
  }

  return (
    <section className="trace-section collapsible-section">
      <details
        open={open}
        onToggle={(e) => setOpen((e.target as HTMLDetailsElement).open)}
      >
        <summary>
          <h2>
            Inference trace <span className="muted">({report.trace.length})</span>{" "}
            <span className="muted" style={{ fontWeight: "normal" }}>
              — every accepted inference, in the order the prover produced it
            </span>
          </h2>
        </summary>
        {open ? (
          <div className="trace-scroll">
            <table className="trace-table">
              <thead>
                <tr>
                  <SortHeader
                    label="#"
                    active={sortBy === "sequence"}
                    dir={sortDir}
                    onClick={() => toggleSort("sequence")}
                    width={56}
                    num
                  />
                  <SortHeader
                    label="Rule"
                    active={sortBy === "rule"}
                    dir={sortDir}
                    onClick={() => toggleSort("rule")}
                    width={220}
                  />
                  <SortHeader
                    label="Theorem"
                    active={sortBy === "theorem"}
                    dir={sortDir}
                    onClick={() => toggleSort("theorem")}
                  />
                  <SortHeader
                    label="Assumptions"
                    active={sortBy === "assumptions"}
                    dir={sortDir}
                    onClick={() => toggleSort("assumptions")}
                  />
                </tr>
              </thead>
              <tbody>
                {rows.map((r) => (
                  <TraceRow
                    key={r.ev.sequence}
                    ev={r.ev}
                    theorem={r.theorem}
                    theoremText={r.theoremText}
                    assumptionTexts={r.assumptionTexts}
                  />
                ))}
              </tbody>
            </table>
          </div>
        ) : null}
      </details>
    </section>
  );
}

interface SortHeaderProps {
  label: string;
  active: boolean;
  dir: SortDir;
  onClick: () => void;
  width?: number;
  num?: boolean;
}

function SortHeader({ label, active, dir, onClick, width, num }: SortHeaderProps) {
  return (
    <th
      onClick={onClick}
      className={num ? "num" : undefined}
      style={{ cursor: "pointer", userSelect: "none", width }}
    >
      {label}
      {active ? (dir === "asc" ? " ▲" : " ▼") : null}
    </th>
  );
}

function compareEvents(
  a: TraceEventT,
  b: TraceEventT,
  sortBy: SortKey,
  theoremByKey: Map<string, TheoremT>,
): number {
  switch (sortBy) {
    case "sequence":
      return a.sequence - b.sequence;
    case "rule":
      return a.rule.localeCompare(b.rule);
    case "theorem": {
      const at = theoremByKey.get(a.theoremKey)?.text ?? "";
      const bt = theoremByKey.get(b.theoremKey)?.text ?? "";
      return at.localeCompare(bt);
    }
    case "assumptions":
      return a.assumptionTheoremKeys.length - b.assumptionTheoremKeys.length;
  }
}

interface RowProps {
  ev: TraceEventT;
  theorem: TheoremT | undefined;
  theoremText: string;
  assumptionTexts: string[];
}

const TraceRow = memo(function TraceRow({
  ev,
  theorem,
  theoremText,
  assumptionTexts,
}: RowProps) {
  const { onMouseEnter, onMouseLeave, onClick, setRef } = useRowHighlight(
    theorem?.highlightIds ?? [],
    theorem?.type ?? null,
    theoremText,
  );

  return (
    <tr
      ref={setRef as (el: HTMLTableRowElement | null) => void}
      data-row=""
      className="trace-row"
      onMouseEnter={onMouseEnter}
      onMouseLeave={onMouseLeave}
      onClick={onClick}
    >
      <td className="num">{ev.sequence}</td>
      <td>
        <span className={badgeClassFor(ev.rule)}>{ev.rule}</span>
        {ev.customRuleName ? (
          <div className="trace-custom-rule" title={ev.customRuleName}>
            {ev.customRuleName}
          </div>
        ) : null}
      </td>
      <td>
        <code className="trace-text">{theoremText}</code>
      </td>
      <td>
        {assumptionTexts.length === 0 ? (
          <span className="muted">(none)</span>
        ) : (
          <div className="trace-assumptions">
            {assumptionTexts.map((t, i) => (
              <code key={i} className="trace-text">
                {t}
              </code>
            ))}
          </div>
        )}
      </td>
    </tr>
  );
});
