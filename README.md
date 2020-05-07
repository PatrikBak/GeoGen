# Automated generation of planar geometry olympiad problems (master's thesis)

**Author:** Bc. Patrik Bak  
**Supervisor:** doc. RNDr. Stanislav Krajči, PhD.  
**Consultant:** Mgr. Michal Rolínek, PhD.  

*P. J. Šafárik University in Košice  
Institute of Computer Science  
Faculty of Science*

## Table of Contents

- [About](#about)
- [Installation](#installation)
  * [Building](#building)
  * [Unit tests](#unit-tests)
- [Libraries](#libraries)
- [Usage](#usage)
  * [Main Launcher](#main-launcher)
  * [Drawing Launcher](#drawing-launcher)
    + [Requirements](#requirements)
    + [Running](#running)
    + [Tip for quick previewing on Windows](#tip-for-quick-previewing-on-windows)
    + [Results](#results)
  * [Theorem Proving Integration Test](#theorem-proving-integration-test)
  * [Helper modules](#helper-modules)
- [Contact](#contact)


## About

The goal is to make a program that will generate geometry problems suitable for mathematical contests.

A few results and lots of information about the functionality of the system can be found in our [paper](https://drive.google.com/open?id=1TXFSKmxR92eRzMYLvk7BuQHeFm4HXZzk).

## Installation

The system requires to have [.NET Core 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1) installed. 

### Building

The application can be built via `dotnet build` run in the [Source](Source) folder. 

### Unit tests

Automated tests can be run via `dotnet test` run in the [Source](Source) folder. 

## Libraries

All major logic is written from scratch, with a heavy use of LINQ to objects.

The following helper libraries have also been used:

 * [NInject](http://www.ninject.org/) - dependency injection container
 * [JSON.Net](https://www.newtonsoft.com/json) - JSON (de)serializer

For tests, we used:

* [NUnit](https://nunit.org/) - unit testing framework
* [FluentAssertions](https://fluentassertions.com/) - a cute way to write unit tests

## Usage

### Main Launcher

This is the main module that runs the main algorithm. The application tries to read the settings from the file `settings.json`. The example settings are available [here](Source/Launchers/GeoGen.MainLauncher/Examples/settings.json). As a start, you may copy this file to the folder with the executable. These settings are configured so that:

* The input files are loaded from the [input folder](Source/Launchers/GeoGen.MainLauncher/Examples/Inputs).
* The output is written to the following subdirectories of the [output folder](Source/Launchers/GeoGen.MainLauncher/Examples/Output):

  1. [**ReadableWithoutProofs**](Source/Launchers/GeoGen.MainLauncher/Examples/Output/ReadableWithoutProofs) - All found theorems that haven't been excluded.
  2. [**ReadableWithProofs**](Source/Launchers/GeoGen.MainLauncher/Examples/Output/ReadableWithProofs) - All found theorems, with reasons why some have been excluded.
  3. [**ReadableBestTheorems**](Source/Launchers/GeoGen.MainLauncher/Examples/Output/ReadableBestTheorems) - The best theorems for each type, ordered by their ranking.
  4. [**JsonOutput**](Source/Launchers/GeoGen.MainLauncher/Examples/Output/JsonOutput) -  All found theorems that haven't been excluded in the JSON format.
  5. [**JsonBestTheorems**](Source/Launchers/GeoGen.MainLauncher/Examples/Output/JsonBestTheorems) - Same as *ReadableBestTheorems*, but in the JSON format.

### Drawing Launcher

The visualizes results produced by the *Main Launcher* by generating [EPS](https://en.wikipedia.org/wiki/Encapsulated_PostScript) figures using [MetaPost](https://en.wikipedia.org/wiki/MetaPost).

#### Requirements

  * [MikTeX](https://miktex.org/download) (other distributions haven't been tested, but they will probably work too)
  * [OPmac](http://petr.olsak.net/opmac.html), used for quick font selection while printing problem texts

To preview generated results, I personally use

 * [SumatraPDF](https://www.sumatrapdfreader.org/free-pdf-reader.html)
 * [Ghostscript](https://ghostscript.com/) (this is required by Sumatra and does the actual rendering)

#### Running

The application tries to read the settings from the file `settings.json`. The example settings are available [here](Source/Launchers/GeoGen.DrawingLauncher/Examples/settings.json).

After running, you are requested to provide a path to a JSON theorem file. These files are produced by the *Main Launcher*. In our examples, the files from the folders [**JsonOutput**](Source/Launchers/GeoGen.MainLauncher/Examples/Output/JsonOutput) and [**JsonBestTheorems**](Source/Launchers/GeoGen.MainLauncher/Examples/Output/JsonBestTheorems). After selecting the theorems to be drawn, the program will generate eps files in the folder with the executable.

#### Tip for quick previewing on Windows

 1. In the settings, set `PostcompilationCommand` to `file.bat`
 2. In the folder with the executable create a file `file.bat` with the following content:

    ```
    @ECHO OFF
    
    SET /A starting_index = %1
    SET /A number_of_files = %2
    SET /A maximal_index = %starting_index% + %number_of_files% - 1
    
    FOR /L %%i IN (%starting_index% 1 %maximal_index%) DO "{SumatraPath}" "figures.%%i"
    
    EXIT 0
    ```

    `{SumatraPath}` should be replaced with the path to the SumatraPDF executable. The usual location on Windows is `C:\Program Files\SumatraPDF\SumatraPDF.exe`. This will ensure that drawn figures are automatically opened for previewing.


#### Results

A few results can be seen in our [paper](https://drive.google.com/open?id=1TXFSKmxR92eRzMYLvk7BuQHeFm4HXZzk), pages 20-24.

### Theorem Proving Integration Test

If you want to review the capabilities of the theorem prover, then run [this test](Source/Tests/IntegrationTests/GeoGen.TheoremProver.IntegrationTest/Program.cs). The `main` method requires two arguments:

1. The path to the folder with inference rules. 
2. The path to the folder with object introduction rules. 

The default values that should work after cloning can be found in [launchSettings.json](Source/Tests/IntegrationTests/GeoGen.TheoremProver.IntegrationTest/Properties/launchSettings.json).

### Helper modules

The following modules were used to prepare the experiment described in our [paper](https://drive.google.com/open?id=1TXFSKmxR92eRzMYLvk7BuQHeFm4HXZzk).

* **Configuration Generation Launcher** - this module was used to test the counts of generated configurations for certain input files (and memory usage). It can be used similarly as the *Main Launcher*, with the default settings available [here](Source/Launchers/GeoGen.ConfigurationGenerationLauncher/Examples/settings.json), which needs to be copied to the folder with the executable. The output is not saved, only printed into the console.
* **Input Generation Launcher** - this module generated input files. Running it would generate around 4500 small files, totaling 4MB.
* **Output Merging Launcher** - this module merged the final result. The `main` method requires one argument, the path to the folder with JSON outputs (the folder is scanned recursively).

## Contact

For more information you can contact me via [email](mailto:patrik.bak.x@gmail.com).