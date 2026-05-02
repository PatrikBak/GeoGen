import { memo, useState } from "react";
import type { TheoremT } from "../types";
import { useRowHighlight } from "../highlight/useHighlight";

interface Props {
  theorems: TheoremT[];
}

export function ToProveOverview({ theorems }: Props) {
  // Default-collapsed; rows aren't rendered until first opened to keep initial load light.
  const [open, setOpen] = useState(false);

  return (
    <section className="collapsible-section">
      <details open={open} onToggle={(e) => setOpen((e.target as HTMLDetailsElement).open)}>
        <summary>
          <h2>
            To prove <span className="muted">({theorems.length})</span>{" "}
            <span className="muted" style={{ fontWeight: "normal" }}>
              — the goals fed to the prover
            </span>
          </h2>
        </summary>
        {open ? (
          <ol className="to-prove-list">
            {theorems.map((t) => (
              <ToProveItem key={t.key} theorem={t} />
            ))}
          </ol>
        ) : null}
      </details>
    </section>
  );
}

const ToProveItem = memo(function ToProveItem({ theorem }: { theorem: TheoremT }) {
  const { onMouseEnter, onMouseLeave, onClick, setRef } = useRowHighlight(
    theorem.highlightIds,
    theorem.type,
    theorem.text,
  );
  return (
    <li
      ref={setRef as (el: HTMLLIElement | null) => void}
      data-row=""
      data-theorem-type={theorem.type}
      className="to-prove-item"
      onMouseEnter={onMouseEnter}
      onMouseLeave={onMouseLeave}
      onClick={onClick}
    >
      {theorem.text}
    </li>
  );
});
