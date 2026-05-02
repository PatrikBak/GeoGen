#!/usr/bin/env python3
"""Print a human-readable diff between two benchmark JSON reports.

Always exits 0 — meant for informational output (e.g. dumped to a CI job
summary), not for failing builds.
"""

from __future__ import annotations

import argparse
import json
import sys
from pathlib import Path


def _signed(delta: int) -> str:
    return f"+{delta}" if delta > 0 else str(delta)


def _index_inputs(report: dict) -> dict[str, dict]:
    return {item["name"]: item for item in report.get("inputs", [])}


def _index_used_rules(report: dict) -> dict[str, int]:
    return {item["rule"]: item["count"] for item in report.get("rules", {}).get("used", [])}


def _diff_inputs(baseline: dict, current: dict, lines: list[str]) -> None:
    base = _index_inputs(baseline)
    cur = _index_inputs(current)
    all_names = sorted(set(base) | set(cur))

    rows: list[tuple[str, str, str, str, str]] = []
    for name in all_names:
        b = base.get(name)
        c = cur.get(name)
        if b is None:
            rows.append((f"+ {name}", str(c["found"]), str(c["proved"]), str(c["unproved"]), str(c["excluded"])))
            continue
        if c is None:
            rows.append((f"- {name}", str(b["found"]), str(b["proved"]), str(b["unproved"]), str(b["excluded"])))
            continue
        if all(b[k] == c[k] for k in ("found", "proved", "unproved", "excluded")):
            continue
        rows.append((
            f"  {name}",
            f"{b['found']} → {c['found']} ({_signed(c['found'] - b['found'])})",
            f"{b['proved']} → {c['proved']} ({_signed(c['proved'] - b['proved'])})",
            f"{b['unproved']} → {c['unproved']} ({_signed(c['unproved'] - b['unproved'])})",
            f"{b['excluded']} → {c['excluded']} ({_signed(c['excluded'] - b['excluded'])})",
        ))

    lines.append("## Per-input theorem counts")
    if not rows:
        lines.append("  (no changes)")
        return
    for input_label, found, proved, unproved, excluded in rows:
        lines.append(f"  {input_label}")
        lines.append(f"      found    : {found}")
        lines.append(f"      proved   : {proved}")
        lines.append(f"      unproved : {unproved}")
        lines.append(f"      excluded : {excluded}")


def _diff_totals(baseline: dict, current: dict, lines: list[str]) -> None:
    b = baseline.get("totals", {})
    c = current.get("totals", {})
    keys = ("found", "proved", "unproved", "excluded")
    if all(b.get(k) == c.get(k) for k in keys):
        return
    lines.append("## Totals")
    for k in keys:
        bv = b.get(k, 0)
        cv = c.get(k, 0)
        if bv == cv:
            lines.append(f"  {k:<8}: {bv} (unchanged)")
        else:
            lines.append(f"  {k:<8}: {bv} → {cv} ({_signed(cv - bv)})")


def _diff_used_rules(baseline: dict, current: dict, lines: list[str]) -> None:
    b = _index_used_rules(baseline)
    c = _index_used_rules(current)

    added = sorted(set(c) - set(b))
    removed = sorted(set(b) - set(c))
    changed = sorted(
        ((rule, b[rule], c[rule]) for rule in set(b) & set(c) if b[rule] != c[rule]),
        key=lambda t: -abs(t[2] - t[1]),
    )

    if not (added or removed or changed):
        lines.append("## Inference rules")
        lines.append("  (no changes)")
        return

    lines.append("## Inference rules")
    if added:
        lines.append(f"  Added ({len(added)}):")
        for rule in added:
            lines.append(f"    + {rule}  (count={c[rule]})")
    if removed:
        lines.append(f"  Removed ({len(removed)}):")
        for rule in removed:
            lines.append(f"    - {rule}  (was count={b[rule]})")
    if changed:
        lines.append(f"  Count changed ({len(changed)}):")
        for rule, bv, cv in changed:
            lines.append(f"    ~ {rule}: {bv} → {cv} ({_signed(cv - bv)})")


def _diff_unused_rules(baseline: dict, current: dict, lines: list[str]) -> None:
    b = set(baseline.get("rules", {}).get("unused", []))
    c = set(current.get("rules", {}).get("unused", []))
    started_using = sorted(b - c)  # were unused, now used
    stopped_using = sorted(c - b)  # were used, now unused

    if not (started_using or stopped_using):
        return
    lines.append("## Unused-rule transitions")
    if started_using:
        lines.append(f"  Newly used ({len(started_using)}):")
        for rule in started_using:
            lines.append(f"    > {rule}")
    if stopped_using:
        lines.append(f"  No longer used ({len(stopped_using)}):")
        for rule in stopped_using:
            lines.append(f"    < {rule}")


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

    if baseline == current:
        print(f"Reports are identical ({args.baseline_label} == {args.current_label}).")
        return 0

    lines: list[str] = [f"# Benchmark diff: {args.baseline_label} → {args.current_label}", ""]
    _diff_totals(baseline, current, lines)
    if lines[-1]:
        lines.append("")
    _diff_inputs(baseline, current, lines)
    lines.append("")
    _diff_used_rules(baseline, current, lines)
    lines.append("")
    _diff_unused_rules(baseline, current, lines)

    print("\n".join(lines).rstrip() + "\n")
    return 0


if __name__ == "__main__":
    sys.exit(main())
