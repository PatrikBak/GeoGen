# Input/Output File Format Reference

This document provides a complete technical reference for input and output file formats. For usage instructions, see the [User Guide](UserGuide.md).

## Input File Format

The input file is a plain text file that specifies the initial configuration, constructions to use, and parameters for the generation process. It is divided into the following sections:

### `Constructions:`

List of constructions that can be used to generate new objects. Each construction name should be on a separate line.

**Available constructions:**

> **Note:** These constructions are also used in inference rules, object introduction rules, and drawing rules.

- `Centroid` - takes three points `A`, `B`, `C` and outputs the centroid of triangle `ABC`
- `CircleWithCenterThroughPoint` - takes two points `A`, `B` and outputs the circle with center `A` and radius `AB`
- `CircleWithDiameter` - takes two points `A`, `B` and outputs the circle with diameter `AB`
- `Circumcenter` - takes three points `A`, `B`, `C` and outputs the circumcenter of triangle `ABC`
- `Circumcircle` - takes three points `A`, `B`, `C` and outputs the circumcircle of triangle `ABC`
- `Excenter` - takes three points `A`, `B`, `C` and outputs the `A`-excenter of triangle `ABC`
- `Excircle` - takes three points `A`, `B`, `C` and outputs the `A`-excircle of triangle `ABC`
- `ExternalAngleBisector` - takes three points `A`, `B`, `C` and outputs the external angle bisector of angle `BAC`
- `Incenter` - takes three points `A`, `B`, `C` and outputs the incenter of triangle `ABC`
- `Incircle` - takes three points `A`, `B`, `C` and outputs the incircle of triangle `ABC`
- `InternalAngleBisector` - takes three points `A`, `B`, `C` and outputs the internal angle bisector of angle `BAC`
- `IntersectionOfLineAndLineFromPoints` - takes a line `l` and two points `A`, `B` and outputs the intersection point of lines `l` and `AB`
- `IntersectionOfLines` - takes two lines and outputs their intersection point
- `IntersectionOfLinesFromPoints` - takes four points `A`, `B`, `C`, `D` and outputs the intersection point of lines `AB` and `CD`
- `IsoscelesTrapezoidPoint` - takes three points `A`, `B`, `C` and outputs a point `D` such that `ABCD` is an isosceles trapezoid
- `LineFromPoints` - takes two points and outputs the line passing through both of them
- `LineThroughCircumcenter` - takes three points `A`, `B`, `C` and outputs the line through `A` and the circumcenter of `ABC`
- `Median` - takes three points `A`, `B`, `C` and outputs the `A`-median of triangle `ABC`
- `Midline` - takes three points `A`, `B`, `C` and outputs the `A`-midline of triangle `ABC`
- `Midpoint` - takes two points `A`, `B` and outputs the midpoint of segment `AB`
- `MidpointOfArc` - takes three points `A`, `B`, `C` and outputs the midpoint of arc `BAC`
- `MidpointOfOppositeArc` - takes three points `A`, `B`, `C` and outputs the midpoint of arc `BAC` not containing `A`
- `NinePointCircle` - takes three points `A`, `B`, `C` and outputs the Nine-Point circle of triangle `ABC`
- `OppositePointOnCircumcircle` - takes three points `A`, `B`, `C` and outputs a point `D` such that `AD` is a diameter of the circumcircle of `ABC`
- `Orthocenter` - takes three points `A`, `B`, `C` and outputs the orthocenter of triangle `ABC`
- `ParallelLine` - takes a point `A` and a line `l` and outputs the line through `A` parallel to `l`
- `ParallelLineToLineFromPoints` - takes three points `A`, `B`, `C` and outputs the line through `A` parallel to `BC`
- `ParallelogramPoint` - takes three points `A`, `B`, `C` and outputs a point `D` such that `ABDC` is a parallelogram
- `PerpendicularBisector` - takes two points `A`, `B` and outputs the perpendicular bisector of segment `AB`
- `PerpendicularLine` - takes a point `P` and a line `l` and outputs the line through `P` perpendicular to `l`
- `PerpendicularLineAtPointOfLine` - takes two points `A`, `B` and outputs the line through `A` perpendicular to `AB`
- `PerpendicularLineToLineFromPoints` - takes three points `A`, `B`, `C` and outputs the perpendicular line through `A` to `BC`
- `PerpendicularProjection` - takes a point `P` and a line `l` and outputs the projection of `P` onto `l`
- `PerpendicularProjectionOnLineFromPoints` - takes three points `A`, `B`, `C` and outputs the projection of `A` onto `BC`
- `PointReflection` - takes two points `A`, `B` and outputs the reflection of `A` in `B`
- `ReflectionInLine` - takes a line `l` and a point `P` and outputs the reflection of `P` in `l`
- `ReflectionInLineFromPoints` - takes three points `A`, `B`, `C` and outputs the reflection of `A` in line `BC`
- `SecondIntersectionOfCircleAndLineFromPoints` - takes four points `A`, `B`, `C`, `D` and outputs the second intersection point of line `AB` and the circumcircle of `ACD`
- `SecondIntersectionOfTwoCircumcircles` - takes five points `A`, `B`, `C`, `D`, `E` and outputs the second intersection point of the circumcircles `ABC` and `ADE`
- `TangentLine` - takes three points `A`, `B`, `C` and outputs the tangent line to the circumcircle of `ABC` at `A`

