#!/usr/bin/env python3
"""Aggregate prover benchmark results into a markdown/text report.

Reads ``ReadableWithProofs/output_*.txt`` files produced by ``GeoGen.MainLauncher``
together with the cumulative ``inference_rule_usages.txt`` and emits a summary:

* per input file: theorems found / proved / unproved
* globally: top-N inference rules by usage count
"""

from __future__ import annotations

import argparse
import re
import sys
from dataclasses import dataclass
from pathlib import Path

THEOREM_LINE_RE = re.compile(r"^\s*\d+\.\s")
SECTION_RE = re.compile(r"^(Proved theorems|Not interesting theorems|Interesting theorems):\s*$")
RULE_LINE_RE = re.compile(r"^\s*\[(\d+)\]\s*-->\s*(.+?)\s*$")


@dataclass
class InputStats:
    name: str
    proved: int = 0
    unproved: int = 0
    excluded: int = 0  # "not interesting" — excluded by symmetry filter

    @property
    def found(self) -> int:
        return self.proved + self.unproved + self.excluded


def parse_readable_with_proofs(path: Path) -> InputStats:
    stats = InputStats(name=path.stem)
    section: str | None = None

    for raw in path.read_text(encoding="utf-8").splitlines():
        match = SECTION_RE.match(raw)
        if match:
            section = match.group(1)
            continue

        if section is None:
            continue

        if not raw.strip():
            # blank line ends the current section's "first numbered list"
            # but proofs span multiple lines, so we only switch via headers above
            continue

        # Theorem entries always start with " <n>. " at top level.
        # Sub-proof lines for the "Proved theorems" section look like
        # "  <n>.<m>. ..." which start with two spaces and a multi-dotted index.
        # We only count top-level entries: 1-space indent + single-dot index.
        if not raw.startswith(" "):
            section = None
            continue
        if raw.startswith("  "):
            continue
        if not THEOREM_LINE_RE.match(raw):
            continue

        if section == "Proved theorems":
            stats.proved += 1
        elif section == "Interesting theorems":
            stats.unproved += 1
        elif section == "Not interesting theorems":
            stats.excluded += 1

    return stats


def parse_rule_usages(paths: list[Path]) -> list[tuple[str, int]]:
    totals: dict[str, int] = {}
    for path in paths:
        if not path.exists():
            continue
        for raw in path.read_text(encoding="utf-8").splitlines():
            match = RULE_LINE_RE.match(raw)
            if not match:
                continue
            rule = match.group(2)
            totals[rule] = totals.get(rule, 0) + int(match.group(1))
    return sorted(totals.items(), key=lambda pair: (-pair[1], pair[0]))


def render(stats: list[InputStats], rules: list[tuple[str, int]], top_n: int | None) -> str:
    lines: list[str] = []
    lines.append("# Prover Benchmark Report")
    lines.append("")
    lines.append("## Theorems per input")
    lines.append("")
    lines.append("| Input | Found | Proved | Unproved | Excluded (symmetry) |")
    lines.append("|---|---:|---:|---:|---:|")
    totals = InputStats(name="TOTAL")
    for s in stats:
        lines.append(
            f"| {s.name} | {s.found} | {s.proved} | {s.unproved} | {s.excluded} |"
        )
        totals.proved += s.proved
        totals.unproved += s.unproved
        totals.excluded += s.excluded
    lines.append(
        f"| **{totals.name}** | **{totals.found}** | **{totals.proved}** | "
        f"**{totals.unproved}** | **{totals.excluded}** |"
    )

    used = [(n, c) for n, c in rules if c > 0]
    unused = [n for n, c in rules if c == 0]

    lines.append("")
    if top_n is None:
        shown = used
        heading = f"## All inference rules used ({len(used)})"
    else:
        shown = used[:top_n]
        heading = f"## Top {top_n} inference rules used"
    lines.append(heading)
    lines.append("")
    if not used:
        lines.append("_No rule usage data found._")
    else:
        lines.append("| # | Count | Rule |")
        lines.append("|---:|---:|---|")
        for i, (name, count) in enumerate(shown, start=1):
            lines.append(f"| {i} | {count} | `{name}` |")

    lines.append("")
    lines.append(f"## Unused inference rules ({len(unused)})")
    lines.append("")
    if not unused:
        lines.append("_None — every loaded rule fired at least once._")
    else:
        for name in sorted(unused):
            lines.append(f"- `{name}`")

    lines.append("")
    return "\n".join(lines)


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument(
        "--proofs-dir",
        action="append",
        required=True,
        type=Path,
        help="Folder containing ReadableWithProofs/output_*.txt files. Repeatable.",
    )
    parser.add_argument(
        "--rule-usages",
        action="append",
        required=True,
        type=Path,
        help="Path to an inference_rule_usages.txt file. Repeatable; counts are summed.",
    )
    parser.add_argument(
        "--output",
        type=Path,
        default=None,
        help="Write the markdown report to this file as well as stdout",
    )
    parser.add_argument(
        "--top-rules",
        type=int,
        default=15,
        help="How many rules to include in the top-rules table (default 15)",
    )
    parser.add_argument(
        "--all-rules",
        action="store_true",
        help="Include every inference rule with non-zero usage; overrides --top-rules.",
    )
    args = parser.parse_args()

    proof_files: list[tuple[str, Path]] = []
    for proofs_dir in args.proofs_dir:
        if not proofs_dir.is_dir():
            print(f"error: proofs dir not found: {proofs_dir}", file=sys.stderr)
            return 1
        for path in sorted(proofs_dir.glob("output*.txt")):
            display = path.stem if path.stem != "output" else proofs_dir.parent.name
            proof_files.append((display, path))

    if not proof_files:
        print("error: no output*.txt files found", file=sys.stderr)
        return 1

    def stats_with_name(name: str, path: Path) -> InputStats:
        s = parse_readable_with_proofs(path)
        s.name = name
        return s

    stats = sorted(
        (stats_with_name(name, path) for name, path in proof_files),
        key=lambda s: s.name,
    )
    rules = parse_rule_usages(args.rule_usages)

    report = render(stats, rules, None if args.all_rules else args.top_rules)
    print(report)
    if args.output:
        args.output.parent.mkdir(parents=True, exist_ok=True)
        args.output.write_text(report, encoding="utf-8")
    return 0


if __name__ == "__main__":
    sys.exit(main())
