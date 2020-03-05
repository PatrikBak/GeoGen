using GeoGen.AnalyticGeometry;
using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Infrastructure;
using GeoGen.TheoremRanker;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GeoGen.Infrastructure.Log;

namespace GeoGen.DrawingLauncher
{
    /// <summary>
    /// Represents a drawer that generates MetaPost figures.
    /// </summary>
    public class MetapostDrawer : IDrawer
    {
        #region Private fields

        /// <summary>
        /// The settings for the drawer.
        /// </summary>
        private readonly MetapostDrawerSettings _settings;

        /// <summary>
        /// The data for the drawer.
        /// </summary>
        private readonly MetapostDrawerData _data;

        #endregion

        #region Dependencies

        /// <summary>
        /// The constructor that calculates coordinates for us.
        /// </summary>
        private readonly IGeometryConstructor _constructor;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MetapostDrawer"/> class.
        /// </summary>
        /// <param name="data">The data for the drawer.</param>
        /// <param name="settings">The settings for the drawer.</param>
        /// <param name="constructor">The constructor that calculates coordinates for us.</param>
        public MetapostDrawer(MetapostDrawerSettings settings, MetapostDrawerData data, IGeometryConstructor constructor)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _data = data ?? throw new ArgumentNullException(nameof(data));
            _constructor = constructor ?? throw new ArgumentNullException(nameof(constructor));
        }

        #endregion

        #region IDrawer implementation

        /// <inheritdoc/>
        public async Task DrawAsync(IEnumerable<RankedTheorem> rankedTheorems, int startingId)
        {
            // Create figures from configuration-theorem pair
            var figures = rankedTheorems.Select((theorem, index) => CreateFigure(theorem, index + startingId)).ToArray();

            #region Writing code file

            // Get the code for them 
            var code = CreateCode(figures, startingId);

            try
            {
                // Create the file that will be compiled
                await File.WriteAllTextAsync(_settings.MetapostCodeFilePath, code);
            }
            catch (Exception e)
            {
                // Throw further
                throw new DrawingLauncherException($"Couldn't write the MetaPost code to the path {_settings.MetapostCodeFilePath}", e);
            }

            #endregion

            #region Compiling

            // Construct the command with parameters
            var command = $"{_settings.CompilationCommand.program} \"{_settings.MetapostCodeFilePath}\"";

            // Run the compilation command
            var (exitCode, output, errors) = await ProcessUtilities.RunCommandAsync(_settings.CompilationCommand.program,
                // With the appended file path at the end
                arguments: $"{_settings.CompilationCommand.arguments} \"{_settings.MetapostCodeFilePath}\"");

            // If the error code is not OK, i.e. not zero, make aware
            if (exitCode != 0)
                throw new CommandException($"The compilation of the MetaPost code file '{_settings.MetapostCodeFilePath}' failed.", command, exitCode, output, errors);

            #endregion

            #region Post-compilation command

            // If there was no error, we run post-compilation command, if it's there
            if (_settings.PostcompilationCommand != null)
            {
                // Construct the command with parameters
                command = $"{_settings.PostcompilationCommand} {figures.Length}";

                // Run it with two arguments, the starting picture id and the number of pictures
                (exitCode, output, errors) = await ProcessUtilities.RunCommandAsync(_settings.PostcompilationCommand, arguments: $"{startingId} {figures.Length}");

                // If the error code is not OK, i.e. not zero, make aware
                if (exitCode != 0)
                    throw new CommandException($"The execution of the post-compilation command failed.", command, exitCode, output, errors);
            }

            #endregion
        }

