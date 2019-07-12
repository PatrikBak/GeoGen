using GeoGen.Core;

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
        /// Initialize a new instance of the <see cref="CircleObject"/> class wrapping a given circle <see cref="ConfigurationObject"/>.
        /// </summary>
        /// <param name="configurationObject">The circle configuration object represented by this geometric object.</param>
        public CircleObject(ConfigurationObject configurationObject)
                : base(configurationObject)
        {
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="CircleObject"/> class by at least 3 <see cref="PointObject"/>s that it passes through.
        /// </summary>
        /// <param name="points">The points that define this circle.</param>
        public CircleObject(params PointObject[] points)
                : base(points)
        {
        }

        #endregion
    }
}