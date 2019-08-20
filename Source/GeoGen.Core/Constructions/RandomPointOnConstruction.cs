using static GeoGen.Core.ConfigurationObjectType;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a construction that outputs a random point on a result
    /// of another construction (which should be either a line or circle).
    /// </summary>
    public class RandomPointOnConstruction : Construction
    {
        #region Public properties

        /// <summary>
        /// Gets the construction outputting a line or circle where a random point
        /// should be constructed.
        /// </summary>
        public Construction Construction { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomPointOnConstruction"/> class.
        /// </summary>
        /// <param name="construction">The construction outputting a line or circle where a random point should be constructed.</param>
        public RandomPointOnConstruction(Construction construction)
            : base($"RandomPointOn{construction.Name}", construction.Signature.Parameters, Point)
        {
            Construction = construction;

            // Make sure the output type of this construction is a line or circle
            if (construction.OutputType != Line && construction.OutputType != Circle)
                throw new GeoGenException("The 'RandomPointOn' can be applied only to a line or circle.");
        }

        #endregion

        #region Public static methods

        /// <summary>
        /// A helper to create a new instance of the <see cref="RandomPointOnConstruction"/> class.
        /// </summary>
        /// <param name="construction">The construction outputting a line or circle where a random point should be constructed.</param>
        /// <returns>The random point construction.</returns>
        public static RandomPointOnConstruction RandomPointOn(Construction construction) => new RandomPointOnConstruction(construction);

        /// <summary>
        /// A helper to create a new instance of the <see cref="RandomPointOnConstruction"/> class.
        /// </summary>
        /// <param name="type">The type of predefined construction outputting a line or circle where a random point should be constructed.</param>
        /// <returns>The random point construction.</returns>
        public static RandomPointOnConstruction RandomPointOn(PredefinedConstructionType type)
            // Use helper to find the construction for the type
            => new RandomPointOnConstruction(Constructions.GetPredefinedconstruction(type));

        #endregion
    }
}
