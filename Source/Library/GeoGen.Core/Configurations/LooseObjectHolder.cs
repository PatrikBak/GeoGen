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
        /// Finds all mappings of loose objects to themselves that represent layout symmetry, which
        /// should represent this idea: We want to be able to take two points and place them horizontally
        /// so that the picture looks 'symmetric' along the vertical direction. 
        /// <list type="number">
        /// <item><see cref="LineSegment"/>: AB has one symmetry remapping AB --> BA.</item>
        /// <item><see cref="LineAndPoint"/>: has no symmetry remapping.</item>
        /// <item><see cref="RightTriangle"/>: ABC has one symmetry remapping BC --> CB 
        /// (the right angle is at A and BC is placed horizontally)</item>
        /// <item><see cref="LineAndTwoPoints"/>One remapping of the points.</item>
        /// <item><see cref="Triangle"/>: ABC has three remapping: ABC --> ACB, ABC --> CBA, ABC --> BAC.</item>
        /// <item><see cref="Quadrilateral"/> and <see cref="CyclicQuadrilateral"/> are the most complicated ones.
        /// A permutation of its points works if and only if we can find two points that changed their order.
        /// These points are then the ones to be placed horizontally. 
        /// </item>
        /// </list>
        /// </summary>
        /// <returns>The dictionaries mapping the current loose objects to themselves.</returns>
        public IEnumerable<IReadOnlyDictionary<LooseConfigurationObject, LooseConfigurationObject>> GetSymmetricMappings()
        {
            // Switch based on the layout
            switch (Layout)
            {
                // Case where we exchange the points
                case LineSegment:

                    // Do the exchange
                    return new Dictionary<LooseConfigurationObject, LooseConfigurationObject>
                        {
                            { LooseObjects[0], LooseObjects[1] },
                            { LooseObjects[1], LooseObjects[0] }
                        }
                        // We need enumerable
                        .ToEnumerable();

                // Case where we exchange any two points
                case Triangle:

                    // Return all 3 options
                    return new[]
                    {
                        // 01 --> 10
                        new Dictionary<LooseConfigurationObject,LooseConfigurationObject>
                        {
                            { LooseObjects[0], LooseObjects[1] },
                            { LooseObjects[1], LooseObjects[0] },
                            { LooseObjects[2], LooseObjects[2] }
                        },

                        // 02 --> 20
                        new Dictionary<LooseConfigurationObject,LooseConfigurationObject>
                        {
                            { LooseObjects[0], LooseObjects[2] },
                            { LooseObjects[1], LooseObjects[1] },
                            { LooseObjects[2], LooseObjects[0] }
                        },

                        // 12 --> 21
                        new Dictionary<LooseConfigurationObject,LooseConfigurationObject>
                        {
                            { LooseObjects[0], LooseObjects[0] },
                            { LooseObjects[1], LooseObjects[2] },
                            { LooseObjects[2], LooseObjects[1] }
                        }
                    };

                // Cases where we have 2 fixed or exchanged points
                case Quadrilateral:
                case CyclicQuadrilateral:

                    // Return all 9 options
                    return new[]
                    {
                        // 01 --> 10 and 23 --> 23
                        new Dictionary<LooseConfigurationObject,LooseConfigurationObject>
                        {
                            { LooseObjects[0], LooseObjects[1] },
                            { LooseObjects[1], LooseObjects[0] },
                            { LooseObjects[2], LooseObjects[2] },
                            { LooseObjects[3], LooseObjects[3] }
                        },

                        // 01 --> 10 and 23 --> 32
                        new Dictionary<LooseConfigurationObject,LooseConfigurationObject>
                        {
                            { LooseObjects[0], LooseObjects[1] },
                            { LooseObjects[1], LooseObjects[0] },
                            { LooseObjects[2], LooseObjects[3] },
                            { LooseObjects[3], LooseObjects[2] }
                        },

                        // 02 --> 20 and 13 --> 13
                        new Dictionary<LooseConfigurationObject,LooseConfigurationObject>
                        {
                            { LooseObjects[0], LooseObjects[2] },
                            { LooseObjects[1], LooseObjects[1] },
                            { LooseObjects[2], LooseObjects[0] },
                            { LooseObjects[3], LooseObjects[3] }
                        },

                        // 02 --> 20 and 13 --> 31
                        new Dictionary<LooseConfigurationObject,LooseConfigurationObject>
                        {
                            { LooseObjects[0], LooseObjects[2] },
                            { LooseObjects[1], LooseObjects[3] },
                            { LooseObjects[2], LooseObjects[0] },
                            { LooseObjects[3], LooseObjects[1] }
                        },
                                     
                        // 03 --> 30 and 12 --> 12
                        new Dictionary<LooseConfigurationObject,LooseConfigurationObject>
                        {
                            { LooseObjects[0], LooseObjects[3] },
                            { LooseObjects[1], LooseObjects[1] },
                            { LooseObjects[2], LooseObjects[2] },
                            { LooseObjects[3], LooseObjects[0] }
                        },

                        // 03 --> 30 and 12 --> 21
                        new Dictionary<LooseConfigurationObject,LooseConfigurationObject>
                        {
                            { LooseObjects[0], LooseObjects[3] },
                            { LooseObjects[1], LooseObjects[2] },
                            { LooseObjects[2], LooseObjects[1] },
                            { LooseObjects[3], LooseObjects[0] }
                        },
                                     
                        // 12 --> 21 and 03 --> 03
                        new Dictionary<LooseConfigurationObject,LooseConfigurationObject>
                        {
                            { LooseObjects[0], LooseObjects[0] },
                            { LooseObjects[1], LooseObjects[2] },
                            { LooseObjects[2], LooseObjects[1] },
                            { LooseObjects[3], LooseObjects[3] }
                        },
                                     
                        // 13 --> 31 and 02 --> 02
                        new Dictionary<LooseConfigurationObject,LooseConfigurationObject>
                        {
                            { LooseObjects[0], LooseObjects[0] },
                            { LooseObjects[1], LooseObjects[3] },
                            { LooseObjects[2], LooseObjects[2] },
                            { LooseObjects[3], LooseObjects[1] }
                        },

                        // 23 --> 32 and 01 --> 01
                        new Dictionary<LooseConfigurationObject,LooseConfigurationObject>
                        {
                            { LooseObjects[0], LooseObjects[0] },
                            { LooseObjects[1], LooseObjects[1] },
                            { LooseObjects[2], LooseObjects[3] },
                            { LooseObjects[3], LooseObjects[2] }
                        },
                    };

                // Case where we can exchange the second two points
                case RightTriangle:
                case LineAndTwoPoints:

                    // Change the second two points
                    return new Dictionary<LooseConfigurationObject, LooseConfigurationObject>
                        {
                            { LooseObjects[0], LooseObjects[0] },
                            { LooseObjects[1], LooseObjects[2] },
                            { LooseObjects[2], LooseObjects[1] }
                        }
                        // We need enumerable
                        .ToEnumerable();

                // Case where there is no option
                case LineAndPoint:
                    return Enumerable.Empty<IReadOnlyDictionary<LooseConfigurationObject, LooseConfigurationObject>>();

                // Unhandled cases
                default:
                    throw new GeoGenException($"Unhandled value of {nameof(LooseObjectLayout)}: {Layout}");
            }
        }

        /// <summary>
        /// Finds all mappings of loose objects to themselves that represent geometrically equivalent
        /// layout. These mappings include the identical mapping. For example, if we have a triangle ABC,
        /// i.e. the loose objects are A, B, C, then there are 6 possible mappings ([A, B, C], [A, C, B],...)
        /// yielding an equivalent layout.
        /// </summary>
        /// <returns>The dictionaries mapping the current loose objects to themselves.</returns>
        public IEnumerable<IReadOnlyDictionary<LooseConfigurationObject, LooseConfigurationObject>> GetIsomorphicMappings()
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

                    // Return all 2 options
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

                    // Return the identity
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