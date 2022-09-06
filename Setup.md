# Installation

The core programming language of the software is C#. The program requires to have [.NET 5.0](https://dotnet.microsoft.com/download/dotnet/5.0) installed. 

The application can be built via `dotnet build` run in the [Source](Source) folder. 

Automated tests can be run via `dotnet test` run in the [Source](Source) folder. 

# Usage

## Main Launcher

This is the main module that runs the main algorithm. The application tries to read the settings from the file `settings.json`. The example settings are available [here](Source/Launchers/GeoGen.MainLauncher/Examples/settings.json). As a start, you may copy this file to the folder with the executable. These settings are configured so that:

* The input files are loaded from the [input folder](Source/Launchers/GeoGen.MainLauncher/Examples/Inputs).
* The output is written to the following subdirectories of the [output folder](Source/Launchers/GeoGen.MainLauncher/Examples/Output):

  1. [**ReadableWithoutProofs**](Source/Launchers/GeoGen.MainLauncher/Examples/Output/ReadableWithoutProofs) - All found theorems that haven't been excluded.
  2. [**ReadableWithProofs**](Source/Launchers/GeoGen.MainLauncher/Examples/Output/ReadableWithProofs) - All found theorems, with reasons why some have been excluded.
  3. [**ReadableBestTheorems**](Source/Launchers/GeoGen.MainLauncher/Examples/Output/ReadableBestTheorems) - The best theorems for each type, ordered by their ranking.
  4. [**JsonOutput**](Source/Launchers/GeoGen.MainLauncher/Examples/Output/JsonOutput) -  All found theorems that haven't been excluded in the JSON format.
  5. [**JsonBestTheorems**](Source/Launchers/GeoGen.MainLauncher/Examples/Output/JsonBestTheorems) - Same as *ReadableBestTheorems*, but in the JSON format.

## Constructions

The following constructions are supported in input files (and object introduction rules / inference rules / drawing rules...):

* `CircleWithCenterThroughPoint` - takes two points `A`, `B` and outputs the circle with the center `A` and a radius `AB`
* `CircleWithDiameter` - takes two points `A`, `B` and outputs the circle with a diameter `AB`.
* `Circumcenter` - takes three points `A`, `B`, `C` and outputs the circumcenter of triangle `ABC`.
* `Circumcircle` - takes three points `A`, `B`, `C` and outputs the circumcircle of triangle `ABC`.
* `Excenter` - takes three points `A`, `B`, `C` and outputs the `A`-excenter of triangle `ABC`.
* `Excircle` - takes three points `A`, `B`, `C` and outputs the `A`-excircle of triangle `ABC`.
* `ExternalAngleBisector` - takes three points `A`, `B`, `C` and outputs the external angle bisector of angle `BAC`.
* `Incenter` - takes three points `A`, `B`, `C` and outputs the incenter of triangle `ABC`.
* `Incircle` - takes three points `A`, `B`, `C` and outputs the incircle of triangle `ABC`.
* `InternalAngleBisector` - takes three points `A`, `B`, `C` and outputs the internal angle bisector of angle `BAC`.
* `IntersectionOfLineAndLineFromPoints` - takes a line `l` and two points `A`, `B` and outputs the intersection point of lines `l` and `AB`.
* `IntersectionOfLines` - takes two lines and outputs their intersection points.
* `IntersectionOfLinesFromPoints` - takes four points `A`, `B`, `C`, `D` and outputs the intersection point of lines `AB` and `CD`.
* `IsoscelesTrapezoidPoint` - takes three points `A`, `B`, `C` and outputs such a point `D` that `ABCD` is an isosceles trapezoid.
* `LineFromPoints` - takes two points and outputs the line passing through both of them.
* `LineThroughCircumcenter` - takes three points `A`, `B`, `C` and outputs the line through `A` and the circumcenter of `ABC`.
* `Median` - takes three points `A`, `B`, `C` and outputs the `A`-median of triangle `ABC`.
* `Midline` - takes three points `A`, `B`, `C` and outputs the `A`-midline of triangle `ABC`.
* `Midpoint` - takes two points `A`, `B` and output the midpoint of segment `AB`.
* `MidpointOfArc` - takes three points `A`, `B`, `C` and outputs the midpoint of arc `BAC`.
* `MidpointOfOppositeArc` - takes three points `A`, `B`, `C` and outputs the midpoint of arc `BAC` not containing `A`.
* `NinePointCircle` - takes three points `A`, `B`, `C` and outputs the Nine-Point circle of triangle `ABC`.
* `OppositePointOnCircumcircle` - takes three points `A`, `B`, `C` and outputs such a point `D` that `AD` is a diameter of the circumcircle of `ABC`.
* `Orthocenter` - takes three points `A`, `B`, `C` and outputs the orthocenter of triangle `ABC`.
* `ParallelLine` - takes a point `A` and a line `l` and outputs the line through `A` parallel to `l`.
* `ParallelLineToLineFromPoints` - takes three points `A`, `B`, `C` and outputs the line through `A` parallel to `BC`.
* `ParallelogramPoint` - takes three points `A`, `B`, `C` and outputs such a point `D` that `ABDC` is a parallelogram.
* `PerpendicularBisector` - takes two points `A`, `B` and outputs the perpendicular bisector of segment `AB`.
* `PerpendicularLine` - takes a point `P` and a line `l` and outputs the line through `P` perpendicular to `l`.
* `PerpendicularLineAtPointOfLine` - takes two points `A`, `B` and outputs the line through `A` perpendicular to `AB`.
* `PerpendicularLineToLineFromPoints` - takes three points `A`, `B`, `C` and outputs the perpendicular line through `A` to `BC`.
* `PerpendicularProjection` - takes a point `P` and a line `l` and outputs the projection of `P` onto `l`.
* `PerpendicularProjectionOnLineFromPoints` - takes three points `A`, `B`, `C` and outputs the projection of `A` onto `BC`.
* `PointReflection` - takes two points `A`, `B` and outputs the reflection of `A` in `B`.
* `ReflectionInLine` - takes a line `l` and a point `P` and outputs the reflection of `A` in `l`.
* `ReflectionInLineFromPoints` - takes three points `A`, `B`, `C` and outputs the reflection of `A` in line `BC`.
* `SecondIntersectionOfCircleAndLineFromPoints` - takes four points `A`, `C`, `C`, `D` and outputs the second intersection point of line `AB` and the circumcircle of `ACD`.
* `SecondIntersectionOfTwoCircumcircles` - takes five points `A`, `B`, `C`, `D`, `E` and outputs the second intersection point of the circumcircles `ABC` and `ADE`.
* `TangentLine` - takes three points `A`, `B`, `C` and outputs the tangent line to the circumcircle of `ABC` at `A`.