### `Initial configuration:`

Defines the starting geometry objects.

- First line: The base object type followed by the names of its points (e.g., `Triangle: A, B, C`).
- Subsequent lines: Definitions of additional initial objects using constructions (e.g., `D = Incenter(A, B, C)`).

**Available base object types:**

- `LineSegment` - two points (e.g., `LineSegment: A, B`)
- `Triangle` - three non-collinear points (e.g., `Triangle: A, B, C`)
- `RightTriangle` - three points where angle at first point is right (e.g., `RightTriangle: A, B, C`)
- `Quadrilateral` - four points, no three collinear (e.g., `Quadrilateral: A, B, C, D`)
- `CyclicQuadrilateral` - four points on a circle, no three collinear (e.g., `CyclicQuadrilateral: A, B, C, D`)
- `LineAndPoint` - a line and a point not on it (e.g., `LineAndPoint: l, A`)
- `LineAndTwoPoints` - a line and two distinct points not on it (e.g., `LineAndTwoPoints: l, A, B`)

### `Iterations:`

The number of steps the generator should take. In each step, it tries to apply available constructions to existing objects.

### `MaximalPoints:`, `MaximalLines:`, `MaximalCircles:`

Constraints on the number of objects of each type that can be added during generation.

### `SymmetryGenerationMode:`

Controls how symmetric configurations are handled. Possible values:

- `GenerateOnlySymmetric` - generates only symmetric configurations (configurations that remain unchanged under at least one symmetry mapping)
- `GenerateOnlyFullySymmetric` - generates only fully symmetric configurations (configurations that remain unchanged under all symmetry mappings)
- `GenerateBothSymmetricAndAsymmetric` - generates both symmetric and asymmetric configurations (default)

## Example Input File

```text
Constructions:

 IntersectionOfLinesFromPoints
 Median

Initial configuration:

 Triangle: A, B, C
 D = Incenter(A, B, C)
 E = PerpendicularProjectionOnLineFromPoints(D, B, C)

Iterations: 1
MaximalPoints: 1
MaximalLines: 1
MaximalCircles: 0
SymmetryGenerationMode: GenerateBothSymmetricAndAsymmetric
```

**Example input files:**

- [input_incenter.txt](Source/Launchers/GeoGen.MainLauncher/Examples/Inputs/input_incenter.txt)
- [input_projections.txt](Source/Launchers/GeoGen.MainLauncher/Examples/Inputs/input_projections.txt)

## Output File Format

