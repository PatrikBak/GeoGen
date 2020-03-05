using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using static GeoGen.Core.LooseObjectLayout;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a wrapper of <see cref="LooseConfigurationObject"/>s together with their <see cref="LooseObjectLayout"/>.
    /// </summary>
    public class LooseObjectHolder
    {
        #region Public properties

        /// <summary>
        /// Gets the list of actual loose configurations objects held by this instance.
        /// </summary>
        public IReadOnlyList<LooseConfigurationObject> LooseObjects { get; }

        /// <summary>
        /// Gets the loose objects wrapped in an objects map.
        /// </summary>
        public ConfigurationObjectMap ObjectMap { get; }

        /// <summary>
        /// Gets the layout in which these objects are arranged.
        /// </summary>
        public LooseObjectLayout Layout { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="LooseObjectHolder"/> 
        /// instance wrapping the actual loose objects and possible their layout.
        /// </summary>
        /// <param name="looseObjects">The actual loose configurations objects.</param>
        /// <param name="layout">The layout of these loose objects.</param>
        public LooseObjectHolder(IEnumerable<LooseConfigurationObject> looseObjects, LooseObjectLayout layout)
        {
            LooseObjects = looseObjects?.ToList() ?? throw new ArgumentNullException(nameof(looseObjects));
            ObjectMap = new ConfigurationObjectMap(looseObjects);
            Layout = layout;

            // Make sure the objects match the layout
            if (!LooseObjects.Select(looseObject => looseObject.ObjectType).SequenceEqual(layout.ObjectTypes()))
                throw new GeoGenException($"The loose objects don't match the specified layout {layout}.");

            // Make sure they are distinct 
            if (LooseObjects.AnyDuplicates())
                throw new GeoGenException("The loose objects are not distinct.");
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Finds all mappings of loose objects to themselves that represent geometrically equivalent
        /// layout. These mappings include the identical mapping. For example, if we have a triangle ABC,
        /// i.e. the loose objects are A, B, C, then there are 6 possible mappings ([A, B, C], [A, C, B],...)
        /// yielding a symmetric layout.
        /// </summary>
        /// <returns>The dictionaries mapping the current loose objects to themselves.</returns>
        public IEnumerable<IReadOnlyDictionary<LooseConfigurationObject, LooseConfigurationObject>> GetSymmetryMappings()
        {
            // Switch based on the layout
            switch (Layout)
            {
                // Cases where any permutation works
                case LineSegment:
                case Triangle:
                case Quadrilateral:
                case CyclicQuadrilateral:

                    // Take all permutations and zip them with the objects
                    return LooseObjects.Permutations().Select(LooseObjects.ZipToDictionary);

                // Case where any permutation of the second two objects work
                case RightTriangle:
                case LineAndTwoPoints:

                    // Take all permutations and zip them with the objects
                    return new[]
                    {
                        // Identity
                        new Dictionary<LooseConfigurationObject,LooseConfigurationObject>
                        {
                            { LooseObjects[0], LooseObjects[0] },
                            { LooseObjects[1], LooseObjects[1] },
                            { LooseObjects[2], LooseObjects[2] }
                        },

                        // Changed the second two points
                        new Dictionary<LooseConfigurationObject,LooseConfigurationObject>
                        {
                            { LooseObjects[0], LooseObjects[0] },
                            { LooseObjects[1], LooseObjects[2] },
                            { LooseObjects[2], LooseObjects[1] }
                        }
                    };

                // Case where there is only an identity
                case LineAndPoint:

                    // Return just an identity dictionary
                    return LooseObjects.ToDictionary(_ => _, _ => _).ToEnumerable();

                // Unhandled cases
                default:
                    throw new GeoGenException($"Unhandled value of {nameof(LooseObjectLayout)}: {Layout}");
            }
        }

        #endregion

        #region HashCode and Equals

        /// <inheritdoc/>
        public override int GetHashCode() => (Layout, LooseObjects.GetHashCodeOfList()).GetHashCode();

        /// <inheritdoc/>
        public override bool Equals(object otherObject)
            // Either the references are equals
            => this == otherObject
                // Or the object is not null
                || otherObject != null
                // And is a loose object holder
                && otherObject is LooseObjectHolder holder
                // And the layouts are equal
                && holder.Layout.Equals(Layout)
                // And the loose objects are equal
                && holder.LooseObjects.SequenceEqual(LooseObjects);

        #endregion

        #region Debug-only to string

#if DEBUG

        /// <inheritdoc/>
        public override string ToString() => $"{Layout}({LooseObjects.Select(looseObject => looseObject.Id).ToJoinedString()})";

#endif

        #endregion
    }
}