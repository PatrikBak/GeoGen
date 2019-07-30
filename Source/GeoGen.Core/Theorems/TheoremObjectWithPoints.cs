using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a <see cref="TheoremObject"/> that might be defined by points, i.e. a line or a circle.
    /// </summary>
    public class TheoremObjectWithPoints : TheoremObject
    {
        #region Public properties

        /// <summary>
        /// Gets the points configuration objects that might be contained in this object,
        /// if it's a line or a circle.
        /// </summary>
        public HashSet<ConfigurationObject> Points { get; }

        /// <summary>
        /// Gets the minimal number of distinct points that are needed to define this type of object.
        /// </summary>
        public int NumberOfNeededPoints { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremObjectWithPoints"/> class.
        /// </summary>
        /// <param name="objectType">The type of the object, which should be either a line or a circle.</param>
        /// <param name="configurationObject">The optional configuration object representing this theorem object.</param>
        /// <param name="points">The optional points that define / lie on this object.</param>
        public TheoremObjectWithPoints(ConfigurationObjectType objectType, ConfigurationObject configurationObject, IEnumerable<ConfigurationObject> points = null)
            : base(objectType, configurationObject)
        {
            Points = points?.ToSet() ?? new HashSet<ConfigurationObject>();

            // Make sure the object is not a point
            if (objectType == ConfigurationObjectType.Point)
                throw new GeoGenException("The object type cannot be a point");

            // Find the number of needed points to define this object
            // We're using that if we don't have a line, then we must have a circle
            NumberOfNeededPoints = objectType == ConfigurationObjectType.Line ? 2 : 3;

            // Make sure we have enough points in the case where the object is not defined
            if (configurationObject == null && NumberOfNeededPoints > Points.Count)
                throw new GeoGenException("There are not enough distinct points to define this object.");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremObjectWithPoints"/> class
        /// representing an object defined by points.
        /// </summary>
        /// <param name="objectType">The type of the object.</param>
        /// <param name="points">The points that should define this object.</param>
        public TheoremObjectWithPoints(ConfigurationObjectType objectType, params ConfigurationObject[] points)
            : this(objectType, configurationObject: null, points)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremObjectWithPoints"/> class
        /// representing an object defined by a given configuration object.
        /// </summary>
        /// <param name="configurationObject">The configuration object defining this theorem object.</param>
        public TheoremObjectWithPoints(ConfigurationObject configurationObject)
            : this(configurationObject.ObjectType, configurationObject)
        {
        }

        #endregion

        #region To String

        /// <summary>
        /// Converts the theorem object with points to a string. 
        /// NOTE: This method is used only for debugging purposes.
        /// </summary>
        /// <returns>A human-readable string representation of the configuration.</returns>
        public override string ToString()
        {
            // If there is a specific configuration object, we include it 
            var objectPart = ConfigurationObject == null ? " " : $" object: {ConfigurationObject.Id}, ";

            // Construct the final string including the ids of the points
            return $"{Type}{objectPart}{Points.Select(p => p.Id).ToJoinedString("--")}";
        }

        #endregion
    }
}