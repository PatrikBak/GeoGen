using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents a geometrical circle that is aware of the points on it.
    /// </summary>
    public class CircleObject : DefinableByPoints
    {
        #region DefineableByPoints properties

        /// <summary>
        /// Gets the minimal number of distinct points that are needed to define the object.
        /// </summary>
        public override int NumberOfNeededPoints { get; } = 3;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor used for when we simply wrap a given configuration object.
        /// </summary>
        /// <param name="id">The id of the object.</param>
        /// <param name="configurationObject">The configuration object.</param>
        public CircleObject(int id, ConfigurationObject configurationObject)
                : base(id, configurationObject)
        {
        }

        /// <summary>
        /// Constructor used when we define object by points.
        /// </summary>
        /// <param name="id">The id of the object.</param>
        /// <param name="points">The points.</param>
        public CircleObject(int id, params PointObject[] points)
                : base(id, points)
        {
        } 

        #endregion
    }
}