        /// <summary>
        /// Constructs a figure for a given ranked theorem.
        /// </summary>
        /// <param name="rankedTheorem">The ranked theorem for which we're drawing a figure.</param>
        /// <param name="id">The id of the figure.</param>
        /// <returns>The MetaPost figure.</returns>
        private MetapostFigure CreateFigure(RankedTheorem rankedTheorem, int id)
        {
            // Safely execute
            var (pictures, constructionData) = GeneralUtilities.TryExecute(
                // Constructing the configuration
                () => _constructor.Construct(rankedTheorem.Configuration, _settings.NumberOfPictures, LooseObjectDrawingStyle.Standard),
                // Make sure a potential exception is caught and re-thrown
                (InconsistentPicturesException e) => throw new ConstructionException("Drawing of the initial configuration failed.", e));

            // Make sure there is no inconstructible object
            if (constructionData.InconstructibleObject != default)
                throw new ConstructionException("The configuration cannot be constructed, because it contains an inconstructible object.");

            // Make sure there are no duplicates
            if (constructionData.Duplicates != default)
                throw new ConstructionException("The configuration cannot be constructed, because it contains duplicate objects");

            // Construct the ranked figures by going through the pictures
            var rankedFigures = pictures.Select(picture =>
                {
                    try
                    {
                        // Try to construct the figure
                        var figure = ConstructFigure(rankedTheorem, picture);

                        // Rank it
                        var rank = figure.CalculateVisualBadness();

                        // Return the tuple
                        return (figure, rank);
                    }
                    catch (Exception e)
                    {
                        // Make aware if there is a weird problem
                        LoggingManager.LogWarning($"A problem with picture number {id}. The message: {e.Message}\n");

                        // Log the exception as a debug message
                        LoggingManager.LogDebug(e.ToString());

                        // Return the default value indicating something didn't work
                        return default;
                    }
                })
                // Take those where it worked out
                .Where(pair => pair != default)
                // Enumerate
                .ToArray();

            // If there are no figures, make aware
            if (rankedFigures.IsEmpty())
                throw new ConstructionException("No successfully drawn figure of the configuration.");

            // Otherwise find the figure with the smallest ranking, i.e. the most beautiful one
            var (figure, rank) = rankedFigures.MinItem(item => item.rank);

            // If the rank is maximal, i.e. ultimately bad figure, make aware
            if (rank == double.MaxValue)
                LoggingManager.LogWarning($"Figure {id} couldn't be drawn nicely.\n");

            // Return the figure anyway
            return figure;
        }

