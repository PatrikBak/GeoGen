using GeoGen.Utilities;
using System.Text;

namespace GeoGen.Core
{
    /// <summary>
    /// A helper class that converts GeoGen core objects to formatted readable strings.
    /// </summary>
    public class OutputFormatter
    {
        #region Private fields

        /// <summary>
        /// The dictionary mapping objects to their names.
        /// </summary>
        private readonly Dictionary<ConfigurationObject, string> _objectNames = new Dictionary<ConfigurationObject, string>();

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputFormatter"/> class.
        /// </summary>
        /// <param name="objects">The objects to be named and used later in formatting.</param>
        /// <param name="includeSubscriptsBeforeNumbers">Indicates whether the names of objects should have subscript marks '_' in from of their numbers. False by default.</param>
        public OutputFormatter(IEnumerable<ConfigurationObject> objects, bool includeSubscriptsBeforeNumbers = false)
        {
            // Name the given objects
            NameObjects(objects, includeSubscriptsBeforeNumbers);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Creates a formatted string describing a given configuration.
        /// </summary>
        /// <returns>The string representing the configuration.</returns>
        public string FormatConfiguration(Configuration configuration)
        {
            // Prepare the result
            var result = new StringBuilder();

            // Compose the loose objects into a string
            var looseObjects = configuration.LooseObjects.Select(looseObject => _objectNames[looseObject]).ToJoinedString();

            // Add the first line with loose objects and its layout
            result.Append($"{configuration.LooseObjectsHolder.Layout}: {looseObjects}\n");

            // Add every constructed object
            foreach (var constructedObject in configuration.ConstructedObjects)
            {
                // Prepare the object's definition
                var objectsDefinition = FormatConfigurationObject(constructedObject);

                // Add it to the result with the name
                result.Append($"{_objectNames[constructedObject]} = {objectsDefinition}\n");
            }

            // Return the trimmed result
            return result.ToString().Trim();
        }

        /// <summary>
        /// Creates a formatted string describing a given theorem. 
        /// </summary>
        /// <param name="theorem">The theorem.</param>
        /// <returns>The string representing the theorem.</returns>
        public string FormatTheorem(Theorem theorem)
        {
            // Prepare the type string
            var typeString = $"{theorem.Type}: ";

            // Switch based on the type
            switch (theorem.Type)
            {
                // Special case where we want the named objects first
                case TheoremType.EqualObjects:
                {
                    // Get their strings
                    var object1String = FormatTheoremObject(theorem.InvolvedObjectsList[0]);
                    var object2String = FormatTheoremObject(theorem.InvolvedObjectsList[1]);

                    // We presort them
                    var smaller = object1String.CompareTo(object2String) < 0 ? object1String : object2String;
                    var larger = smaller == object1String ? object2String : object1String;

                    // Get the objects (might be null)
                    var object1 = ((BaseTheoremObject)theorem.InvolvedObjectsList[0]).ConfigurationObject;
                    var object2 = ((BaseTheoremObject)theorem.InvolvedObjectsList[1]).ConfigurationObject;

                    // Find which of them has a name
                    var isObject1Named = object1 != null && _objectNames.ContainsKey(object1);
                    var isObject2Named = object2 != null && _objectNames.ContainsKey(object2);

                    // If both or neither has a name, return them sorted
                    if ((isObject1Named && isObject2Named) || (!isObject1Named && !isObject2Named))
                        return $"{typeString}{smaller}, {larger}";

                    // Otherwise we want the named object first
                    var namedObject = isObject1Named ? object1String : object2String;
                    var otherObject = namedObject == object1String ? object2String : object1String;

                    // Return them in the right order
                    return $"{typeString}{namedObject}, {otherObject}";
                }

                // Special case where there is a point and we want it first
                case TheoremType.Incidence:
                {
                    // Get the objects
                    var object1 = ((BaseTheoremObject)theorem.InvolvedObjectsList[0]).ConfigurationObject;
                    var object2 = ((BaseTheoremObject)theorem.InvolvedObjectsList[1]).ConfigurationObject;

                    // Get their strings
                    var object1String = FormatTheoremObject(theorem.InvolvedObjectsList[0]);
                    var object2String = FormatTheoremObject(theorem.InvolvedObjectsList[1]);

                    // We want the point first
                    var pointString = (object1 != null && object1.ObjectType == ConfigurationObjectType.Point) ? object1String : object2String;
                    var otherString = pointString == object1String ? object2String : object1String;

                    // Return them in the right order
                    return $"{typeString}{pointString}, {otherString}";
                }

                // In every other case, convert each object and sort them
                default:
                    return $"{typeString}{theorem.InvolvedObjects.Select(FormatTheoremObject).Ordered().ToJoinedString()}";
            }
        }

        /// <summary>
        /// Gets the string definition of a passed configuration object. Loose objects have only
        /// names as their definition, whereas constructed objects have the construction and arguments.
        /// </summary>
        /// <param name="configurationObject">The object that we're formatting.</param>
        /// <returns>The string representing the object.</returns>
        public string FormatConfigurationObject(ConfigurationObject configurationObject)
        {
            // Switch based on the object
            return configurationObject switch
            {
                // Loose objects have their names as the definition
                LooseConfigurationObject _ => _objectNames[configurationObject],

                // For constructed objects we include the construction and arguments
                ConstructedConfigurationObject constructedObject => $"{constructedObject.Construction.Name}({constructedObject.PassedArguments.Select(FormatArgument).ToJoinedString()})",

                // Unhandled cases
                _ => throw new GeoGenException($"Unhandled type of {nameof(ConfigurationObject)}: {configurationObject.GetType()}")
            };
        }

        /// <summary>
        /// Converts a given construction argument to a string using the curly braces notation.
        /// </summary>
        /// <param name="argument">The argument to be converted.</param>
        /// <returns>The resulting string.</returns>
        public string FormatArgument(ConstructionArgument argument)
        {
            // Switch based on the argument type
            return argument switch
            {
                // If we have an object argument, ask directly for the name of its object
                ObjectConstructionArgument objectArgument => _objectNames.GetValueOrDefault(objectArgument.PassedObject)
                        // If there is none, we don't have a different option but converting it (and get an ugly string)
                        ?? FormatConfigurationObject(objectArgument.PassedObject),

                // For set argument we wrap the result in curly braces and convert the inner arguments
                SetConstructionArgument setArgument => $"{{{setArgument.PassedArguments.Select(FormatArgument).Ordered().ToJoinedString()}}}",

                // Unhandled cases
                _ => throw new GeoGenException($"Unhandled type of {nameof(ConstructionArgument)}: {argument.GetType()}"),
            };
        }

        /// <summary>
        /// Converts a given theorem object to a string.
        /// </summary>
        /// <param name="theoremObject">The theorem object to be converted.</param>
        /// <returns>The resulting string.</returns>
        public string FormatTheoremObject(TheoremObject theoremObject)
        {
            // Switch on the type
            switch (theoremObject)
            {
                // Base objects might have an object part
                case BaseTheoremObject baseObject:

                    // If it has the object part...
                    if (baseObject.ConfigurationObject != null)
                    {
                        // Get the object for comfort
                        var definingObject = baseObject.ConfigurationObject;

                        // Try to return it's name, if there is any
                        if (_objectNames.ContainsKey(definingObject))
                            return _objectNames[definingObject];

                        // Otherwise return the definition
                        return FormatConfigurationObject(definingObject);
                    }

                    // If this object doesn't have an object part, it must be defined differently
                    switch (baseObject)
                    {
                        // If we have an object with points...
                        case TheoremObjectWithPoints objectWithPoints:

                            // Get the points
                            var namedPoints = objectWithPoints.Points
                                // That have a name
                                .Where(_objectNames.ContainsKey)
                                // Get those names
                                .Select(point => _objectNames[point])
                                // Order them
                                .Ordered();

                            // Get the points
                            var otherPoints = objectWithPoints.Points
                                // That don't have a name
                                .Where(point => !_objectNames.ContainsKey(point))
                                // Get their definition
                                .Select(FormatConfigurationObject)
                                // Order them
                                .Ordered();

                            // Get the final string by concating the named points first and then the order points
                            var pointsPart = namedPoints.Concat(otherPoints).ToJoinedString();

                            // Dig further to provide information whether it is a line or circle
                            return objectWithPoints switch
                            {
                                // If we have a line, add [] around points 
                                LineTheoremObject _ => $"[{pointsPart}]",

                                // If we have a circle, add () around points
                                CircleTheoremObject _ => $"({pointsPart})",

                                // Unhandled cases
                                _ => throw new GeoGenException($"Unhandled type of {nameof(TheoremObjectWithPoints)}: {objectWithPoints.GetType()}"),
                            };

                        // Unhandled cases
                        default:
                            throw new GeoGenException($"Unhandled type of {nameof(BaseTheoremObject)}: {baseObject.GetType()}");
                    }

                // For a line segment
                case LineSegmentTheoremObject lineSegment:

                    // We convert individual objects
                    var point1 = FormatTheoremObject(lineSegment.Point1);
                    var point2 = FormatTheoremObject(lineSegment.Point2);

                    // Get the smaller and the larger
                    var smaller = point1.CompareTo(point2) < 0 ? point1 : point2;
                    var larger = smaller == point1 ? point2 : point1;

                    // Compose the final string
                    return $"{smaller}, {larger}";

                // Unhandled cases
                default:
                    throw new GeoGenException($"Unhandled type of {nameof(TheoremObject)}: {theoremObject.GetType()}");
            }
        }

        /// <summary>
        /// Gets the name of the passed object.
        /// </summary>
        /// <param name="configurationObject">The object.</param>
        /// <returns>The objects name.</returns>
        public string GetObjectName(ConfigurationObject configurationObject) =>
            // Try to get it from the dictionary and make it known if it's not there
            _objectNames.GetValueOrDefault(configurationObject) ?? throw new GeoGenException("Unknown object.");

        #endregion

        #region Private methods

        /// <summary>
        /// Names the passed objects and adds their names to the <see cref="_objectNames"/> dictionary.
        /// </summary>
        /// <param name="objects">The objects to be named.</param>
        /// <param name="includeSubscriptsBeforeNumbers">Indicates whether the names of objects should have subscript marks '_' in from of their numbers. False by default.</param>
        private void NameObjects(IEnumerable<ConfigurationObject> objects, bool includeSubscriptsBeforeNumbers = false)
        {
            // Prepare the numbers of currently named objects of particular types
            var namedPoints = 0;
            var namedCircles = 0;
            var namedLines = 0;

            // Helper values of the total numbers of points and lines
            var numberOfLines = objects.Count(_object => _object.ObjectType == ConfigurationObjectType.Line);
            var numberOfCircles = objects.Count(_object => _object.ObjectType == ConfigurationObjectType.Circle);

            // Go through all the objects
            foreach (var configurationObject in objects)
            {
                // Prepare the name
                var name = default(string);

                // Handle the cases based on the type
                switch (configurationObject.ObjectType)
                {
                    // If we have a point...
                    case ConfigurationObjectType.Point:

                        // Compose the name
                        name = $"{(char)('A' + namedPoints)}";

                        // Count it 
                        namedPoints++;

                        break;

                    // If we have a line...
                    case ConfigurationObjectType.Line:

                        // Compose the name
                        name = numberOfLines == 1 ? "l" : $"l{(includeSubscriptsBeforeNumbers ? "_" : "")}{namedLines + 1}";

                        // Count it 
                        namedLines++;

                        break;

                    // If we have a circle...
                    case ConfigurationObjectType.Circle:

                        // Compose the name
                        name = numberOfCircles == 1 ? "c" : $"c{(includeSubscriptsBeforeNumbers ? "_" : "")}{namedCircles + 1}";

                        // Count it 
                        namedCircles++;

                        break;
                }

                // Register the name
                _objectNames.Add(configurationObject, name);
            }
        }

        #endregion
    }
}