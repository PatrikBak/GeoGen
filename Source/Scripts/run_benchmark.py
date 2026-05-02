#!/usr/bin/env python3
"""Build the launcher, run the prover benchmark in parallel, and produce a report.

One launcher process per input file in ``ProverBenchmark/Inputs``. Each gets its
own settings file pointing at a per-input output sub-folder so the per-process
``inference_rule_usages.txt`` files don't clobber each other.
"""

from __future__ import annotations

import argparse
import json
import shutil
import subprocess
import sys
from concurrent.futures import ThreadPoolExecutor, as_completed
from pathlib import Path

REPO_ROOT = Path(__file__).resolve().parents[2]
SOURCE_DIR = REPO_ROOT / "Source"
LAUNCHER_BIN = SOURCE_DIR / "Launchers" / "GeoGen.MainLauncher" / "bin" / "Release" / "net10.0"
REPORT_SCRIPT = Path(__file__).resolve().parent / "benchmark_report.py"
BASE_SETTINGS = LAUNCHER_BIN / "ProverBenchmark" / "settings.json"


def step(msg: str) -> None:
    print(f"==> {msg}", flush=True)


def _path_for_settings(p: Path) -> str:
    try:
        return p.relative_to(LAUNCHER_BIN).as_posix()
    except ValueError:
        return p.resolve().as_posix()


def write_settings_for(stem: str, out_dir: Path) -> Path:
    rel = _path_for_settings(out_dir)
    with BASE_SETTINGS.open() as f:
        settings = json.load(f)
    settings["ProblemGeneratorInputProviderSettings"]["InputFilePrefix"] = stem
    runner = settings["ProblemGenerationRunnerSettings"]
    runner["ReadableOutputWithoutProofsFolder"] = f"{rel}/ReadableWithoutProofs"
    runner["ReadableOutputWithProofsFolder"] = f"{rel}/ReadableWithProofs"
    runner["JsonOutputFolder"] = f"{rel}/JsonOutput"
    runner["ReadableBestTheoremFolder"] = f"{rel}/ReadableBestTheorems"
    runner["JsonBestTheoremFolder"] = f"{rel}/JsonBestTheorems"
    runner["InferenceRuleUsageFilePath"] = f"{rel}/inference_rule_usages.txt"
    settings["Serilog"]["WriteTo"] = [
        w for w in settings["Serilog"]["WriteTo"] if w.get("Name") != "File"
    ]
    settings["Serilog"]["WriteTo"].append(
        {"Name": "File", "Args": {"Path": f"{rel}/launcher.log"}}
    )
    dest = out_dir / "settings.json"
    with dest.open("w") as f:
        json.dump(settings, f, indent=2)
    return dest


def run_launcher(stem: str, out_dir: Path) -> tuple[str, int]:
    for sub in ("ReadableWithoutProofs", "ReadableWithProofs", "JsonOutput",
                "ReadableBestTheorems", "JsonBestTheorems"):
        (out_dir / sub).mkdir(parents=True, exist_ok=True)
    settings_file = write_settings_for(stem, out_dir)
    log = out_dir / "stdout.log"
    with log.open("wb") as f:
        rc = subprocess.run(
            ["dotnet", "GeoGen.dll", _path_for_settings(settings_file)],
            cwd=LAUNCHER_BIN,
            stdout=f,
            stderr=subprocess.STDOUT,
        ).returncode
    return stem, rc


MODE_INPUTS: dict[str, set[str] | None] = {
    "very-fast": {"input_incenter"},
    "fast": {"input_triangle_centroid"},
    "slow": None,
}


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument(
        "--mode",
        choices=sorted(MODE_INPUTS.keys()),
        default="slow",
        help=(
            "Which inputs to run. very-fast: input_incenter (smoke test); "
            "fast: input_triangle_centroid; slow: every input_*.txt (default)."
        ),
    )
    parser.add_argument(
        "--skip-build",
        action="store_true",
        help="Skip the dotnet build step (use existing binaries).",
    )
    parser.add_argument(
        "--output-dir",
        type=Path,
        default=None,
        help=(
            "Override the output root (default: bin/.../ProverBenchmark/Output). "
            "Useful for collecting reports from multiple revisions side by side."
        ),
    )
    args = parser.parse_args()

    if not args.skip_build:
        step("Building solution (Release)")
        subprocess.run(
            ["dotnet", "build", str(SOURCE_DIR / "GeoGen.sln"), "--configuration", "Release"],
            check=True,
        )

    step("Preparing benchmark output directories")
    output_root = args.output_dir or (LAUNCHER_BIN / "ProverBenchmark" / "Output")
    if output_root.exists():
        shutil.rmtree(output_root)
    output_root.mkdir(parents=True)

    inputs_dir = LAUNCHER_BIN / "ProverBenchmark" / "Inputs"
    stems = sorted(p.stem for p in inputs_dir.glob("input_*.txt"))
    selection = MODE_INPUTS[args.mode]
    if selection is not None:
        stems = [s for s in stems if s in selection]
    if not stems:
        print(f"no input files matched mode={args.mode} under {inputs_dir}", file=sys.stderr)
        return 1

    step(f"Running {len(stems)} benchmark inputs in parallel: {' '.join(stems)}")

    failed: list[str] = []
    with ThreadPoolExecutor(max_workers=len(stems)) as pool:
        futures = {
            pool.submit(run_launcher, stem, output_root / stem): stem
            for stem in stems
        }
        for stem in stems:
            print(f"    -> {stem}", flush=True)
        for future in as_completed(futures):
            stem, rc = future.result()
            if rc != 0:
                failed.append(stem)
                print(f"    !! {stem} exited with rc={rc}", flush=True, file=sys.stderr)

    if failed:
        print(
            f"benchmark failed for: {', '.join(failed)}; see stdout.log per input dir",
            file=sys.stderr,
        )
        return 1

    step("Generating report")
    report_args: list[str] = []
    for stem in stems:
        d = output_root / stem
        report_args += ["--proofs-dir", _path_for_settings(d / "ReadableWithProofs")]
        report_args += ["--rule-usages", _path_for_settings(d / "inference_rule_usages.txt")]
    report_path = output_root / "report.json"
    subprocess.run(
        [
            sys.executable,
            str(REPORT_SCRIPT),
            *report_args,
            "--output",
            _path_for_settings(report_path),
        ],
        cwd=LAUNCHER_BIN,
        check=True,
    )

    print()
    step(f"Done. Report written to: {report_path}")
    return 0


if __name__ == "__main__":
    sys.exit(main())
