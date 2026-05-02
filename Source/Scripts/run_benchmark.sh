#!/usr/bin/env bash
# Build the launcher, run the prover benchmark in parallel (one process per
# input), and produce a merged report listing every used inference rule.
# Usage: Source/Scripts/run_benchmark.sh
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "${SCRIPT_DIR}/../.." && pwd)"
SOURCE_DIR="${REPO_ROOT}/Source"
LAUNCHER_BIN="${SOURCE_DIR}/Launchers/GeoGen.MainLauncher/bin/Release/net10.0"
REPORT_SCRIPT="${SCRIPT_DIR}/benchmark_report.py"
BASE_SETTINGS="${LAUNCHER_BIN}/ProverBenchmark/settings.json"

echo "==> Building solution (Release)"
dotnet build "${SOURCE_DIR}/GeoGen.sln" --configuration Release

echo "==> Preparing benchmark output directories"
cd "${LAUNCHER_BIN}"
rm -rf ProverBenchmark/Output
mkdir -p ProverBenchmark/Output

mapfile -t INPUTS < <(ls ProverBenchmark/Inputs/input_*.txt | xargs -n1 basename | sed 's/\.txt$//')
if [ "${#INPUTS[@]}" -eq 0 ]; then
  echo "no input files found in ProverBenchmark/Inputs" >&2
  exit 1
fi

echo "==> Running ${#INPUTS[@]} benchmark inputs in parallel: ${INPUTS[*]}"

declare -a PIDS=()
for stem in "${INPUTS[@]}"; do
  out_dir="ProverBenchmark/Output/${stem}"
  mkdir -p \
    "${out_dir}/ReadableWithoutProofs" \
    "${out_dir}/ReadableWithProofs" \
    "${out_dir}/JsonOutput" \
    "${out_dir}/ReadableBestTheorems" \
    "${out_dir}/JsonBestTheorems"

  settings_file="${out_dir}/settings.json"
  python3 - "${BASE_SETTINGS}" "${stem}" "${out_dir}" "${settings_file}" <<'PY'
import json, sys
base, stem, out_dir, dest = sys.argv[1:]
with open(base) as f:
    s = json.load(f)
s["ProblemGeneratorInputProviderSettings"]["InputFilePrefix"] = stem
runner = s["ProblemGenerationRunnerSettings"]
runner["ReadableOutputWithoutProofsFolder"] = f"{out_dir}/ReadableWithoutProofs"
runner["ReadableOutputWithProofsFolder"]    = f"{out_dir}/ReadableWithProofs"
runner["JsonOutputFolder"]                  = f"{out_dir}/JsonOutput"
runner["ReadableBestTheoremFolder"]         = f"{out_dir}/ReadableBestTheorems"
runner["JsonBestTheoremFolder"]             = f"{out_dir}/JsonBestTheorems"
runner["InferenceRuleUsageFilePath"]        = f"{out_dir}/inference_rule_usages.txt"
s["Serilog"]["WriteTo"] = [w for w in s["Serilog"]["WriteTo"] if w.get("Name") != "File"]
s["Serilog"]["WriteTo"].append({"Name": "File", "Args": {"Path": f"{out_dir}/launcher.log"}})
with open(dest, "w") as f:
    json.dump(s, f, indent=2)
PY

  echo "    -> ${stem}"
  ( dotnet GeoGen.dll "${settings_file}" > "${out_dir}/stdout.log" 2>&1 ) &
  PIDS+=("$!")
done

fail=0
for pid in "${PIDS[@]}"; do
  if ! wait "${pid}"; then
    fail=1
  fi
done

if [ "${fail}" -ne 0 ]; then
  echo "one or more benchmark inputs failed; see stdout.log per input dir" >&2
  exit 1
fi

echo "==> Generating report"
declare -a REPORT_ARGS=()
for stem in "${INPUTS[@]}"; do
  REPORT_ARGS+=( --proofs-dir "ProverBenchmark/Output/${stem}/ReadableWithProofs" )
  REPORT_ARGS+=( --rule-usages "ProverBenchmark/Output/${stem}/inference_rule_usages.txt" )
done

python3 "${REPORT_SCRIPT}" \
  "${REPORT_ARGS[@]}" \
  --all-rules \
  --output ProverBenchmark/Output/report.md

echo
echo "==> Done. Report written to: ${LAUNCHER_BIN}/ProverBenchmark/Output/report.md"
