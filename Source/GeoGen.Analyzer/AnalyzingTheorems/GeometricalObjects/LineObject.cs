using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents a geometrical line that can be defined by two <see cref="PointObject"/>. 
    /// It contains all the <see cref="PointObject"/>s that lie on it.
    /// </summary>
    public class LineObject : DefinableByPoints
    {
        #region DefineableByPoints properties

        /// <summary>
        /// Gets the minimal number of distinct points that are needed to define this type of object.
        /// </summary>
        public override int NumberOfNeededPoints => 2;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a new instance of the <see cref="LineObject"/> class wrapping a given line <see cref="ConfigurationObject"/>.
        /// </summary>
        /// <param name="configurationObject">The line configuration object represented by this geometrical object.</param>
        public LineObject(ConfigurationObject configurationObject)
                : base(configurationObject)
        {
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="LineObject"/> class by at least 2 <see cref="PointObject"/>s that it passes through.
        /// </summary>
        /// <param name="points">The points that define this line.</param>
        public LineObject(params PointObject[] points)
                : base(points)
        {
        }

        #endregion
    }
}