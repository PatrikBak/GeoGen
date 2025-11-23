# User Guide

This guide explains how to use the application.

**Related Documentation:**

- **[Input/Output Format Reference](InputOutputFormat.md)** - Complete technical specification of file formats (all possible values, structures, examples)

## Main Launcher

This is the main module that runs the algorithm. The application tries to read settings from the file `settings.json`. Example settings are available [here](Source/Launchers/GeoGen.MainLauncher/settings.json).

- Input files are loaded from the [input folder](Source/Launchers/GeoGen.MainLauncher/Examples/Inputs). See [Input/Output Format Reference](InputOutputFormat.md#input-file-format) for complete details on creating input files.
- Output is written to the following subdirectories of the [output folder](Source/Launchers/GeoGen.MainLauncher/Examples/Output):

  1. [**ReadableWithoutProofs**](Source/Launchers/GeoGen.MainLauncher/Examples/Output/ReadableWithoutProofs) - All found theorems that haven't been excluded.
  2. [**ReadableWithProofs**](Source/Launchers/GeoGen.MainLauncher/Examples/Output/ReadableWithProofs) - All found theorems, with reasons why some have been excluded.
  3. [**ReadableBestTheorems**](Source/Launchers/GeoGen.MainLauncher/Examples/Output/ReadableBestTheorems) - The best theorems for each type, ordered by their ranking.
  4. [**JsonOutput**](Source/Launchers/GeoGen.MainLauncher/Examples/Output/JsonOutput) - All found theorems that haven't been excluded, in JSON format.
  5. [**JsonBestTheorems**](Source/Launchers/GeoGen.MainLauncher/Examples/Output/JsonBestTheorems) - Same as _ReadableBestTheorems_, but in JSON format.

  - **Output file format:** See [Input/Output Format Reference](InputOutputFormat.md#output-file-format) for complete details on output file structure.

> **Note:** The `settings.json` file contains well-chosen defaults. For typical usage, you do **not** need to modify this file.

## Concepts

### Constructions

Constructions are geometric operations used to create new objects from existing ones. They are used in input files, inference rules, object introduction rules, and drawing rules.

**See:** [Complete list of available constructions](InputOutputFormat.md#constructions) in the Input/Output Format Reference.

### Inference Rules

The inference rules used by the theorem prover can be found [here](Source/Library/GeoGen.TheoremProver.InferenceRuleProvider/Rules). They use the [constructions](InputOutputFormat.md#constructions) defined in the Input/Output Format Reference.

### Object Introduction Rules

The object introduction rules used can be found [here](Source/Library/GeoGen.TheoremProver.ObjectIntroductionRuleProvider/Rules/object_introduction_rules.txt). They use the [constructions](InputOutputFormat.md#constructions) defined in the Input/Output Format Reference.

## Drawing Launcher

This module visualizes the results produced by the _Main Launcher_ by generating [EPS](https://en.wikipedia.org/wiki/Encapsulated_PostScript) figures using [MetaPost](https://en.wikipedia.org/wiki/MetaPost).

### Requirements

- [MikTeX](https://miktex.org/download) (other distributions haven't been tested but will probably work)
- [OPmac](http://petr.olsak.net/opmac.html) (used for quick font selection while printing problem texts)

To preview generated results, I recommend:

- [SumatraPDF](https://www.sumatrapdfreader.org/free-pdf-reader.html)
- [Ghostscript](https://ghostscript.com/) (required by Sumatra for rendering)

### Running

The application attempts to read settings from the file `settings.json`. Example settings are available [here](Source/Launchers/GeoGen.DrawingLauncher/settings.json).

After running, you will be prompted to provide a path to a JSON theorem file. These files are produced by the _Main Launcher_. In the examples, these are found in [**JsonOutput**](Source/Launchers/GeoGen.MainLauncher/Examples/Output/JsonOutput) and [**JsonBestTheorems**](Source/Launchers/GeoGen.MainLauncher/Examples/Output/JsonBestTheorems). After selecting the theorems to draw, the program will generate EPS files in the executable's folder.

> **JSON format details:** See the [JSON Output Format](InputOutputFormat.md#json-output) section for the complete structure of JSON theorem files.

### Macros and Rules

The [Data folder](Source/Launchers/GeoGen.DrawingLauncher/Data) contains all MetaPost macros and custom [drawing rules](Source/Launchers/GeoGen.DrawingLauncher/Data/drawing_rules.txt).

### Tip for Quick Previewing on Windows

1. In the settings, set `PostcompilationCommand` to `file.bat`.
2. Create a file named `file.bat` in the executable's folder with the following content:

   ```batch
   @ECHO OFF

   SET /A starting_index = %1
   SET /A number_of_files = %2
   SET /A maximal_index = %starting_index% + %number_of_files% - 1

   FOR /L %%i IN (%starting_index% 1 %maximal_index%) DO "{SumatraPath}" "figures.%%i"

   EXIT 0
   ```

Replace `{SumatraPath}` with the path to the SumatraPDF executable. The usual location on Windows is `C:\Program Files\SumatraPDF\SumatraPDF.exe`. This ensures drawn figures are automatically opened for previewing.
