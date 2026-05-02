#!/usr/bin/env python3
"""Print a unified diff of two benchmark report files.

Always exits 0 — meant for informational output (e.g. dumped to a CI job
summary), not for failing builds.
"""

from __future__ import annotations

import argparse
import difflib
import sys
from pathlib import Path


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("baseline", type=Path, help="Older / reference report.md")
    parser.add_argument("current", type=Path, help="New report.md to compare")
    parser.add_argument(
        "--baseline-label",
        default=None,
        help="Label for the baseline file in the diff header (default: path)",
    )
    parser.add_argument(
        "--current-label",
        default=None,
        help="Label for the current file in the diff header (default: path)",
    )
    args = parser.parse_args()

    if not args.baseline.is_file():
        print(f"error: baseline file not found: {args.baseline}", file=sys.stderr)
        return 1
    if not args.current.is_file():
        print(f"error: current file not found: {args.current}", file=sys.stderr)
        return 1

    baseline = args.baseline.read_text(encoding="utf-8").splitlines(keepends=True)
    current = args.current.read_text(encoding="utf-8").splitlines(keepends=True)

    diff = list(
        difflib.unified_diff(
            baseline,
            current,
            fromfile=args.baseline_label or str(args.baseline),
            tofile=args.current_label or str(args.current),
        )
    )

    if not diff:
        print("Reports are identical.")
    else:
        sys.stdout.writelines(diff)

    return 0


if __name__ == "__main__":
    sys.exit(main())
