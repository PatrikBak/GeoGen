using GeoGen.Core;

namespace GeoGen.Constructor
{
    /// <summary>
    /// The extensions methods for <see cref="IGeometryConstructor"/>.
    /// </summary>
    public static class IGeometryConstructorExtensions
    {
        /// <summary>
        /// Constructs a given <see cref="Configuration"/> to a given number of pictures, using the 
        /// loose object drawing from the class <see cref="LooseObjectLayoutDrawing.ConstructUniformLayout(LooseObjectLayout)"/>.
        /// Throws an <see cref="InconsistentPicturesException"/> if the construction couldn't be carried out consistently.
        /// </summary>
        /// <param name="configuration">The configuration to be constructed.</param>
        /// <param name="numberOfPictures">The number of <see cref="Picture"/>s where the configuration should be drawn.</param>
        /// <returns>The tuple consisting of the pictures and the construction data.</returns>
        public static (PicturesOfConfiguration pictures, ConstructionData data) ConstructWithUniformLayout(this IGeometryConstructor constructor, Configuration configuration, int numberOfPictures)
            // Call the drawing function that uses a custom loose object drawer
            => constructor.Construct(configuration, numberOfPictures, () => LooseObjectLayoutDrawing.ConstructUniformLayout(configuration.LooseObjectsHolder.Layout));
    }
}