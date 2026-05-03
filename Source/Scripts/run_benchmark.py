#!/usr/bin/env -S uv run --script
# /// script
# requires-python = ">=3.10"
# ///
"""Build the launcher, run the prover benchmark in parallel, and produce a report.

One launcher process per input file in ``ProverBenchmark/Inputs``. Each gets its
own settings file pointing at a per-input output sub-folder so the per-process
``inference_rule_usages.txt`` files don't clobber each other.

With ``--compare-with <ref>``, the script also runs the benchmark from another
git revision in a temporary worktree and prints a structured diff of the two
``report.json`` files to stdout.
"""

from __future__ import annotations

import argparse
import json
import os
import shutil
import subprocess
import sys
import tempfile
import time
from concurrent.futures import ThreadPoolExecutor, as_completed
from dataclasses import dataclass
from pathlib import Path

THIS_FILE = Path(__file__).resolve()
DEFAULT_REPO_ROOT = THIS_FILE.parents[2]
REPORT_SCRIPT = THIS_FILE.parent / "benchmark_report.py"
DIFF_SCRIPT = THIS_FILE.parent / "benchmark_diff.py"

MODE_INPUTS: dict[str, set[str] | None] = {
    "very-fast": {"input_incenter"},
    "fast": {"input_triangle_centroid"},
    "slow": None,
}


@dataclass
class Workspace:
    """Paths derived from a repo checkout."""
    root: Path

    @property
    def source_dir(self) -> Path:
        return self.root / "Source"

    @property
    def solution(self) -> Path:
        return self.source_dir / "GeoGen.sln"

    @property
    def launcher_bin(self) -> Path:
        return self.source_dir / "Launchers" / "GeoGen.MainLauncher" / "bin" / "Release" / "net10.0"

    @property
    def base_settings(self) -> Path:
        return self.launcher_bin / "ProverBenchmark" / "settings.json"

    @property
    def inputs_dir(self) -> Path:
        return self.launcher_bin / "ProverBenchmark" / "Inputs"


def step(msg: str) -> None:
    print(f"==> {msg}", flush=True)


def _path_for_settings(p: Path, ws: Workspace) -> str:
    try:
        return p.relative_to(ws.launcher_bin).as_posix()
    except ValueError:
        return p.resolve().as_posix()


def write_settings_for(stem: str, out_dir: Path, ws: Workspace) -> Path:
    rel = _path_for_settings(out_dir, ws)
    with ws.base_settings.open() as f:
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


def run_launcher(stem: str, out_dir: Path, ws: Workspace) -> tuple[str, int, float]:
    for sub in ("ReadableWithoutProofs", "ReadableWithProofs", "JsonOutput",
                "ReadableBestTheorems", "JsonBestTheorems"):
        (out_dir / sub).mkdir(parents=True, exist_ok=True)
    settings_file = write_settings_for(stem, out_dir, ws)
    log = out_dir / "stdout.log"
    # The launcher's exit hook calls Console.ReadKey() unless $TERM is set,
    # which crashes when stdin is captured (CI, our subprocess pipe). Force it.
    env = {**os.environ, "TERM": os.environ.get("TERM", "dumb")}
    started = time.monotonic()
    with log.open("wb") as f:
        rc = subprocess.run(
            ["dotnet", "GeoGen.dll", _path_for_settings(settings_file, ws)],
            cwd=ws.launcher_bin,
            stdout=f,
            stderr=subprocess.STDOUT,
            stdin=subprocess.DEVNULL,
            env=env,
        ).returncode
    elapsed = time.monotonic() - started
    return stem, rc, elapsed


