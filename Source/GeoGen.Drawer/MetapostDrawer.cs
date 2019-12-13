using GeoGen.AnalyticGeometry;
using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoGen.Drawer
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

        #region IMetapostDrawer implementation

        /// <summary>
        /// Draws given configurations with its theorems.
        /// </summary>
        /// <param name="configurationWithTheorems">The configurations with theorems to be drawn.</param>
        /// <returns>The task representing the result.</returns>
        public async Task DrawAsync(IEnumerable<(Configuration configuration, Theorem theorem)> configurationWithTheorems)
        {
            // Create a figure from configuration-theorem pair
            var figures = configurationWithTheorems.Select(CreateFigure).ToArray();

            #region Writing code file

            // Get the code for them 
            var code = CreateCode(figures);

            try
            {
                // Create the file that will be compiled
                await File.WriteAllTextAsync(_settings.MetapostCodeFilePath, code);
            }
            catch (Exception e)
            {
                // Throw further
                throw new DrawerException($"Couldn't write the MetaPost code to the path {_settings.MetapostCodeFilePath}", e);
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

                // Run it with the argument equal to the number of generated files
                (exitCode, output, errors) = await ProcessUtilities.RunCommandAsync(_settings.PostcompilationCommand, arguments: $"{figures.Length}");

                // If the error code is not OK, i.e. not zero, make aware
                if (exitCode != 0)
                    throw new CommandException($"The execution of the post-compilation command failed.", command, exitCode, output, errors);
            }

            #endregion
        }

        /// <summary>
        /// Constructs a figure for the given configuration and theorem.
        /// </summary>
        /// <param name="configurtionTheoremPair">The pair of configuration and theorem to be drawn.</param>
        /// <returns>The MetaPost figure.</returns>
        private MetapostFigure CreateFigure((Configuration configuration, Theorem theorem) configurtionTheoremPair)
        {
            // Deconstruct
            var (configuration, theorem) = configurtionTheoremPair;

            // Safely execute
            var (pictures, constructionData) = GeneralUtilities.TryExecute(
                // Constructing the configuration
                () => _constructor.Construct(configuration, _settings.NumberOfPictures),
                // Make sure a potential exception is caught and re-thrown
                (InconsistentPicturesException e) => throw new ConstructionException("Drawing of the initial configuration failed.", e));

            // Make sure there is no inconstructible object
            if (constructionData.InconstructibleObject != default)
                throw new ConstructionException("The configuration cannot be constructed, because it contains an inconstructible object.");

            // Make sure there are no duplicates
            if (constructionData.Duplicates != default)
                throw new ConstructionException("The configuration cannot be constructed, because it contains duplicate objects");

            // Construct the figures
            return pictures.Select(picture =>
            {
                try
                {
                    // Try to construct the figure
                    var figure = ConstructFigure(configuration, theorem, picture);

                    // Rank it
                    var rank = figure.CalculateVisualBadness();

                    // Return the tuple
                    return (figure, rank);
                }
                catch (AnalyticException e)
                {
                    // Make aware if there is a weird problem
                    throw new ConstructionException("Cannot construct a figure because analytic geometry failed.", e);
                }
            })
            // Find the one with the smallest ranking, i.e. the most beautiful one
            .MinItem(item => item.rank)
            // Unwrap it
            .figure;
        }

        /// <summary>
        /// Performs the construction of a MetaPost figure holding the passed configuration and theorem
        /// with respect to the passed picture containing needed analytic objects.
        /// </summary>
        /// <param name="configuration">The configuration to be constructed.</param>
        /// <param name="theorem">The theorem to be constructed.</param>
        /// <param name="picture">The picture with analytic representations of the objects.</param>
        /// <returns>The constructed MetaPost figure.</returns>
        private MetapostFigure ConstructFigure(Configuration configuration, Theorem theorem, Picture picture)
        {
            // Create an empty figure
            var figure = new MetapostFigure();

            #region Loose object layout construction

            // Get the loose objects
            var looseObjects = configuration.LooseObjects.Select(picture.Get).ToArray();

            // Switch based on the loose objects layout
            switch (configuration.LooseObjectsHolder.Layout)
            {
                // Triangle case
                case LooseObjectsLayout.Triangle:

                    // In this case we have three points
                    var points = looseObjects.Cast<Point>().ToArray();

                    // We want to mark each as a normal object
                    points.ForEach(point => figure.AddPoint(point, ObjectDrawingStyle.NormalObject));

                    // And also draw the actual triangle
                    points.UnorderedPairs().ForEach(pair => figure.AddSegment(pair.Item1, pair.Item2, ObjectDrawingStyle.NormalObject, shifted: false));

                    break;

                // Currently nothing else :/
                default:
                    throw new DrawerException($"Unhandled type of {nameof(LooseObjectsLayout)}: {configuration.LooseObjectsHolder.Layout}");
            }

            #endregion

            #region Constructed object construction

            // Add each constructed object
            configuration.ConstructedObjects.ForEach(constructedObject =>
            {
                // Make sure it has a rule
                var rule = _data.DrawingRules.GetOrDefault(constructedObject.Construction)
                    // If not, make aware
                    ?? throw new DrawerException($"Cannot draw the picture, because construction {constructedObject.Construction} has no drawing rule.");

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

                    // The remapped object now has the real object as its arguments, so we can construct it
                    var analyticObject = _constructor.Construct(picture, remmapedObject, addToPicture: true);

                    // If the object cannot be constructed, make aware
                    if (analyticObject == null)
                        throw new ConstructionException($"A drawing command of the drawing rule for {rule.ObjectToDraw.Construction} contains an inconstructible object.");

                    // It might be used in other constructions, so we need to put it into the map
                    objectMap.Add(auxiliaryObject, remmapedObject);
                });

                // Perform the individual drawing commands
                rule.DrawingCommands.ForEach(command =>
                {
                    // Find the remapped versions of objects being drawn
                    var objectsBeingDrawn = command.Arguments.Select(configurationObject =>
                            // Object has to be already mapped
                            objectMap.GetOrDefault(configurationObject)
                                // Otherwise we have a huge problem...
                                ?? throw new DrawerException($"A drawing command of the drawing rule for {rule.ObjectToDraw.Construction} contains an undefined object."))
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

                        // If something else...
                        default:
                            throw new DrawerException($"Unhandled type of {nameof(DrawingCommandType)}: {command.Type}");
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
                        ?? throw new DrawerException("This lines should have had an intersection according to the theorem.");

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
                        ?? throw new DrawerException("This lines should have had an intersection according to the theorem.");

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
                        throw new DrawerException("This circles should have been tangent according to the theorem.");

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
                        throw new DrawerException("This line and circle should have been tangent according to the theorem.");

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
                        .Select(segment => segment.ObjectsSet.Cast<PointTheoremObject>()
                            // Get the points
                            .Select(point => (Point)picture.Get(point.ConfigurationObject)).ToArray())
                        // Enumerate
                        .ToArray();

                    // Mark all the points
                    points.Flatten().ForEach(point => figure.AddPoint(point, ObjectDrawingStyle.TheoremObject));

                    // Draw the segments
                    points.ForEach(segmentPoints => figure.AddSegment(segmentPoints[0], segmentPoints[1], ObjectDrawingStyle.TheoremObject, shifted: false));

                    break;
                }

                // With incidence
                case TheoremType.Incidence:
                {
                    // Get the point and the other object
                    var point = theorem.InvolvedObjects.OfType<CircleTheoremObject>().First();
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
                            throw new DrawerException($"Unhandled type of {nameof(TheoremObjectWithPoints)}: {lineOrCircle.GetType()}");
                    }

                    break;
                }

                // If something else...
                default:
                    throw new DrawerException($"Unhandled type of {nameof(TheoremType)}: {theorem.Type.GetType()}");
            }

            #endregion

            #region Labeling

            // Go through all the objects            
            configuration.AllObjects
                // That are points
                // TODO: Support labels of other objects 
                .Where(configurationObject => configurationObject.ObjectType == ConfigurationObjectType.Point)
                // Make a label for each
                .ForEach((configurationObject, index) =>
                {
                    // Create the label so that points are labeled A, B, C,...
                    var label = $"${(char)('A' + index)}$";

                    // Get the analytic version
                    var analyticPoint = (Point)picture.Get(configurationObject);

                    // Add the label
                    figure.AddLabel(analyticPoint, label);
                });

            #endregion

            // Return it
            return figure;
        }

        /// <summary>
        /// The method that generates the actual MetaPost code to be complied. 
        /// </summary>
        /// <param name="figures">The figures to be drawn.</param>
        /// <returns>A compilable MetaPost code of the figures.</returns>
        private string CreateCode(IEnumerable<MetapostFigure> figures)
        {
            // Let's use StringBuilder for 'efficiency'
            var code = new StringBuilder();

            // Append the preamble
            code.Append($"input \"{_settings.MetapostMacrosLibraryPath}\"\n\n");

            // Append all the figures
            figures.ForEach((figure, index) =>
            {
                // Append the preamble
                code.Append($"beginfig({index + 1});\n\n");

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