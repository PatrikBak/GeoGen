#!/usr/bin/env -S uv run --script
# /// script
# requires-python = ">=3.10"
# dependencies = ["rich>=13.0"]
# ///
"""Print a human-readable diff between two benchmark JSON reports.

Renders the *current* report as a table-driven console view, with
``A → B (+/-N)`` annotations on every cell that changed since the baseline.
Always exits 0 — meant for informational console / CI-summary output.
"""

from __future__ import annotations

import argparse
import json
import sys
from pathlib import Path

from rich.console import Console
from rich.rule import Rule
from rich.table import Table


def _signed(delta: int) -> str:
    return f"+{delta}" if delta > 0 else str(delta)


def _delta_cell(current: int | None, baseline: int | None) -> str:
    """Format a numeric cell with rich markup, annotating deltas."""
    if current is None:
        return f"[red](removed; was {baseline})[/red]"
    if baseline is None:
        return f"[green]{current} (new)[/green]"
    if baseline == current:
        return str(current)
    delta = current - baseline
    color = "green" if delta > 0 else "red"
    return f"{baseline} → {current} [{color}]({_signed(delta)})[/{color}]"


def _index_inputs(report: dict) -> dict[str, dict]:
    return {item["name"]: item for item in report.get("inputs", [])}


def _index_used_rules(report: dict) -> dict[str, int]:
    return {item["rule"]: item["count"] for item in report.get("rules", {}).get("used", [])}


def _render_inputs(baseline: dict, current: dict, console: Console) -> None:
    base = _index_inputs(baseline)
    cur = _index_inputs(current)
    names = sorted(set(base) | set(cur))

    table = Table(title="Theorems per input", title_justify="left", show_lines=False)
    table.add_column("Input", overflow="fold")
    for col in ("Found", "Proved", "Unproved", "Excluded"):
        table.add_column(col, justify="right")

    for name in names:
        b = base.get(name)
        c = cur.get(name)
        if c is None:
            table.add_row(
                f"[red]{name} (removed)[/red]",
                _delta_cell(None, b["found"]),
                _delta_cell(None, b["proved"]),
                _delta_cell(None, b["unproved"]),
                _delta_cell(None, b["excluded"]),
            )
            continue
        label = name if b is not None else f"[green]{name} (new)[/green]"
        table.add_row(
            label,
            _delta_cell(c["found"], b["found"] if b else None),
            _delta_cell(c["proved"], b["proved"] if b else None),
            _delta_cell(c["unproved"], b["unproved"] if b else None),
            _delta_cell(c["excluded"], b["excluded"] if b else None),
        )

    btot = baseline.get("totals", {})
    ctot = current.get("totals", {})
    table.add_row(
        "[bold]TOTAL[/bold]",
        _delta_cell(ctot.get("found"), btot.get("found")),
        _delta_cell(ctot.get("proved"), btot.get("proved")),
        _delta_cell(ctot.get("unproved"), btot.get("unproved")),
        _delta_cell(ctot.get("excluded"), btot.get("excluded")),
    )
    console.print(table)


def _render_used_rules(baseline: dict, current: dict, console: Console) -> None:
    base = _index_used_rules(baseline)
    cur = _index_used_rules(current)

    table = Table(
        title=f"Used inference rules ({len(cur)})",
        title_justify="left",
        show_lines=False,
    )
    table.add_column("#", justify="right")
    table.add_column("Count", justify="right")
    table.add_column("Rule", overflow="fold")

    sorted_cur = sorted(cur.items(), key=lambda p: (-p[1], p[0]))
    for rank, (rule, count) in enumerate(sorted_cur, start=1):
        baseline_count = base.get(rule)
        rule_label = rule if baseline_count is not None else f"[green]{rule} (new)[/green]"
        table.add_row(str(rank), _delta_cell(count, baseline_count), rule_label)

    removed = sorted(set(base) - set(cur))
    for rule in removed:
        table.add_row("[red]–[/red]", _delta_cell(None, base[rule]), f"[red]{rule} (removed)[/red]")

    console.print(table)


def _render_unused_rules(baseline: dict, current: dict, console: Console) -> None:
    base_set = set(baseline.get("rules", {}).get("unused", []))
    cur_list = sorted(current.get("rules", {}).get("unused", []))
    cur_set = set(cur_list)

    console.print(Rule(f"Unused inference rules ({len(cur_list)})", align="left"))
    if not cur_list:
        console.print("  (none)")
    else:
        for rule in cur_list:
            if rule not in base_set:
                console.print(f"  [green]+ {rule}[/green]")
            else:
                console.print(f"    {rule}")

    moved_to_used = sorted(base_set - cur_set)
    if moved_to_used:
        console.print()
        console.print(f"[bold]No longer unused (now used) ({len(moved_to_used)}):[/bold]")
        for rule in moved_to_used:
            console.print(f"  [green]- {rule}[/green]")


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

    console = Console()
    console.print(
        Rule(f"Prover benchmark report ({args.baseline_label} → {args.current_label})", align="left")
    )

    if baseline == current:
        console.print("[bold]Reports are identical.[/bold]")

    _render_inputs(baseline, current, console)
    _render_used_rules(baseline, current, console)
    _render_unused_rules(baseline, current, console)
    return 0


if __name__ == "__main__":
    sys.exit(main())
