using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Constructor
{
    /// <summary>
    /// Represents a geometric circle that can be defined by three <see cref="PointObject"/>. 
    /// It contains all the <see cref="PointObject"/>s that lie on it.
    /// </summary>
    public class CircleObject : DefinableByPoints
    {
        #region DefineableByPoints properties

        /// <summary>
        /// Gets the minimal number of distinct points that are needed to define this type of object.
        /// </summary>
        public override int NumberOfNeededPoints => 3;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a new instance of the <see cref="CircleObject"/> class.
        /// </summary>
        /// <param name="points">The points that define this circle.</param>
        public CircleObject(params PointObject[] points)
                : base(configurationObject: null, points)
        {
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="CircleObject"/> class.
        /// </summary>
        /// <param name="configurationObject">The configuration object represented by this circle.</param>
        /// <param name="points">The points that define this circle.</param>
        public CircleObject(ConfigurationObject configurationObject, IEnumerable<PointObject> points)
                : base(configurationObject, points)
        {
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="CircleObject"/> class.
        /// </summary>
        /// <param name="configurationObject">The configuration object represented by this circle.</param>
        public CircleObject(ConfigurationObject configurationObject)
                : base(configurationObject, Array.Empty<PointObject>())
        {
        }

        #endregion

        #region To String

        /// <summary>
        /// Converts the line object to a string. 
        /// NOTE: This method is used only for debugging purposes.
        /// </summary>
        /// <returns>A human-readable string representation of the configuration.</returns>
        public override string ToString()
        {
            // If there is a specific configuration object, we include it 
            var objectPart = ConfigurationObject == null ? "" : $"{ConfigurationObject.Id}";

            // If there are points, include them
            var pointsPart = Points.Any() ? $"({Points.Select(p => p.ToString()).Ordered().ToJoinedString()})" : "";

            // Construct the final string including the points
            return $"{objectPart}{pointsPart}";
        }

        #endregion
    }
}