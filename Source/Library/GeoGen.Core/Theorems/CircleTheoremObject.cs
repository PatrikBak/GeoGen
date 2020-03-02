using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a <see cref="TheoremObjectWithPoints"/> that is a circle.
    /// </summary>
    public class CircleTheoremObject : TheoremObjectWithPoints
    {
        #region Public abstract properties implementation

        /// <inheritdoc/>
        public override int NumberOfDefiningPoints => 3;

        /// <inheritdoc/>
        public override ConfigurationObjectType Type => ConfigurationObjectType.Circle;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CircleTheoremObject"/> class
        /// defined by a circle configuration object.
        /// </summary>
        /// <param name="circleObject">The configuration circle object representing this theorem object.</param>
        public CircleTheoremObject(ConfigurationObject circleObject)
            : base(circleObject)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CircleTheoremObject"/> 
        /// class defined by points.
        /// </summary>        
        /// <param name="point1">A point of the circle.</param>
        /// <param name="point2">A point of the circle.</param>
        /// <param name="point3">A point of the circle.</param>
        public CircleTheoremObject(ConfigurationObject point1, ConfigurationObject point2, ConfigurationObject point3)
            : base(point1, point2, point3)
        {
        }

        #endregion

        #region Public abstract methods implementation

        /// <inheritdoc/>
        public override TheoremObject Remap(IReadOnlyDictionary<ConfigurationObject, ConfigurationObject> mapping, bool flattenObjectsFromPoints = false)
        {
            // Remap object and points
            var objectPoints = RemapObjectAndPoints(mapping);

            // If it cannot be done, return null
            if (objectPoints == default)
                return null;

            // If this is defined by points, use the points constructor
            if (DefinedByPoints)
                return new CircleTheoremObject(objectPoints.points[0], objectPoints.points[1], objectPoints.points[2]);

            // Otherwise this is defined via an explicit object
            // Find out if the explicit object is a circle from points, which is true if it is constructed
            var isThisCircleFromPoints = objectPoints.explicitObject is ConstructedConfigurationObject constructedObject
                // And the construction is predefined
                && constructedObject.Construction is PredefinedConstruction predefinedConstruction
                // With the type equal to Circumcircle
                && predefinedConstruction.Type == PredefinedConstructionType.Circumcircle;

            // If this is a circle from points and it should be made explicit, do it
            if (isThisCircleFromPoints && flattenObjectsFromPoints)
            {
                // Get the points
                var points = ((ConstructedConfigurationObject)objectPoints.explicitObject).PassedArguments.FlattenedList;

                // And use them to map this explicit object to an implicit one with points
                return new CircleTheoremObject(points[0], points[1], points[2]);
            }

            // Otherwise simply wrap the explicit object
            return new CircleTheoremObject(objectPoints.explicitObject);
        }

        #endregion

        #region Debug-only to string

#if DEBUG

        /// <inheritdoc/>
        public override string ToString()
        {
            // If the object is defined by a specific configuration object, we return its id
            if (DefinedByExplicitObject)
                return ConfigurationObject.Id.ToString();

            // Otherwise it's defined by points
            return $"({Points.Select(point => point.Id).Ordered().ToJoinedString()})";
        }

#endif

        #endregion
    }
}