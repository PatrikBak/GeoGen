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

        /// <summary>
        /// Gets the number of points that might define this type of object.
        /// </summary>
        public override int NumberOfDefiningPoints => 3;

        /// <summary>
        /// The type of configuration object this theorem objects represents.
        /// </summary>
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

        /// <summary>
        /// Recreates the theorem object by applying a given mapping of the inner configuration objects.
        /// Every <see cref="ConfigurationObject"/> internally contained in this theorem object must be
        /// present in the mapping. If the mapping cannot be done (for example because 2 points
        /// making a line are mapped to the same point), then null is returned.
        /// </summary>
        /// <param name="mapping">The dictionary representing the mapping.</param>
        /// <returns>The remapped theorem object, or null, if the mapping cannot be done.</returns>
        public override TheoremObject Remap(Dictionary<ConfigurationObject, ConfigurationObject> mapping)
        {
            // Remap object and points
            var objectPoints = RemapObjectAndPoints(mapping);

            // If it cannot be done, return null
            if (objectPoints == default)
                return null;

            // If this is defined by an object, use the object constructor
            if (DefinedByExplicitObject)
                return new CircleTheoremObject(objectPoints.explicitObject);

            // Otherwise use the points constructor
            return new CircleTheoremObject(objectPoints.points[0], objectPoints.points[1], objectPoints.points[2]);
        }

        #endregion

        #region To String

        /// <summary>
        /// Converts the circle theorem object to a string. 
        /// NOTE: This method is used only for debugging purposes.
        /// </summary>
        /// <returns>A human-readable string representation of the configuration.</returns>
        public override string ToString()
        {
            // If the object is defined by a specific configuration object, we return its id
            if (DefinedByExplicitObject)
                return ConfigurationObject.Id.ToString();

            // Otherwise it's defined by points
            return $"({Points.Select(p => p.Id).Ordered().ToJoinedString()})";
        }

        #endregion    
    }
}