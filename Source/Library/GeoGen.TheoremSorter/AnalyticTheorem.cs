using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Utilities;
using System.Linq;

namespace GeoGen.TheoremSorter
{
    /// <summary>
    /// Represents a helper class that 'encodes' a <see cref="Theorem"/> and <see cref="Picture"/>
    /// into comparable content so that we can find equal analytically equal theorems. This class therefore
    /// correctly implement <see cref="Equals(object)"/> and <see cref="GetHashCode"/>.
    /// </summary>
    internal class AnalyticTheorem
    {
        #region Private fields

        /// <summary>
        /// The type of the encoded theorem.
        /// </summary>
        private readonly TheoremType _type;

        /// <summary>
        /// The encoded analytic content that correctly implement hash code and equals.
        /// </summary>
        private readonly object _content;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalyticTheorem"/> class.
        /// </summary>
        /// <param name="theorem">The theorem that should be drawn analytically.</param>
        /// <param name="picture">The picture where the inner objects of the theorem are drawn.</param>
        public AnalyticTheorem(Theorem theorem, Picture picture)
        {
            // Set the type
            _type = theorem.Type;

            // Set the content by switching on the type
            _content = _type switch
            {
                // These types can be compared as object sets
                TheoremType.CollinearPoints or
                TheoremType.ConcyclicPoints or
                TheoremType.ConcurrentLines or
                TheoremType.ParallelLines or
                TheoremType.PerpendicularLines or
                TheoremType.TangentCircles or
                TheoremType.LineTangentToCircle or
                TheoremType.Incidence =>
                            // Take the inner objects
                            theorem.InvolvedObjects
                                    // We know they are base
                                    .Cast<BaseTheoremObject>()
                                    // And therefore constructible
                                    .Select(picture.Construct)
                                    // Enumerate them to a read-only hash set
                                    .ToReadOnlyHashSet(),

                // In this case we have two line segment objects
                TheoremType.EqualLineSegments =>
                            // Take the inner objects
                            theorem.InvolvedObjects
                                    // We know they are line segments
                                    .Cast<LineSegmentTheoremObject>()
                                    // From each create their point set 
                                    .Select(segment => segment.PointSet.Select(picture.Construct).ToReadOnlyHashSet())
                                    // Enumerate them to a read-only hash set
                                    .ToReadOnlyHashSet(),

                // Unhandled cases
                _ => throw new TheoremSorterException($"Unhandled value of {nameof(TheoremType)}: {_type}"),
            };
        }

        #endregion

        #region HashCode and Equals

        /// <inheritdoc/>
        public override int GetHashCode() => (_type, _content).GetHashCode();

        /// <inheritdoc/>
        public override bool Equals(object otherObject)
            // Either the references are equals
            => this == otherObject
                // Or the object is not null
                || otherObject != null
                // And is an AnalyticTheorem object
                && otherObject is AnalyticTheorem analyticTheorem
                // And their types are equal
                && analyticTheorem._type.Equals(_type)
                // And their contents are equal
                && analyticTheorem._content.Equals(_content);

        #endregion
    }
}
