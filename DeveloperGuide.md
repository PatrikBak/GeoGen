# Developer Guide

This section is intended for developers who want to modify the source code or build it themselves.

## Development Setup

### Prerequisites

The core programming language of the software is C#. To build the project, you need to have [.NET 10.0](https://dotnet.microsoft.com/download/dotnet/10.0) installed.

The application can be built by running `dotnet build` in the [Source](Source) folder.

Automated tests can be run via `dotnet test` in the [Source](Source) folder.

## Theorem Proving Integration Test

To review the capabilities of the theorem prover, run [this test](Source/Tests/IntegrationTests/GeoGen.TheoremProver.IntegrationTest/Program.cs). The `main` method requires two arguments:

1. Path to the folder with inference rules.
2. Path to the folder with object introduction rules.

Default values that should work after cloning can be found in [launchSettings.json](Source/Tests/IntegrationTests/GeoGen.TheoremProver.IntegrationTest/Properties/launchSettings.json).

## Helper Modules

The following modules were used to prepare the large-scale experiments described in the [thesis](https://drive.google.com/file/d/1dsaxDCMzlAPfB3e4rd8ut2RuZ_sn2Zm5/view?usp=sharing):

- **Configuration Generation Launcher** - Used to test generated configuration counts for input files (and memory usage). Can be used similarly to the _Main Launcher_, with default settings [here](Source/Launchers/GeoGen.ConfigurationGenerationLauncher/settings.json). Copy these settings to the executable's folder. Output is printed to the console only.
- **Input Generation Launcher** - Generates input files. Running it creates around 4500 small files, totaling 4MB.
- **Output Merging Launcher** - Merges the final results. The `main` method requires one argument: the path to the folder with JSON outputs (scanned recursively).
