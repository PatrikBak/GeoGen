#!/usr/bin/env python3
"""Aggregate prover benchmark results into a structured JSON report.

Reads ``ReadableWithProofs/output*.txt`` files produced by ``GeoGen.MainLauncher``
together with ``inference_rule_usages.txt`` files and writes a JSON document
covering theorems found / proved / unproved per input, totals, and inference
rule usage (with unused rules listed separately).
"""

from __future__ import annotations

import argparse
import json
import re
import sys
from dataclasses import asdict, dataclass
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
            continue

        # Top-level theorem lines start with a single space + "<n>.".
        # Nested proof lines start with two spaces and a multi-dotted index;
        # only the top-level lines should count.
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


def build_report(stats: list[InputStats], rules: list[tuple[str, int]]) -> dict:
    total = InputStats(name="TOTAL")
    for s in stats:
        total.proved += s.proved
        total.unproved += s.unproved
        total.excluded += s.excluded

    inputs_payload = [
        {
            "name": s.name,
            "found": s.found,
            "proved": s.proved,
            "unproved": s.unproved,
            "excluded": s.excluded,
        }
        for s in stats
    ]
    totals_payload = {
        "found": total.found,
        "proved": total.proved,
        "unproved": total.unproved,
        "excluded": total.excluded,
    }
    used = [{"rule": name, "count": count} for name, count in rules if count > 0]
    unused = sorted(name for name, count in rules if count == 0)

    return {
        "inputs": inputs_payload,
        "totals": totals_payload,
        "rules": {"used": used, "unused": unused},
    }


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument(
        "--proofs-dir",
        action="append",
        required=True,
        type=Path,
        help="Folder containing ReadableWithProofs/output*.txt files. Repeatable.",
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
        required=True,
        help="Write the JSON report to this file.",
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
    report = build_report(stats, rules)

    args.output.parent.mkdir(parents=True, exist_ok=True)
    with args.output.open("w", encoding="utf-8") as f:
        json.dump(report, f, indent=2)
        f.write("\n")
    return 0


if __name__ == "__main__":
    sys.exit(main())
