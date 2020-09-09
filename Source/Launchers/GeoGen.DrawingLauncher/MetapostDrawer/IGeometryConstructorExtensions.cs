using GeoGen.AnalyticGeometry;
using GeoGen.Constructor;
using GeoGen.Core;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.DrawingLauncher
{
    /// <summary>
    /// The extensions methods for <see cref="IGeometryConstructor"/>.
    /// </summary>
    public static class IGeometryConstructorExtensions
    {
        /// <summary>
        /// Constructs a given <see cref="Configuration"/> to a given number of pictures, using the 
        /// loose object drawing that is in some cases more flexible than the drawing from  the method
        /// <see cref="LooseObjectLayoutDrawing.ConstructUniformLayout(LooseObjectLayout)"/>. These cases
        /// are two: Triangle, when <see cref="RandomLayoutsHelpers.ConstructRandomTriangle"/> is used, 
        /// and Quadrilateral, when <see cref="RandomLayoutsHelpers.ConstructRandomConvexQuadrilateralWithHorizontalSide"/>
        /// or <see cref="RandomLayoutsHelpers.ConstructRandomConvexQuadrilateralWithHorizontalDiagonal"/>
        /// if the picture is symmetric in such a way that it would look better with a horizontal diagonal,
        /// or <see cref="RandomLayoutsHelpers.ConstructRandomConvexQuadrilateralWithHorizontalSide"/> otherwise.
        /// </summary>
        /// <param name="configuration">The configuration to be constructed.</param>
        /// <param name="theorem">The theorem that is used to determine symmetry be constructed.</param>
        /// <param name="numberOfPictures">The number of <see cref="Picture"/>s where the configuration should be drawn.</param>
        /// <returns>The tuple consisting of the pictures and the construction data.</returns>
        /// <exception cref="ConstructorException">Thrown when the construction couldn't be carried out correctly.</exception>
        public static PicturesOfConfiguration ConstructWithFlexibleLayoutRespectingSymmetry(this IGeometryConstructor constructor, Configuration configuration, Theorem theorem, int numberOfPictures)
        {
            #region Loose object layout construction

            // Prepare the function that will construct the loose objects
            IAnalyticObject[] LooseObjectsConstructor()
            {
                // The layout is drawn based on its type
                switch (configuration.LooseObjectsHolder.Layout)
                {
                    // We want to draw less uniform triangles
                    case LooseObjectLayout.Triangle:
                    {
                        // Create the points
                        var (point1, point2, point3) = RandomLayoutsHelpers.ConstructRandomTriangle();

                        // Return them in an array 
                        return new IAnalyticObject[] { point1, point2, point3 };
                    }

                    // In quadrilateral cases we need to have a look at its symmetry
                    case LooseObjectLayout.Quadrilateral:
                    {
                        // Assume we're drawing a quadrilateral ABCD
                        var A = configuration.LooseObjects[0];
                        var B = configuration.LooseObjects[1];
                        var C = configuration.LooseObjects[2];
                        var D = configuration.LooseObjects[3];

                        // We will use a simple local function that checks whether a mapping is symmetric
                        bool IsSymmetric(IReadOnlyDictionary<LooseConfigurationObject, LooseConfigurationObject> mapping)
                            // Take the real mappings
                            => theorem.GetSymmetryMappings(configuration)
                                // And have a look whether any behaves as our
                                .Any(symmetryMapping => mapping.All(pair => symmetryMapping[pair.Key].Equals(pair.Value)));

                        // Find out if the mapping for when the diagonal BD is horizontal is symmetric. 
                        // In that case, we need to exchange B and D
                        var canDiagonalBeHorizontal = IsSymmetric(new Dictionary<LooseConfigurationObject, LooseConfigurationObject>
                        {
                            { A, A },
                            { B, D },
                            { C, C },
                            { D, B }
                        });

                        // Find out if the mapping for when the side AB is horizontal is symmetric. 
                        // In that case, we need to exchange A, B and also C, D
                        var canSideBeHorizontal = IsSymmetric(new Dictionary<LooseConfigurationObject, LooseConfigurationObject>
                        {
                            { A, B },
                            { B, A },
                            { C, D },
                            { D, C }
                        });

                        // We will want to have horizontal mapping preferably, i.e. if it is possible or the diagonal is not
                        if (canSideBeHorizontal || !canDiagonalBeHorizontal)
                        {
                            // Construct the layout
                            var (point1, point2, point3, point4) = RandomLayoutsHelpers.ConstructRandomConvexQuadrilateralWithHorizontalSide();

                            // Return the points in an array 
                            return new IAnalyticObject[] { point1, point2, point3, point4 };
                        }
                        // Otherwise the side cannot be horizontal, but at least the diagonal can be
                        else
                        {
                            // Construct the layout
                            var (point1, point2, point3, point4) = RandomLayoutsHelpers.ConstructRandomConvexQuadrilateralWithHorizontalDiagonal();

                            // Return the points in an array 
                            return new IAnalyticObject[] { point1, point2, point3, point4 };
                        }
                    }

                    // By default fall-back to the default uniform layout
                    default:
                        return LooseObjectLayoutDrawing.ConstructUniformLayout(configuration.LooseObjectsHolder.Layout);
                }
            }

            #endregion

            try
            {
                // Try to construct the configuration where the passed theorem holds using our custom layout drawer
                var (pictures, constructionData) = constructor.Construct(configuration, numberOfPictures, LooseObjectsConstructor);

                // Make sure there is no inconstructible object
                if (constructionData.InconstructibleObject != default)
                    throw new ConstructionException("The configuration cannot be constructed, because it contains an inconstructible object.");

                // Make sure there are no duplicates
                if (constructionData.Duplicates != default)
                    throw new ConstructionException("The configuration cannot be constructed, because it contains duplicate objects");

                // If everything is correct, return the pictures
                return pictures;
            }
            catch (InconsistentPicturesException e)
            {
                // If there is an inconsistency problem, re-throw it with a better exception
                throw new ConstructionException("Construction of the configuration failed due to inconsistencies.", e);
            }
        }
    }
}