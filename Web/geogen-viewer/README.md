# GeoGen Prover Report Viewer

Stateless web viewer for JSON dumps produced by the GeoGen TheoremProver
integration test. Drop a `manifest.json` + `scenarios/*.json` folder onto
the upload zone; pick a scenario; explore the configuration, proved/unproved
theorems, and inference trace with the diagram highlighted on hover/click.

## Generate the JSON data

The C# integration test produces the JSON. From the repo root:

```bash
./Scripts/regen-reports.sh
```

The script `cd`s into the integration-test project and invokes `dotnet run`
with the inference-rule folder, the rule-file extension, and the
object-introduction rule file as positional arguments. It assumes `dotnet`
is on `PATH`; if you installed via `dotnet-install.sh`, add `~/.dotnet` to
your shell's `PATH`.

After the run finishes, the dump lands in:

```
Source/Tests/IntegrationTests/GeoGen.TheoremProver.IntegrationTest/bin/Debug/net10.0/json-reports/
├── manifest.json
└── scenarios/
    ├── OrthicTriangle.json
    ├── IncenterMedianConcurrency.json
    └── …
```

To add or change a scenario, edit the `Test configurations` region near the
bottom of `Program.cs` and register the new factory in the dispatch list at
the top of `Main`.

## Run the viewer locally

From this directory (`Web/geogen-viewer/`):

```bash
npm install      # first time only
npm run dev      # http://localhost:5173 with hot-reload
```

Open the URL, drag the entire `json-reports/` folder onto the upload zone.
The index lists every scenario; click one to enter its report.

## Production build / preview

```bash
npm run build    # type-checks (tsc -b) then builds to dist/
npm run preview  # serves dist/ on http://localhost:4173
```

The build emits a fully static bundle in `dist/` — drop it on any static
host or open `dist/index.html` directly. There is no server component.

## Schema versioning

`Web/geogen-viewer/src/schema.ts` exports `EXPECTED_SCHEMA`; the C# side has
`ReportSchema.SchemaVersion`. Both must match. Mismatches surface as a clear
error from the upload validator — no silent partial render.

When the schema shape changes:

- Adding an optional field is non-breaking; leave the version alone.
- Removing or renaming a field, or making an optional field required, bumps
  the version on both sides.
