#!/usr/bin/env python3
"""Print a human-readable diff between two benchmark JSON reports.

Renders the *current* report as plain text, with ``A → B (+/-N)`` annotations
on every cell that changed since the baseline. Always exits 0 — meant for
informational console / CI-summary output.
"""

from __future__ import annotations

import argparse
import json
import sys
from pathlib import Path


def _signed(delta: int) -> str:
    return f"+{delta}" if delta > 0 else str(delta)


def _cell(current: int | None, baseline: int | None) -> str:
    """Format a numeric cell, annotating with the delta if it changed."""
    if current is None:
        return f"(removed; was {baseline})"
    if baseline is None or baseline == current:
        return str(current)
    return f"{baseline} → {current} ({_signed(current - baseline)})"


def _index_inputs(report: dict) -> dict[str, dict]:
    return {item["name"]: item for item in report.get("inputs", [])}


def _index_used_rules(report: dict) -> dict[str, int]:
    return {item["rule"]: item["count"] for item in report.get("rules", {}).get("used", [])}


def _section(title: str, lines: list[str]) -> None:
    lines.append("")
    lines.append(title)
    lines.append("=" * len(title))


def _format_table(headers: list[str], rows: list[list[str]]) -> list[str]:
    widths = [len(h) for h in headers]
    for row in rows:
        for i, cell in enumerate(row):
            widths[i] = max(widths[i], len(cell))
    out = []
    out.append("  " + "  ".join(h.ljust(widths[i]) for i, h in enumerate(headers)))
    out.append("  " + "  ".join("-" * widths[i] for i in range(len(headers))))
    for row in rows:
        out.append("  " + "  ".join(row[i].ljust(widths[i]) for i in range(len(row))))
    return out


def _render_inputs(baseline: dict, current: dict, lines: list[str]) -> None:
    base = _index_inputs(baseline)
    cur = _index_inputs(current)
    names = sorted(set(base) | set(cur))

    _section("Theorems per input", lines)
    rows: list[list[str]] = []
    for name in names:
        b = base.get(name)
        c = cur.get(name)
        if c is None:
            rows.append([
                f"{name} (removed)",
                _cell(None, b["found"]),
                _cell(None, b["proved"]),
                _cell(None, b["unproved"]),
                _cell(None, b["excluded"]),
            ])
            continue
        prefix = "" if b is not None else " (new)"
        rows.append([
            f"{name}{prefix}",
            _cell(c["found"], b["found"] if b else None),
            _cell(c["proved"], b["proved"] if b else None),
            _cell(c["unproved"], b["unproved"] if b else None),
            _cell(c["excluded"], b["excluded"] if b else None),
        ])

    btot = baseline.get("totals", {})
    ctot = current.get("totals", {})
    rows.append([
        "TOTAL",
        _cell(ctot.get("found"), btot.get("found")),
        _cell(ctot.get("proved"), btot.get("proved")),
        _cell(ctot.get("unproved"), btot.get("unproved")),
        _cell(ctot.get("excluded"), btot.get("excluded")),
    ])

    lines.extend(_format_table(
        ["Input", "Found", "Proved", "Unproved", "Excluded"],
        rows,
    ))


def _render_used_rules(baseline: dict, current: dict, lines: list[str]) -> None:
    base = _index_used_rules(baseline)
    cur = _index_used_rules(current)

    _section(f"Used inference rules ({len(cur)})", lines)
    if not cur and not base:
        lines.append("  (none)")
        return

    sorted_cur = sorted(cur.items(), key=lambda p: (-p[1], p[0]))
    rows: list[list[str]] = []
    for rank, (rule, count) in enumerate(sorted_cur, start=1):
        baseline_count = base.get(rule)
        annotation = ""
        if baseline_count is None:
            annotation = " (new)"
        rows.append([
            str(rank),
            _cell(count, baseline_count),
            f"{rule}{annotation}",
        ])

    # Removed rules: were used in baseline, not in current
    removed = sorted(set(base) - set(cur))
    if removed:
        for rule in removed:
            rows.append(["–", _cell(None, base[rule]), f"{rule} (removed)"])

    lines.extend(_format_table(["#", "Count", "Rule"], rows))


def _render_unused_rules(baseline: dict, current: dict, lines: list[str]) -> None:
    base = sorted(baseline.get("rules", {}).get("unused", []))
    cur = sorted(current.get("rules", {}).get("unused", []))
    base_set = set(base)
    cur_set = set(cur)

    _section(f"Unused inference rules ({len(cur)})", lines)
    if not cur:
        lines.append("  (none)")
    else:
        for rule in cur:
            marker = "+" if rule not in base_set else " "
            lines.append(f"  {marker} {rule}")

    moved_to_used = sorted(base_set - cur_set)
    if moved_to_used:
        lines.append("")
        lines.append(f"  No longer unused (now used) ({len(moved_to_used)}):")
        for rule in moved_to_used:
            lines.append(f"  - {rule}")


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("baseline", type=Path, help="Baseline report.json")
    parser.add_argument("current", type=Path, help="New report.json to compare")
    parser.add_argument(
        "--baseline-label",
        default="baseline",
        help="Label for the baseline (default: 'baseline')",
    )
    parser.add_argument(
        "--current-label",
        default="current",
        help="Label for the current report (default: 'current')",
    )
    args = parser.parse_args()

    if not args.baseline.is_file():
        print(f"error: baseline file not found: {args.baseline}", file=sys.stderr)
        return 1
    if not args.current.is_file():
        print(f"error: current file not found: {args.current}", file=sys.stderr)
        return 1

    baseline = json.loads(args.baseline.read_text(encoding="utf-8"))
    current = json.loads(args.current.read_text(encoding="utf-8"))

    header = f"Prover benchmark report ({args.baseline_label} → {args.current_label})"
    lines: list[str] = [header, "=" * len(header)]

    if baseline == current:
        lines.append("")
        lines.append("Reports are identical.")

    _render_inputs(baseline, current, lines)
    _render_used_rules(baseline, current, lines)
    _render_unused_rules(baseline, current, lines)

    print("\n".join(lines).rstrip() + "\n")
    return 0


if __name__ == "__main__":
    sys.exit(main())
