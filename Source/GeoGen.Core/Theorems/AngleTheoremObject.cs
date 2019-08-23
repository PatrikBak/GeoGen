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
        /// <param name="object1">The first line of the angle.</param>
        /// <param name="object2">The second line of the angle.</param>
        public AngleTheoremObject(LineTheoremObject object1, LineTheoremObject object2)
            : base(object1, object2)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AngleTheoremObject"/> class
        /// using two lines specified by points.
        /// <param name="firstLinePoints">The points of the first line.</param>
        /// <param name="secondLinePoints">The points of the second line.</param>
        public AngleTheoremObject(IEnumerable<ConfigurationObject> firstLinePoints, IEnumerable<ConfigurationObject> secondLinePoints)
            : this(new LineTheoremObject(configurationObject: null, firstLinePoints), new LineTheoremObject(configurationObject: null, secondLinePoints))
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

        #region HashCode and Equals

        /// <summary>
        /// Gets the hash code of this object.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            // I don't think there is a better way to define it, see the comment to the 
            // GetHashCode method in the LineTheoremObject or CircleTheoremObject classes
            return "Angle".GetHashCode();
        }

        /// <summary>
        /// Finds out if a passed object is equal to this one.
        /// </summary>
        /// <param name="otherObject">The passed object.</param>
        /// <returns>true, if they are equal; false otherwise.</returns>
        public override bool Equals(object otherObject)
        {
            // Either the references are equals
            return this == otherObject
                // Or the object is not null
                || otherObject != null
                // And it is an angle object
                && otherObject is AngleTheoremObject angle
                // And either the first and second object are equal
                && ((Object1.Equals(angle.Object1) && Object2.Equals(angle.Object2))
                // Or the first one is equal to the second and vice versa
                || (Object1.Equals(angle.Object2) && Object2.Equals(angle.Object1)));
        }

        #endregion
    }
}
