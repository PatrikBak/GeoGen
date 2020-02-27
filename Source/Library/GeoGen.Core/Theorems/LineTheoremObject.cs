using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a <see cref="TheoremObjectWithPoints"/> that is a line.
    /// </summary>
    public class LineTheoremObject : TheoremObjectWithPoints
    {
        #region Public abstract properties implementation

        /// <summary>
        /// Gets the number of points that might define this type of object.
        /// </summary>
        public override int NumberOfDefiningPoints => 2;

        /// <summary>
        /// The type of configuration object this theorem objects represents.
        /// </summary>
        public override ConfigurationObjectType Type => ConfigurationObjectType.Line;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="LineTheoremObject"/> class
        /// defined by a line configuration object.
        /// </summary>
        /// <param name="lineObject">The configuration line object representing this theorem object.</param>
        public LineTheoremObject(ConfigurationObject lineObject)
            : base(lineObject)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineTheoremObject"/> 
        /// class defined by points.
        /// </summary>        
        /// <param name="point1">A point of the line.</param>
        /// <param name="point2">A point of the line.</param>
        public LineTheoremObject(ConfigurationObject point1, ConfigurationObject point2)
            : base(point1, point2)
        {
        }

        #endregion

        #region Public abstract methods implementation

        /// <summary>
        /// Recreates the theorem object by applying a given mapping of the inner configuration objects.
        /// Every <see cref="ConfigurationObject"/> internally contained in this theorem object must be
        /// present in the mapping. If the mapping cannot be done (for example because 2 points
        /// making a line are mapped to the same point), then null is returned.
        /// </summary>
        /// <param name="mapping">The dictionary representing the mapping.</param>
        /// <param name="flattenObjectsFromPoints">Indicates whether explicit objects LineFromPoints or Circumcircle should be made implicit.</param>
        /// <returns>The remapped theorem object, or null, if the mapping cannot be done.</returns>
        public override TheoremObject Remap(IReadOnlyDictionary<ConfigurationObject, ConfigurationObject> mapping, bool flattenObjectsFromPoints = false)
        {
            // Remap object and points
            var objectPoints = RemapObjectAndPoints(mapping);

            // If it cannot be done, return null
            if (objectPoints == default)
                return null;

            // If this is defined by points, use the points constructor
            if (DefinedByPoints)
                return new LineTheoremObject(objectPoints.points[0], objectPoints.points[1]);

            // Otherwise this is defined via an explicit object
            // Find out if the explicit object is a line from points, which is true if it is constructed
            var isThisLineFromPoints = objectPoints.explicitObject is ConstructedConfigurationObject constructedObject
                // And the construction is predefined
                && constructedObject.Construction is PredefinedConstruction predefinedConstruction
                // With the type equal to LineFromPoints
                && predefinedConstruction.Type == PredefinedConstructionType.LineFromPoints;

            // If this is a line from points and it should be made explicit, do it
            if (isThisLineFromPoints && flattenObjectsFromPoints)
            {
                // Then get the points
                var points = ((ConstructedConfigurationObject)objectPoints.explicitObject).PassedArguments.FlattenedList;

                // And use them to map this explicit object to an implicit one with points
                return new LineTheoremObject(points[0], points[1]);
            }

            // Otherwise simply wrap the explicit object
            return new LineTheoremObject(objectPoints.explicitObject);
        }

        #endregion

        #region Debug-only to string

#if DEBUG

        /// <summary>
        /// Converts the line theorem object to a string. 
        /// </summary>
        /// <returns>A human-readable string representation of the configuration.</returns>
        public override string ToString()
        {
            // If the object is defined by a specific configuration object, we return its id
            if (DefinedByExplicitObject)
                return ConfigurationObject.Id.ToString();

            // Otherwise it's defined by points
            return $"[{Points.Select(point => point.Id).Ordered().ToJoinedString()}]";
        }

#endif

        #endregion
    }
}
