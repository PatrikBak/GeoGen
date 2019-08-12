using GeoGen.Core;
using System.Collections.Generic;

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
                : base(configurationObject:null, points)
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
                : base(configurationObject, new PointObject[0])
        {
        }

        #endregion
    }
}