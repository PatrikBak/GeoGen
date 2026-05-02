#!/usr/bin/env bash
# Regenerate the JSON report dump consumed by Web/geogen-viewer/.
# Builds the integration test, runs the prover over every registered scenario,
# and writes manifest.json + scenarios/*.json under the test's bin/ folder.
set -euo pipefail

cd "$(dirname "$0")/../Source/Tests/IntegrationTests/GeoGen.TheoremProver.IntegrationTest"

dotnet run -- \
    ../../../Launchers/GeoGen.MainLauncher/Data/InferenceRules \
    txt \
    ../../../Launchers/GeoGen.MainLauncher/Data/object_introduction_rules.txt

echo
echo "Reports written to:"
echo "  $(pwd)/bin/Debug/net10.0/json-reports/"