The application produces several types of output files in the configured output directory. See the [User Guide](UserGuide.md#main-launcher) for information about where these files are written and how to use them.

## Readable Output (Text)

Files in `ReadableWithoutProofs` and `ReadableWithProofs` folders are human-readable text files.

**Example readable output files:**

- [output_incenter.txt](Source/Launchers/GeoGen.MainLauncher/Examples/Output/ReadableWithoutProofs/output_incenter.txt) (without proofs)
- [output_incenter.txt](Source/Launchers/GeoGen.MainLauncher/Examples/Output/ReadableWithProofs/output_incenter.txt) (with proofs)
- [output_projections.txt](Source/Launchers/GeoGen.MainLauncher/Examples/Output/ReadableWithoutProofs/output_projections.txt) (without proofs)
- [output_projections.txt](Source/Launchers/GeoGen.MainLauncher/Examples/Output/ReadableWithProofs/output_projections.txt) (with proofs)
- [CollinearPoints.txt](Source/Launchers/GeoGen.MainLauncher/Examples/Output/ReadableBestTheorems/CollinearPoints.txt) (best theorems)
- [ConcyclicPoints.txt](Source/Launchers/GeoGen.MainLauncher/Examples/Output/ReadableBestTheorems/ConcyclicPoints.txt) (best theorems)

### Structure:

1.  **Constructions**: Lists the constructions used.
2.  **Initial configuration**: Shows the starting objects.
3.  **Theorems**: Lists the discovered geometric theorems.
    - Format: `Type: Object1, Object2, ...` (e.g., `ParallelLines: [A, B], [C, D]` or `CollinearPoints: A, D, G`).
    - In `ReadableWithProofs`, each theorem is followed by a "proof" or reason why it was not excluded (often internal prover details).

**Available theorem types:**

- `CollinearPoints` - three or more points are collinear
- `ConcyclicPoints` - four or more points are concyclic
- `ConcurrentLines` - three lines are concurrent in a point that is not in the picture
- `ParallelLines` - two lines are parallel to each other
- `PerpendicularLines` - two lines are perpendicular to each other
- `TangentCircles` - two circles are tangent to each other
- `LineTangentToCircle` - a line is tangent to a circle
- `EqualLineSegments` - two line segments have equal lengths
- `EqualObjects` - two objects with formally different definitions represent the same geometric object
- `Incidence` - a point lies on a line or circle

## JSON Output

Files in `JsonOutput` and `JsonBestTheorems` folders contain structured data suitable for programmatic processing.

**Example JSON output files:**

- [output_incenter.json](Source/Launchers/GeoGen.MainLauncher/Examples/Output/JsonOutput/output_incenter.json)
- [output_projections.json](Source/Launchers/GeoGen.MainLauncher/Examples/Output/JsonOutput/output_projections.json)
- [CollinearPoints.json](Source/Launchers/GeoGen.MainLauncher/Examples/Output/JsonBestTheorems/CollinearPoints.json) (best theorems)
- [ConcyclicPoints.json](Source/Launchers/GeoGen.MainLauncher/Examples/Output/JsonBestTheorems/ConcyclicPoints.json) (best theorems)

### Structure (JSON):

The JSON output is an array of theorem objects. Each theorem object has the following structure:

```json
[
  {
    "TheoremString": "string",
    "Ranking": {
      "Rankings": {
        "Symmetry": {
          "Ranking": number,
          "Weight": number,
          "Contribution": number
        },
        "Level": {
          "Ranking": number,
          "Weight": number,
          "Contribution": number
        },
        "NumberOfCyclicQuadrilaterals": {
          "Ranking": number,
          "Weight": number,
          "Contribution": number
        },
        "NumberOfTheorems": {
          "Ranking": number,
          "Weight": number,
          "Contribution": number
        }
      },
      "TotalRanking": number
    },
    "ConfigurationString": "string"
  }
]
```

**Fields:**

- `TheoremString` - String representation of the theorem (format: `TheoremType: Object1, Object2, ...`)
- `Ranking` - Heuristic ranking with component scores
  - `Rankings` - Individual ranking components:
    - `Symmetry` - Symmetry score (typically 0.0 to 1.0)
    - `Level` - Level score (typically 0.0 to 1.0)
    - `NumberOfCyclicQuadrilaterals` - Count of cyclic quadrilaterals
    - `NumberOfTheorems` - Total number of theorems
    - Each component has `Ranking` (the score), `Weight` (multiplier), and `Contribution` (Ranking × Weight)
  - `TotalRanking` - Final weighted sum of all ranking components
- `ConfigurationString` - String representation of the configuration where the theorem holds (uses `\n` for line breaks)

**Note:** Both `JsonOutput` and `JsonBestTheorems` files contain the same structure. The exact structure matches the internal C# `RankedTheoremIntermediate` class.
