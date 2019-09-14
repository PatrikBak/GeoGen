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
            // Remap the objects
            var objects = RemapObjects(mapping);

            // Reconstruct based on the fact whether remapping could be done
            return objects != default ? new AngleTheoremObject((LineTheoremObject)objects.Item1, (LineTheoremObject)objects.Item2) : null;
        }

        #endregion
    }
}