def run_benchmark(
    *,
    ws: Workspace,
    mode: str,
    output_root: Path,
    skip_build: bool,
) -> Path:
    """Build (optionally), run the launcher in parallel, and write report.json.

    Returns the path to the produced report.json.
    """
    if not skip_build:
        step(f"Building solution (Release) at {ws.root}")
        subprocess.run(
            ["dotnet", "build", str(ws.solution), "--configuration", "Release"],
            check=True,
        )

    step(f"Preparing benchmark output directory: {output_root}")
    if output_root.exists():
        shutil.rmtree(output_root)
    output_root.mkdir(parents=True)

    stems = sorted(p.stem for p in ws.inputs_dir.glob("input_*.txt"))
    selection = MODE_INPUTS[mode]
    if selection is not None:
        stems = [s for s in stems if s in selection]
    if not stems:
        raise RuntimeError(f"no input files matched mode={mode} under {ws.inputs_dir}")

    step(f"Running {len(stems)} benchmark inputs in parallel: {' '.join(stems)}")

    failed: list[str] = []
    runtimes: dict[str, float] = {}
    with ThreadPoolExecutor(max_workers=len(stems)) as pool:
        futures = {
            pool.submit(run_launcher, stem, output_root / stem, ws): stem
            for stem in stems
        }
        for stem in stems:
            print(f"    -> {stem}", flush=True)
        for future in as_completed(futures):
            stem, rc, elapsed = future.result()
            runtimes[stem] = elapsed
            if rc != 0:
                failed.append(stem)
                print(f"    !! {stem} exited with rc={rc} after {elapsed:.1f}s",
                      flush=True, file=sys.stderr)
                stdout_log = output_root / stem / "stdout.log"
                if stdout_log.exists():
                    tail = stdout_log.read_text(errors="replace").splitlines()[-200:]
                    print(
                        f"--- last 200 lines of {stdout_log} ---",
                        file=sys.stderr,
                    )
                    print("\n".join(tail), file=sys.stderr)
                    print(f"--- end of {stdout_log} ---", file=sys.stderr)
            else:
                print(f"    ok {stem} in {elapsed:.1f}s", flush=True)

    if failed:
        raise RuntimeError(
            f"benchmark failed for: {', '.join(failed)}; see stdout.log per input dir"
        )

    step("Generating report")
    report_args: list[str] = []
    for stem in stems:
        d = output_root / stem
        report_args += ["--proofs-dir", _path_for_settings(d / "ReadableWithProofs", ws)]
        report_args += ["--rule-usages", _path_for_settings(d / "inference_rule_usages.txt", ws)]
        if stem in runtimes:
            report_args += ["--runtime", f"{stem}={runtimes[stem]:.3f}"]
    report_path = output_root / "report.json"
    subprocess.run(
        [
            "uv", "run", "--script", str(REPORT_SCRIPT),
            *report_args,
            "--output",
            _path_for_settings(report_path, ws),
        ],
        cwd=ws.launcher_bin,
        check=True,
    )
    return report_path


def run_in_worktree(ref: str, mode: str, output_dir: Path) -> Path:
    """Check out ``ref`` into a temporary worktree, run the benchmark, return report.json path."""
    step(f"Creating temporary worktree at {ref}")
    worktree_path = Path(tempfile.mkdtemp(prefix="geogen-bench-"))
    worktree_path.rmdir()  # `git worktree add` wants the path to not exist
    try:
        subprocess.run(
            ["git", "worktree", "add", "--detach", str(worktree_path), ref],
            cwd=DEFAULT_REPO_ROOT,
            check=True,
        )
        ws = Workspace(root=worktree_path)
        return run_benchmark(
            ws=ws,
            mode=mode,
            output_root=output_dir,
            skip_build=False,
        )
    finally:
        subprocess.run(
            ["git", "worktree", "remove", "--force", str(worktree_path)],
            cwd=DEFAULT_REPO_ROOT,
            check=False,
        )


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
        help="Skip the dotnet build step for the current checkout.",
    )
    parser.add_argument(
        "--output-dir",
        type=Path,
        default=None,
        help=(
            "Override the output root for the current-checkout run "
            "(default: bin/.../ProverBenchmark/Output)."
        ),
    )
    parser.add_argument(
        "--compare-with",
        nargs="?",
        const="origin/master",
        default=None,
        metavar="REF",
        help=(
            "Also run the benchmark from another git revision in a temporary "
            "worktree and print a diff. Defaults to 'origin/master' if no value."
        ),
    )
    args = parser.parse_args()

    ws = Workspace(root=DEFAULT_REPO_ROOT)
    output_root = args.output_dir or (ws.launcher_bin / "ProverBenchmark" / "Output")

    try:
        current_report = run_benchmark(
            ws=ws,
            mode=args.mode,
            output_root=output_root,
            skip_build=args.skip_build,
        )
    except RuntimeError as e:
        print(str(e), file=sys.stderr)
        return 1

    print()
    step(f"Current-checkout report: {current_report}")

    if args.compare_with is None:
        return 0

    print()
    step(f"Comparing with {args.compare_with}")
    baseline_dir = output_root.parent / f"Output-{args.compare_with.replace('/', '_')}"
    try:
        baseline_report = run_in_worktree(args.compare_with, args.mode, baseline_dir)
    except RuntimeError as e:
        msg = str(e)
        if "no input files matched" in msg:
            print(
                f"skipping comparison: {args.compare_with} has no benchmark inputs "
                "for this mode (probably predates the benchmark).",
                file=sys.stderr,
            )
            return 0
        print(f"comparison run failed: {e}", file=sys.stderr)
        return 1
    except subprocess.CalledProcessError as e:
        print(f"comparison run failed: {e}", file=sys.stderr)
        return 1

    print()
    step(f"Diff ({args.compare_with} → current):")
    print()
    subprocess.run(
        [
            "uv", "run", "--script", str(DIFF_SCRIPT),
            str(baseline_report),
            str(current_report),
            "--baseline-label",
            args.compare_with,
            "--current-label",
            "current",
        ],
        check=False,
    )
    return 0


if __name__ == "__main__":
    sys.exit(main())
