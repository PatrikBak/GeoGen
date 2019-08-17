using GeoGen.Core;
using System.Collections.Generic;

namespace GeoGen.Constructor
{
    /// <summary>
    /// Represents a geometric line that can be defined by two <see cref="PointObject"/>. 
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
        /// Initialize a new instance of the <see cref="LineObject"/> class.
        /// </summary>
        /// <param name="points">The points that define this line.</param>
        public LineObject(params PointObject[] points)
                : base(configurationObject: null, points)
        {
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="LineObject"/> class.
        /// </summary>
        /// <param name="configurationObject">The configuration object represented by this line.</param>
        /// <param name="points">The points that define this line.</param>
        public LineObject(ConfigurationObject configurationObject, IEnumerable<PointObject> points)
                : base(configurationObject, points)
        {
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="LineObject"/> class.
        /// </summary>
        /// <param name="configurationObject">The configuration object represented by this line.</param>
        public LineObject(ConfigurationObject configurationObject)
                : base(configurationObject, new PointObject[0])
        {
        }

        #endregion

        #region To String

        /// <summary>
        /// Converts a given object to a string. 
        /// NOTE: This method is used only for debugging purposes.
        /// </summary>
        /// <returns>A human-readable string representation of the object.</returns>
        public override string ToString() => $"Line: {base.ToString()}";

        #endregion
    }
}