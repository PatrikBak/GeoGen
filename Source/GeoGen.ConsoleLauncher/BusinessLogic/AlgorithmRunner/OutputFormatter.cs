using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// A helper class that converts a configuration and its theorems to formatted readable strings.
    /// </summary>
    public class OutputFormatter
    {
        #region Private fields

        /// <summary>
        /// The dictionary mapping objects to their names.
        /// </summary>
        private readonly Dictionary<ConfigurationObject, string> _objectNames = new Dictionary<ConfigurationObject, string>();

        /// <summary>
        /// The configuration to be formatted.
        /// </summary>
        private readonly Configuration _configuration;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputFormatter"/> class 
        /// handling the given configuration.
        /// </summary>
        /// <param name="configuration">The configuration to be formatted.</param>
        public OutputFormatter(Configuration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            // Call the function that creates names for the objects in the configuration
            NameObjects();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Creates a formatted string describing the configuration.
        /// </summary>
        /// <returns>The string representing the configuration.</returns>
        public string FormatConfiguration()
        {
            // Prepare the result
            var result = new StringBuilder();

            // Compose the loose objects string
            var looseObjects = _configuration.LooseObjectsHolder.LooseObjects.Select(o => _objectNames[o]).ToJoinedString();

            // Add the first line with loose objects
            result.Append($"{_configuration.LooseObjectsHolder.Layout}: {looseObjects}\n");

            // Add every constructed object
            foreach (var constructedObject in _configuration.ConstructedObjects)
            {
                // Prepare the object's definition
                var objectsDefinition = $"{constructedObject.Construction.Name}({constructedObject.PassedArguments.Select(ArgumentToString).ToJoinedString()})";

                // Add it to the result
                result.Append($"{_objectNames[constructedObject]} = {objectsDefinition}\n");
            }

            // Return the trimmed result
            return result.ToString().Trim();
        }

        /// <summary>
        /// Creates a formatted string describing the configuration.
        /// </summary>
        /// <param name="theorems">The theorems.</param>
        /// <returns>The string representing the theorems.</returns>
        public string FormatTheorems(IEnumerable<Theorem> theorems)
        {
            // Convert individual theorems to a string
            return theorems.Select(theorem =>
            {
                // Switch based on the type
                switch (theorem.Type)
                {
                    // Handle the case where the first two objects are exchangeable
                    case TheoremType.EqualAngles:
                    case TheoremType.EqualLineSegments:

                        // Convert the first two and the second to in an ordered way.
                        var firstTwo = theorem.InvolvedObjects.Take(2).Select(TheoremObjectToString).OrderBy(s => s).ToJoinedString();
                        var secondTwo = theorem.InvolvedObjects.Skip(2).Select(TheoremObjectToString).OrderBy(s => s).ToJoinedString();

                        // Get the smaller and the larger
                        var smaller = firstTwo.CompareTo(secondTwo) < 0 ? firstTwo : secondTwo;
                        var larger = smaller == firstTwo ? secondTwo : firstTwo;

                        // Compose the final string
                        return $" - {theorem.Type}: {smaller}, {larger}";

                    // Handle the case where the objects should not be sorted
                    case TheoremType.LineTangentToCircle:

                        // Simply convert each object to a string
                        return $" - {theorem.Type}: {theorem.InvolvedObjects.Select(TheoremObjectToString).ToJoinedString()}";

                    // In every other case...
                    default:

                        // Convert each object and sort them
                        return $" - {theorem.Type}: {theorem.InvolvedObjects.Select(TheoremObjectToString).OrderBy(s => s).ToJoinedString()}";
                }
            })
            // Make each on a separate line
            .ToJoinedString("\n");
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Names the objects of the <see cref="_configuration"/> and adds them 
        /// to the <see cref="_objectNames"/> dictionary.
        /// </summary>
        private void NameObjects()
        {
            // Prepare the numbers of currently named objects of particular types
            var namedPoints = 0;
            var namedCircles = 0;
            var namedLines = 0;

            // Helper values of the total numbers of points and lines
            var numberOfLines = _configuration.ObjectsMap.AllObjects.Count(o => o.ObjectType == ConfigurationObjectType.Line);
            var numberOfCircles = _configuration.ObjectsMap.AllObjects.Count(o => o.ObjectType == ConfigurationObjectType.Circle);

            // Go through all the objects
            foreach (var configurationObject in _configuration.ObjectsMap.AllObjects)
            {
                // Prepare the name
                var name = default(string);

                // Handle the cases based on the type
                switch (configurationObject.ObjectType)
                {
                    // If we have a point...
                    case ConfigurationObjectType.Point:

                        // Compose the name
                        name = $"{(char) ('A' + namedPoints)}";

                        // Count it 
                        namedPoints++;

                        break;

                    // If we have a line...
                    case ConfigurationObjectType.Line:

                        // Compose the name
                        name = numberOfLines == 1 ? "l" : $"l{namedLines + 1}";

                        // Count it 
                        namedLines++;

                        break;

                    // If we have a circle...
                    case ConfigurationObjectType.Circle:

                        // Compose the name
                        name = numberOfCircles == 1 ? "c" : $"c{namedCircles + 1}";

                        // Count it 
                        namedCircles++;

                        break;
                }

                // Register the name
                _objectNames.Add(configurationObject, name);
            }
        }

        /// <summary>
        /// Converts a given construction argument to a string using the curly braces notation.
        /// </summary>
        /// <param name="argument">The argument to be converted.</param>
        /// <returns>The resulting string.</returns>
        private string ArgumentToString(ConstructionArgument argument)
        {
            // If we have an object argument, ask directly for the name of its object
            if (argument is ObjectConstructionArgument objectArgument)
                return _objectNames[objectArgument.PassedObject];

            // Otherwise we have a set argument
            var setArgument = (SetConstructionArgument) argument;

            // We wrap the result in curly braces and convert the inner arguments
            return $"{{{setArgument.PassedArguments.Select(ArgumentToString).OrderBy(s => s).ToJoinedString()}}}";
        }

        /// <summary>
        /// Converts a given theorem object to a string.
        /// </summary>
        /// <param name="theoremObject">The theorem object to be converted.</param>
        /// <returns>The resulting string.</returns>
        private string TheoremObjectToString(TheoremObject theoremObject)
        {
            // Prepare the object name part
            var objectNamePart = theoremObject.ConfigurationObject == null ? "" : _objectNames[theoremObject.ConfigurationObject];

            // If the object is a point, then there is no other part
            if (theoremObject.Type == ConfigurationObjectType.Point)
                return objectNamePart;

            // Otherwise we have a line or circle
            var lineOrCircle = (TheoremObjectWithPoints) theoremObject;

            // Find out if it's a line
            var isLine = lineOrCircle.Type == ConfigurationObjectType.Line;

            // Prepare the points list
            var pointsList = lineOrCircle.Points.Select(o => _objectNames[o]).OrderBy(s => s).ToJoinedString();

            // Prepare the points part
            var pointsPart = lineOrCircle.Points.Count == 0 ? "" : $"{(isLine ? "[" : "(")}{pointsList}{(isLine ? "]" : ")")}";

            // Compose the fink string
            return $"{objectNamePart}{pointsPart}";
        }

        #endregion
    }
}