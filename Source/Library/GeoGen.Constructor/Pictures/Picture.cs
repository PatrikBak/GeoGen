using GeoGen.AnalyticGeometry;
using GeoGen.Core;
using GeoGen.Utilities;
using System.Collections;

namespace GeoGen.Constructor
{
    /// <summary>
    /// Represents a picture that handles mapping of <see cref="ConfigurationObject"/> to their analytic 
    /// representations, i.e. <see cref="IAnalyticObject"/>s. 
    /// </summary>
    public class Picture : IEnumerable<(ConfigurationObject configurationObject, IAnalyticObject analyticObject)>
    {
        #region Private fields

        /// <summary>
        /// The map representing the actual content of the picture.
        /// </summary>
        private readonly Map<ConfigurationObject, IAnalyticObject> _content;

        /// <summary>
        /// The dictionary mapping accepted types of analytic objects to 
        /// their corresponding configuration object types.
        /// </summary>
        private readonly IReadOnlyDictionary<Type, ConfigurationObjectType> _correctTypes;

        /// <summary>
        /// The dictionary mapping objects that are not present in the container to their
        /// equal objects that are present.
        /// </summary>
        private readonly Dictionary<ConfigurationObject, ConfigurationObject> _equalObjects;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Picture"/> class.
        /// </summary>
        public Picture()
        {
            // Set the correct types mapping
            _correctTypes = new Dictionary<Type, ConfigurationObjectType>
            {
                {typeof(Point), ConfigurationObjectType.Point},
                {typeof(Circle), ConfigurationObjectType.Circle},
                {typeof(Line), ConfigurationObjectType.Line}
            };

            // Create the equal objects dictionary
            _equalObjects = new Dictionary<ConfigurationObject, ConfigurationObject>();

            // Create the content map
            _content = new Map<ConfigurationObject, IAnalyticObject>();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Gets the analytic representation of a given configuration object. If this object 
        /// is not explicitly present, but it's equal version is, then the representation of
        /// this equal object will be returned.
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        /// <returns>The analytic object.</returns>
        public IAnalyticObject Get(ConfigurationObject configurationObject)
            // Return the object if it's present, otherwise fall-back to duplicates
            => _content.GetRightValueOrDefault(configurationObject) ?? _content.GetRightValue(_equalObjects[configurationObject]);

        /// <summary>
        /// Gets the configuration object corresponding to a given analytic object.
        /// </summary>
        /// <param name="analyticObject">The analytic object.</param>
        /// <returns>The configuration object of a given analytic object.</returns>
        public ConfigurationObject Get(IAnalyticObject analyticObject) => _content.GetLeftValue(analyticObject);

        /// <summary>
        /// Finds out if a given analytic object is present if the picture.
        /// </summary>
        /// <param name="analyticObject">The analytic object.</param>
        /// <returns>true, if the object is present in the picture; false otherwise.</returns>
        public bool Contains(IAnalyticObject analyticObject) => _content.ContainsRightKey(analyticObject);

        /// <summary>
        /// Finds out if a given configuration object is present if the picture.
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        /// <returns>true, if the object is present in the picture; false otherwise.</returns>
        public bool Contains(ConfigurationObject configurationObject) =>
            // Return that the object is either in the dictionary or among duplicates
            _content.ContainsLeftKey(configurationObject) || _equalObjects.ContainsKey(configurationObject);

        /// <summary>
        /// Removes a given configuration object from the picture.
        /// </summary>
        /// <param name="configurationObject">The object to be removed.</param>
        public void Remove(ConfigurationObject configurationObject)
        {
            // If it's in not in the content
            if (!_content.ContainsLeftKey(configurationObject))
            {
                // Then it is mapped to some duplicate, remove it from the equal object dictionary
                _equalObjects.Remove(configurationObject);

                // We're done
                return;
            }

            // Otherwise we are going to remove it from the content dictionary, which might
            // be a problem because some equal objects might be mapped to it. Find such object
            var firstEqualObject = _equalObjects.Where(pair => pair.Value.Equals(configurationObject))
                // Take it
                .Select(pair => pair.Key)
                // Take the first or null
                .FirstOrDefault();

            // If this object is null
            if (firstEqualObject == null)
            {
                // Then we can safely remove the object
                _content.RemoveLeft(configurationObject);

                // And we're done
                return;
            }

            // Otherwise we first find the equal analytic representation of the current object
            var analyticObject = _content.GetRightValue(configurationObject);

            // Now we remove it
            _content.RemoveLeft(configurationObject);

            // Add the equal object with this analytic representation
            _content.Add(firstEqualObject, analyticObject);

            // Remove the equal object from the equal object dictionary
            _equalObjects.Remove(firstEqualObject);

            // Ensure that every object that had the previous one in the equal object dictionary
            _equalObjects.Where(pair => pair.Value.Equals(configurationObject)).ToList()
                // Has now the found equal object as the representant
                .ForEach(pair => _equalObjects[pair.Key] = firstEqualObject);
        }

        /// <summary>
        /// Clones the picture.
        /// </summary>
        /// <returns>The cloned picture.</returns>
        public Picture Clone()
        {
            // Create an empty picture
            var clonedPicture = new Picture();

            // Copy the content
            _content.ForEach(pair => clonedPicture._content.Add(pair.Item1, pair.Item2));

            // Return it
            return clonedPicture;
        }

        /// <summary>
        /// Finds the analytic version of a given base theorem object.
        /// </summary>
        /// <param name="baseTheoremObject">The base theorem object that we're constructing.</param>
        /// <returns>The analytic version of the passed object, or null, if the construction couldn't be carried out.</returns>
        public IAnalyticObject Construct(BaseTheoremObject baseTheoremObject)
        {
            // If the base object is defined explicitly
            if (baseTheoremObject.ConfigurationObject != null)
                // Then we know it's inner configuration object is already constructed and we find it in the picture
                return Get(baseTheoremObject.ConfigurationObject);

            // Otherwise switch based on type
            switch (baseTheoremObject)
            {
                // If we have an object with points, i.e. line / circle...
                case TheoremObjectWithPoints objectWithPoints:

                    // Take the points
                    var points = objectWithPoints.Points
                        // Find their analytic versions
                        .Select(Get)
                        // Cast
                        .Cast<Point>()
                        // Make sure they are distinct
                        .Distinct()
                        // Enumerate
                        .ToArray();

                    // If the number of found points doesn't match the number of defining ones,
                    // then the construction couldn't be carried out
                    if (points.Length != objectWithPoints.NumberOfDefiningPoints)
                        return null;

                    // If the counts are fine, we can construct the object
                    return objectWithPoints switch
                    {
                        // Line requires 2 points
                        LineTheoremObject _ => new Line(points[0], points[1]),

                        // Circle requires 3 points
                        CircleTheoremObject _ => new Circle(points[0], points[1], points[2]),

                        // Unhandled cases
                        _ => throw new ConstructorException($"Unhandled type of {nameof(TheoremObjectWithPoints)}: {objectWithPoints.GetType()}")
                    };

                // Unhandled cases
                default:
                    throw new ConstructorException($"Unhandled type of {nameof(TheoremObject)}: {baseTheoremObject.GetType()}");
            }
        }

        #endregion

        #region Internal methods

        /// <summary>
        /// Tries to add given pair of objects to the picture. If there already is an equal analytic object, 
        /// the <paramref name="equalObject"/> will be set to it and the object won't be added. Otherwise it 
        /// will be added and the <paramref name="equalObject"/> will have its default value (which is null).
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        /// <param name="analyticObject">The analytic object.</param>
        /// <param name="equalObject">Either the equal configuration object already present in the picture; or null, if there's none.</param>
        internal void TryAdd(ConfigurationObject configurationObject, IAnalyticObject analyticObject, out ConfigurationObject equalObject)
        {
            // Check if the types of objects correspond. This could save us some time while finding weird errors
            if (_correctTypes[analyticObject.GetType()] != configurationObject.ObjectType)
                throw new ConstructorException("Can't add objects of wrong types to the picture.");

            // Check if we don't have a duplicity 
            if (_content.ContainsRightKey(analyticObject))
            {
                // If yes, set it
                equalObject = _content.GetLeftValue(analyticObject);

                // And terminate
                return;
            }

            // Now we can safely add them 
            _content.Add(configurationObject, analyticObject);

            // And set that there is not equal object
            equalObject = default;
        }

        /// <summary>
        /// Marks that a given object that is already present in the container has a given equal object. 
        /// This will affect queering for analytic version of an object via <see cref="Get(IAnalyticObject)"/> calls.
        /// </summary>
        /// <param name="existingObject">The object already present in the container.</param>
        /// <param name="equalObject">The equal object.</param>
        internal void MarkDuplicate(ConfigurationObject existingObject, ConfigurationObject equalObject)
            // Mark the equality in the equal objects dictionary
            => _equalObjects.Add(equalObject, existingObject);

        #endregion

        #region IEnumerable implementation

        /// <inheritdoc/>
        public IEnumerator<(ConfigurationObject, IAnalyticObject)> GetEnumerator() =>
            // Take the content
            _content
            // As well as equal objects
            .Concat(_equalObjects.Select(pair => (pair.Key, _content.GetRightValue(pair.Value))))
            // Return the enumerator
            .GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }
}