## Inference rules

The used inference rules can be found [here](Source/Library/GeoGen.TheoremProver.InferenceRuleProvider/Rules). They use constructions described [here](#constructions).

## Object introduction rules

The used object introduction rules can be found [here](Source/Library/GeoGen.TheoremProver.ObjectIntroductionRuleProvider/Rules/object_introduction_rules.txt). They use constructions described [here](#constructions).

## Drawing Launcher

Provides visualization of the results produced by *Main Launcher* via generating [EPS](https://en.wikipedia.org/wiki/Encapsulated_PostScript) figures using [MetaPost](https://en.wikipedia.org/wiki/MetaPost).

### Requirements

  * [MikTeX](https://miktex.org/download) (other distributions haven't been tested, but they will probably work too)
  * [OPmac](http://petr.olsak.net/opmac.html), used for quick font selection while printing problem texts

To preview generated results, I personally use

 * [SumatraPDF](https://www.sumatrapdfreader.org/free-pdf-reader.html)
 * [Ghostscript](https://ghostscript.com/) (this is required by Sumatra and does the actual rendering)

### Running

The application tries to read the settings from the file `settings.json`. The example settings are available [here](Source/Launchers/GeoGen.DrawingLauncher/Examples/settings.json).

After running, you are requested to provide a path to a JSON theorem file. These files are produced by the *Main Launcher*. In our examples, the files from the folders [**JsonOutput**](Source/Launchers/GeoGen.MainLauncher/Examples/Output/JsonOutput) and [**JsonBestTheorems**](Source/Launchers/GeoGen.MainLauncher/Examples/Output/JsonBestTheorems). After selecting the theorems to be drawn, the program will generate eps files in the folder with the executable.

### Macros and rules

Note the following [folder](Source/Launchers/GeoGen.DrawingLauncher/Data), which contains all written MetaPost macros and custom [drawing rules](Source/Launchers/GeoGen.DrawingLauncher/Data/drawing_rules.txt). 

### Tip for quick previewing on Windows

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

## Theorem Proving Integration Test

If you want to review the capabilities of the theorem prover, then run [this test](Source/Tests/IntegrationTests/GeoGen.TheoremProver.IntegrationTest/Program.cs). The `main` method requires two arguments:

1. The path to the folder with inference rules. 
2. The path to the folder with object introduction rules. 

The default values that should work after cloning can be found in [launchSettings.json](Source/Tests/IntegrationTests/GeoGen.TheoremProver.IntegrationTest/Properties/launchSettings.json).

## Helper modules

The following modules were used to prepare the large-scale experiments described in the [thesis](https://drive.google.com/file/d/1dsaxDCMzlAPfB3e4rd8ut2RuZ_sn2Zm5/view?usp=sharing).

* **Configuration Generation Launcher** - this module was used to test the counts of generated configurations for certain input files (and memory usage). It can be used similarly as the *Main Launcher*, with the default settings available [here](Source/Launchers/GeoGen.ConfigurationGenerationLauncher/Examples/settings.json), which needs to be copied to the folder with the executable. The output is not saved, only printed into the console.
* **Input Generation Launcher** - this module generated input files. Running it would generate around 4500 small files, totaling 4MB.
* **Output Merging Launcher** - this module merged the final result. The `main` method requires one argument, the path to the folder with JSON outputs (the folder is scanned recursively).
