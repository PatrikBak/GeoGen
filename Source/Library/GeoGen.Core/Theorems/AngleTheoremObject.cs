using System.Collections.Generic;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents an angle defined by two <see cref="LineTheoremObject"/>s.
    /// </summary>
    public class AngleTheoremObject : PairTheoremObject
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AngleTheoremObject"/> class.
        /// </summary>
        /// <param name="line1">The first line of the angle.</param>
        /// <param name="line2">The second line of the angle.</param>
        public AngleTheoremObject(LineTheoremObject line1, LineTheoremObject line2)
            : base(line1, line2)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AngleTheoremObject"/> class
        /// using two lines specified by points.
        /// </summary>
        /// <param name="line1Point1">A point of the first line.</param>
        /// <param name="line1Point2">A point of the first line.</param>
        /// <param name="line2Point1">A point of the second line.</param>
        /// <param name="line2Point2">A point of the second line.</param>
        public AngleTheoremObject(ConfigurationObject line1Point1, ConfigurationObject line1Point2,
                                  ConfigurationObject line2Point1, ConfigurationObject line2Point2)
            : this(new LineTheoremObject(line1Point1, line1Point2), new LineTheoremObject(line2Point1, line2Point2))
        {
        }

        #endregion

        #region Remap implementation

        /// <inheritdoc/>
        public override TheoremObject Remap(IReadOnlyDictionary<ConfigurationObject, ConfigurationObject> mapping, bool flattenObjectsFromPoints = false)
        {
            // Remap the objects
            var objects = RemapObjects(mapping, flattenObjectsFromPoints);

            // Reconstruct based on the fact whether remapping could be done
            return objects != default ? new AngleTheoremObject((LineTheoremObject)objects.Item1, (LineTheoremObject)objects.Item2) : null;
        }

        #endregion
    }
}