        /// <summary>
        /// Performs the construction of a MetaPost figure holding the passed theorem and its configuration
        /// with respect to the passed picture containing needed analytic objects.
        /// </summary>
        /// <param name="rankedTheorem">The ranked theorem for which we're drawing a figure.</param>
        /// <param name="picture">The picture with analytic representations of the objects.</param>
        /// <returns>The constructed MetaPost figure.</returns>
        private MetapostFigure ConstructFigure(RankedTheorem rankedTheorem, Picture picture)
        {
            // Get the configuration and theorem for comfort
            var configuration = rankedTheorem.Configuration;
            var theorem = rankedTheorem.Theorem;

            // Create an empty figure
            var figure = new MetapostFigure();

            #region Loose object layout construction

            // Get the loose objects
            var looseObjects = configuration.LooseObjects.Select(picture.Get).ToArray();

            // Switch based on the loose objects layout
            switch (configuration.LooseObjectsHolder.Layout)
            {
                // Triangle case
                case LooseObjectLayout.Triangle:

                    // In this case we have three points
                    var points = looseObjects.Cast<Point>().ToArray();

                    // We want to mark each as a normal object
                    points.ForEach(point => figure.AddPoint(point, ObjectDrawingStyle.NormalObject));

                    // And also draw the actual triangle
                    points.UnorderedPairs().ForEach(pair => figure.AddSegment(pair.Item1, pair.Item2, ObjectDrawingStyle.NormalObject, shifted: false));

                    break;

                // Unhandled cases
                default:
                    throw new DrawingLauncherException($"Unhandled value of {nameof(LooseObjectLayout)}: {configuration.LooseObjectsHolder.Layout}");
            }

            #endregion

            #region Constructed object construction

            // Add each constructed object
            configuration.ConstructedObjects.ForEach(constructedObject =>
            {
                // Make sure it has a rule
                var rule = _data.DrawingRules.GetValueOrDefault(constructedObject.Construction)
                    // If not, make aware
                    ?? throw new DrawingLauncherException($"Cannot draw the picture, because construction {constructedObject.Construction} has no drawing rule.");

                // Remap the template objects
                var objectMap = rule.ObjectToDraw.PassedArguments.FlattenedList
                    // To the actual objects
                    .ZipToDictionary(constructedObject.PassedArguments.FlattenedList);

                // Add the actual object to the mapping
                objectMap.Add(rule.ObjectToDraw, constructedObject);

                // Add the auxiliary objects too
                rule.AuxiliaryObjects.ForEach(auxiliaryObject =>
                {
                    // Remap the object by taking the same construction
                    var remmapedObject = new ConstructedConfigurationObject(auxiliaryObject.Construction,
                        // And remapping its arguments
                        auxiliaryObject.PassedArguments.FlattenedList.Select(argument => objectMap[argument]).ToArray());

                    // It might be used in other constructions, so we need to put it into the map
                    objectMap.Add(auxiliaryObject, remmapedObject);

                    // If the picture already contains the object, don't carry out the construction
                    if (picture.Contains(remmapedObject))
                        return;

                    // The remapped object now has the real object as its arguments, so we can construct it
                    var analyticObject = _constructor.Construct(picture, remmapedObject, addToPicture: true);

                    // If the object cannot be constructed, make aware
                    if (analyticObject == null)
                        throw new ConstructionException($"A drawing command of the drawing rule for {rule.ObjectToDraw.Construction} contains an inconstructible object.");
                });

                // Perform the individual drawing commands
                rule.DrawingCommands.ForEach(command =>
                {
                    // Find the remapped versions of objects being drawn
                    var objectsBeingDrawn = command.Arguments.Select(configurationObject =>
                            // Object has to be already mapped
                            objectMap.GetValueOrDefault(configurationObject)
                                // Otherwise we have a huge problem...
                                ?? throw new DrawingLauncherException($"A drawing command of the drawing rule for {rule.ObjectToDraw.Construction} contains an undefined object."))
                        // Or even their analytic version
                        .Select(configurationObject => picture.Get(configurationObject))
                        // Enumerate
                        .ToArray();

                    // Switch based on the type of command
                    switch (command.Type)
                    {
                        // If marking a point
                        case DrawingCommandType.Point:

                            // Then there is exactly one object of type point that we easily mark
                            figure.AddPoint((Point)objectsBeingDrawn[0], command.Style);

                            break;

                        // If drawing a segment
                        case DrawingCommandType.Segment:

                            // Then there are two points
                            figure.AddSegment((Point)objectsBeingDrawn[0], (Point)objectsBeingDrawn[1], command.Style, shifted: false);

                            break;

                        // If drawing a shifted segment 
                        case DrawingCommandType.ShiftedSegment:

                            // Then there are two points
                            figure.AddSegment((Point)objectsBeingDrawn[0], (Point)objectsBeingDrawn[1], command.Style, shifted: true);

                            break;

                        // If drawing a line that might contain points
                        case DrawingCommandType.Line:

                            // Then there is a line and potentially some other points
                            figure.AddLine((Line)objectsBeingDrawn[0], objectsBeingDrawn.Skip(1).Cast<Point>().ToArray(), command.Style, shifted: false);

                            break;

                        // If drawing a shifted line that should contain at least one point
                        case DrawingCommandType.ShiftedLine:

                            // Then there is a line and potentially some other points (at least one)
                            figure.AddLine((Line)objectsBeingDrawn[0], objectsBeingDrawn.Skip(1).Cast<Point>().ToArray(), command.Style, shifted: true);

                            break;

                        // If drawing a circle
                        case DrawingCommandType.Circle:

                            // Then there is just a circle
                            figure.AddCircle((Circle)objectsBeingDrawn[0], command.Style);

                            break;

                        // Unhandled cases
                        default:
                            throw new DrawingLauncherException($"Unhandled type of {nameof(DrawingCommandType)}: {command.Type}");
                    }
                });
            });

            #endregion

            #region Theorem object construction

            // Helper function that draws a line
            Line ConstructLine(LineTheoremObject line) =>
                // If the line is specified explicitly...
                line.DefinedByExplicitObject ?
                    // Then we just pull the analytic version of it from the picture
                    (Line)picture.Get(line.ConfigurationObject) :
                    // Otherwise we construct it from its points
                    new Line((Point)picture.Get(line.PointsList[0]), (Point)picture.Get(line.PointsList[1]));

            // Helper function that constructs a circle
            Circle ConstructCircle(CircleTheoremObject circle) =>
                // If the line is specified explicitly...
                circle.DefinedByExplicitObject ?
                    // Then we just pull the analytic version of it from the picture
                    (Circle)picture.Get(circle.ConfigurationObject) :
                    // Otherwise we construct it from its points
                    new Circle((Point)picture.Get(circle.PointsList[0]), (Point)picture.Get(circle.PointsList[1]), (Point)picture.Get(circle.PointsList[2]));

            // Helper function adds a given theorem line to the picture, together with the points it should pass through
            // It can be specified whether the line should be drawn as a shifted one, or a regular one
            void DrawTheoremLine(LineTheoremObject line, bool shifted, params Point[] passingPoints)
            {
                // If the line is defined explicitly...
                if (line.DefinedByExplicitObject)
                {
                    // Then we only know about the passed points passing through it
                    figure.AddLine(ConstructLine(line), passingPoints, ObjectDrawingStyle.TheoremObject, shifted);

                    // And we're done
                    return;
                }

                // Otherwise we want to say that the line passes through its defining points
                var finalPassingPoints = line.Points.Select(picture.Get).Cast<Point>()
                    // As well as through the passed passing points
                    .Concat(passingPoints)
                    // Enumerate
                    .ToArray();

                // Finally draw it
                figure.AddLine(ConstructLine(line), finalPassingPoints, ObjectDrawingStyle.TheoremObject, shifted);
            }

            // Switch based on theorem type
            switch (theorem.Type)
            {
                // With collinear points
                case TheoremType.CollinearPoints:
                {
                    // We get the points
                    var points = theorem.InvolvedObjects.Cast<PointTheoremObject>()
                        // And their analytic versions
                        .Select(theoremObject => (Point)picture.Get(theoremObject.ConfigurationObject))
                        // Enumerate
                        .ToArray();

                    // We mark all of them
                    points.ForEach(point => figure.AddPoint(point, ObjectDrawingStyle.TheoremObject));

                    // Construct the line they make
                    var line = new Line(points[0], points[1]);

                    // Make sure this one is drawn too, containing all the points
                    figure.AddLine(line, points, ObjectDrawingStyle.TheoremObject, shifted: false);

                    break;
                }

                // With concyclic points
                case TheoremType.ConcyclicPoints:
                {
                    // We get the points
                    var points = theorem.InvolvedObjects.Cast<PointTheoremObject>()
                        // And their analytic versions
                        .Select(theoremObject => (Point)picture.Get(theoremObject.ConfigurationObject))
                        // Enumerate
                        .ToArray();

                    // We mark all of them
                    points.ForEach(point => figure.AddPoint(point, ObjectDrawingStyle.TheoremObject));

                    // Construct the circle they make
                    var circle = new Circle(points[0], points[1], points[2]);

                    // Make sure this one is drawn too, containing all the points
                    figure.AddCircle(circle, ObjectDrawingStyle.TheoremObject);

                    break;
                }

                // With concurrent lines
                case TheoremType.ConcurrentLines:
                {
                    // Get the intersection of the first line
                    var intersection = ConstructLine((LineTheoremObject)theorem.InvolvedObjectsList[0])
                        // With the second
                        .IntersectionWith(ConstructLine((LineTheoremObject)theorem.InvolvedObjectsList[1]))
                        // If there is no, then this is weird...
                        ?? throw new DrawingLauncherException("This lines should have had an intersection according to the theorem.");

                    // Draw the intersection point
                    figure.AddPoint(intersection, ObjectDrawingStyle.TheoremObject);

                    // Draw all the three lines so that they pass through the intersection
                    theorem.InvolvedObjects.Cast<LineTheoremObject>().ForEach(line => DrawTheoremLine(line, shifted: false, intersection));

                    break;
                }

                // With parallel lines
                case TheoremType.ParallelLines:

                    // We just draw them, without knowing about any other points they pass through
                    theorem.InvolvedObjects.Cast<LineTheoremObject>().ForEach(line => DrawTheoremLine(line, shifted: false));

                    break;

                // With perpendicular lines
                case TheoremType.PerpendicularLines:
                {
                    // Get the intersection of the first line
                    var intersection = ConstructLine((LineTheoremObject)theorem.InvolvedObjectsList[0])
                        // With the second
                        .IntersectionWith(ConstructLine((LineTheoremObject)theorem.InvolvedObjectsList[1]))
                        // If there is no, then this is weird...
                        ?? throw new DrawingLauncherException("This lines should have had an intersection according to the theorem.");

                    // Draw the intersection point
                    figure.AddPoint(intersection, ObjectDrawingStyle.TheoremObject);

                    // Draw all the three lines so that they pass through the intersection
                    theorem.InvolvedObjects.Cast<LineTheoremObject>().ForEach(line => DrawTheoremLine(line, shifted: false, intersection));

                    break;
                }

                // With tangent circles
                case TheoremType.TangentCircles:
                {
                    // Get the intersections of the first circle
                    var intersections = ConstructCircle((CircleTheoremObject)theorem.InvolvedObjectsList[0])
                        // With the second
                        .IntersectWith(ConstructCircle((CircleTheoremObject)theorem.InvolvedObjectsList[1]));

                    // Make sure there is one
                    if (intersections.Length != 1)
                        throw new DrawingLauncherException("This circles should have been tangent according to the theorem.");

                    // Get the intersection
                    var intersection = intersections[0];

                    // Draw the intersection point
                    figure.AddPoint(intersection, ObjectDrawingStyle.TheoremObject);

                    // Draw the circles
                    theorem.InvolvedObjects.Cast<CircleTheoremObject>().ForEach(circle => figure.AddCircle(ConstructCircle(circle), ObjectDrawingStyle.TheoremObject));

                    break;
                }

                // With a line tangent to a circle
                case TheoremType.LineTangentToCircle:
                {
                    // Get the circle and line
                    var circle = theorem.InvolvedObjects.OfType<CircleTheoremObject>().First();
                    var line = theorem.InvolvedObjects.OfType<LineTheoremObject>().First();

                    // Get their intersections
                    var intersections = ConstructCircle(circle).IntersectWith(ConstructLine(line));

                    // Make sure there is one
                    if (intersections.Length != 1)
                        throw new DrawingLauncherException("This line and circle should have been tangent according to the theorem.");

                    // Get the intersection
                    var intersection = intersections[0];

                    // Draw the intersection point
                    figure.AddPoint(intersection, ObjectDrawingStyle.TheoremObject);

                    // Draw the circle
                    figure.AddCircle(ConstructCircle(circle), ObjectDrawingStyle.TheoremObject);

                    // Draw the line so that it passes through the intersection point and is shifted
                    DrawTheoremLine(line, shifted: true, intersection);

                    break;
                }

                // With line segments
                case TheoremType.EqualLineSegments:
                {
                    // Take the line segments
                    var points = theorem.InvolvedObjects.Cast<LineSegmentTheoremObject>()
                        // And for each
                        .Select(segment => segment.PointSet.Cast<PointTheoremObject>()
                            // Get the points
                            .Select(point => (Point)picture.Get(point.ConfigurationObject)).ToArray())
                        // Enumerate
                        .ToArray();

                    // Draw the segments
                    points.ForEach(segmentPoints => figure.AddSegment(segmentPoints[0], segmentPoints[1], ObjectDrawingStyle.TheoremObject, shifted: false));

                    break;
                }

                // With incidence
                case TheoremType.Incidence:
                {
                    // Get the point and the other object
                    var point = theorem.InvolvedObjects.OfType<PointTheoremObject>().First();
                    var lineOrCircle = theorem.InvolvedObjects.OfType<TheoremObjectWithPoints>().First();

                    // Mark the point
                    figure.AddPoint((Point)picture.Get(point.ConfigurationObject), ObjectDrawingStyle.TheoremObject);

                    // Switch on the other object's type
                    switch (lineOrCircle)
                    {
                        // If this is a line...
                        case LineTheoremObject line:

                            // Then draw it so it passes through the point and is shifted a bit
                            DrawTheoremLine(line, shifted: true, (Point)picture.Get(point.ConfigurationObject));

                            break;

                        // If this is a circle...
                        case CircleTheoremObject circle:

                            // Then simply draw it
                            figure.AddCircle(ConstructCircle(circle), ObjectDrawingStyle.TheoremObject);

                            break;

                        // If something else...
                        default:
                            throw new DrawingLauncherException($"Unhandled type of {nameof(TheoremObjectWithPoints)}: {lineOrCircle.GetType()}");
                    }

                    break;
                }

                // Unhandled cases
                default:
                    throw new DrawingLauncherException($"Unhandled type of {nameof(TheoremType)}: {theorem.Type.GetType()}");
            }

            #endregion

            // Prepare the output formatter that does all the naming
            var formatter = new OutputFormatter(configuration.AllObjects, includeSubscriptsBeforeNumbers: true);

            #region Labeling

            // Go through all the objects and add a label for each            
            configuration.AllObjects.ForEach((configurationObject, index) =>
            {
                // Get the analytic version
                var analyticObject = picture.Get(configurationObject);

                // Add the label with the TeX dollars
                figure.AddLabel(analyticObject, $"${formatter.GetObjectName(configurationObject)}$");
            });

            #endregion

            #region Statement

            // Prepare the call for the macro that will define the loose objects layout
            var looseObjectMacro = $"{configuration.LooseObjectsHolder.Layout}{_settings.ConstructionTextMacroPrefix}" +
                // Get individual loose objects
                $"({configuration.LooseObjects.Select(looseObject => $"\"{formatter.GetObjectName(looseObject)}\"").ToJoinedString()})";

            // Now we converted individual constructed objects
            var constructedObjectMacros = configuration.ConstructedObjects.Select(constructedObject =>
                // Get the construction name that is used with the prefix in the macro call
                $"{constructedObject.Construction.Name}{_settings.ConstructionTextMacroPrefix}(" +
                // Append the actual object name
                $"\"{formatter.GetObjectName(constructedObject)}\", " +
                // Append the converted arguments
                constructedObject.PassedArguments.Select(formatter.FormatArgument)
                    // Join them
                    .ToJoinedString()
                    // Get rid of curly brackets that TeX won't read
                    .Replace("{", "").Replace("}", "")
                    // Now we need to split them again 
                    .Split(',')
                    // And trim and append double quotes
                    .Select(s => $"\"{s.Trim()}\"")
                    // And join again
                    .ToJoinedString()
                // And finally append the end of the call
                + ")");

            // Local helper function that converts a theorem object
            string ConvertTheoremObject(TheoremObject theoremObject) =>
                    // By taking it's inner objects
                    theoremObject.GetInnerConfigurationObjects()
                    // Converting those
                    .Select(formatter.GetObjectName)
                    // Gluing them together 
                    .ToJoinedString("");

            // Prepare the value holding the converted and sorted theorem objects
            string theoremObjectString;

            // Convert theorem objects based on the type of theorem
            switch (theorem.Type)
            {
                // In these cases we just want an alphabetical order
                case TheoremType.CollinearPoints:
                case TheoremType.ConcyclicPoints:
                case TheoremType.ConcurrentLines:
                case TheoremType.ParallelLines:
                case TheoremType.PerpendicularLines:
                case TheoremType.TangentCircles:
                case TheoremType.EqualLineSegments:

                    // Simply convert individual objects 
                    theoremObjectString = theorem.InvolvedObjects.Select(theoremObject => $"\"{ConvertTheoremObject(theoremObject)}\"")
                        // Sort
                        .Ordered()
                        // Join
                        .ToJoinedString();

                    break;

                // Here we want the line to be first
                case TheoremType.LineTangentToCircle:

                    // Convert the line and the circle
                    var lineName = ConvertTheoremObject(theorem.InvolvedObjects.OfType<LineTheoremObject>().First());
                    var circleName = ConvertTheoremObject(theorem.InvolvedObjects.OfType<CircleTheoremObject>().First());

                    // Compose the final string 
                    theoremObjectString = $"\"{lineName}\", \"{circleName}\"";

                    break;

                // Here we want the point to be first
                case TheoremType.Incidence:

                    // Convert the point and the other object
                    var pointName = ConvertTheoremObject(theorem.InvolvedObjects.OfType<PointTheoremObject>().First());
                    var lineOrCircleName = ConvertTheoremObject(theorem.InvolvedObjects.OfType<TheoremObjectWithPoints>().First());

                    // Compose the final string 
                    theoremObjectString = $"\"{pointName}\", \"{lineOrCircleName}\"";

                    break;

                // Unhandled cases
                default:
                    throw new DrawingLauncherException($"Unhandled value of {nameof(TheoremType)}: {theorem.Type}.");
            }

            // The final theorem macro consists of the theorem type with the prefix, which makes
            // the macro name, and already composed arguments for this macro 
            var theoremMacro = $"{theorem.Type}{_settings.ConstructionTextMacroPrefix}({theoremObjectString})";

            #endregion

            // Now we are ready to prepare the final lines of text. We take the definition of the layout
            var finalLines = looseObjectMacro.ToEnumerable()
                // Append the constructed objects
                .Concat(constructedObjectMacros)
                // Append a new line 
                .Concat("\"\\par \"")
                // Append the theorem macro
                .Concat(theoremMacro);

            #region Ranking            

            // If we are supposed to include ranking, do it
            if (_settings.IncludeRanking)
            {
                // Prepare the ranking table by calling the macro for the ranking table
                var rank = $"{_settings.RankingTableMacro}(" +
                    // Now we will append individual rankings
                    rankedTheorem.Ranking.Rankings
                        // Sorted by the contribution
                        .OrderBy(pair => -pair.Value.Contribution)
                        // Now we can convert each to a single string with these 4 values. Add the type first
                        .Select(pair => $"\"{pair.Key}\"," +
                            // Append the ranking
                            $"\"${pair.Value.Ranking.ToStringWithDecimalDot()}$\"," +
                            // Append the weight
                            $"\"${pair.Value.Weight.ToStringWithDecimalDot()}$\"," +
                            // Append the contribution
                            $"\"${pair.Value.Contribution.ToStringWithDecimalDot()}$\"")
                        // They are joined together by commas, the macro will take care of splitting these quadruples
                        .ToJoinedString(",")
                    // Append the end of the call
                    + ")";

                // Take the already constructed lines
                finalLines = finalLines
                    // Append a new line with some additional space 
                    .Concat("\"\\par\\noindent\\medskip \"")
                    // Append the table with rankings
                    .Concat(rank)
                    // Append a new line with some additional space 
                    .Concat("\"\\par\\medskip \"")
                    // Append the total ranking
                    .Concat($"\"Total ranking: ${rankedTheorem.Ranking.TotalRanking.ToStringWithDecimalDot()}$\"");
            }

            #endregion

            // Construct the final text by joining the lines with '&', which is the concatenation operator in MetaPost
            // For better readability we add some spaces and new lines
            var finalText = finalLines.ToJoinedString($"\n& ");

            // Finally we can add the text to the picture
            figure.AddText(finalText);

            // Return it
            return figure;
        }

        /// <summary>
        /// The method that generates the actual MetaPost code to be complied. 
        /// </summary>
        /// <param name="figures">The figures to be drawn.</param>
        /// <param name="startingId">The id of the first figure. Others will be identified consecutively.</param>
        /// <returns>A compilable MetaPost code of the figures.</returns>
        private string CreateCode(IEnumerable<MetapostFigure> figures, int startingId)
        {
            // Let's use StringBuilder for 'efficiency'
            var code = new StringBuilder();

            // Append the preamble
            code.Append($"input \"{_settings.MetapostMacroLibraryPath}\"\n\n");

            // Append all the figures
            figures.ForEach((figure, index) =>
            {
                // Append the preamble
                code.Append($"beginfig({startingId + index});\n\n");

                // Append the actual code of the picture using the provided drawing data
                code.Append(figure.ToCode(_settings.DrawingData));

                // Append the end
                code.Append($"\nendfig;\n\n");
            });

            // Append the end
            code.Append("end");

            // Return the result
            return code.ToString();
        }

        #endregion
    }
}