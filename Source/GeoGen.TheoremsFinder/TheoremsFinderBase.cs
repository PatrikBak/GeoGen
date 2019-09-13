using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.TheoremsFinder
{
    /// <summary>
    /// The base class for <see cref="ITheoremsFinder"/>s. 
    /// </summary>
    public abstract class TheoremsFinderBase : ITheoremsFinder
    {
        #region Private fields

        /// <summary>
        /// The type of the theorem that this finder finds.
        /// </summary>
        private readonly TheoremType _type;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremsFinderBase"/> class.
        /// </summary>
        protected TheoremsFinderBase()
        {
            // Find the type
            _type = FindTypeFromClassName();
        }

        #endregion

        #region Finding type from the class name

        /// <summary>
        /// Infers the type of the theorems finder from the class name. 
        /// The class name should be in the form {type}TheoremsFinder.
        /// </summary>
        /// <returns>The inferred type.</returns>
        private TheoremType FindTypeFromClassName()
        {
            // Call the utility helper that does the job
            return EnumUtilities.ParseEnumValueFromClassName<TheoremType>(GetType(), classNamePrefix: "TheoremsFinder");
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Converts given objects to a theorem holding in a given configuration.
        /// </summary>
        /// <param name="configuration">The configuration where the theorem holds.</param>
        /// <param name="geometricObjects">Flattened geometric objects that are converted to a theorem.</param>
        /// <returns>The theorem.</returns>
        protected Theorem ToTheorem(Configuration configuration, IEnumerable<GeometricObject> geometricObjects)
        {
            // Map geometric objects to theorem objects
            var allTheoremObjects = geometricObjects.Select(geometricObject => geometricObject switch
            {
                // In point case we have just the object
                PointObject point => new PointTheoremObject(geometricObject.ConfigurationObject) as TheoremObject,

                // In line case we need to take points into account
                LineObject line => new LineTheoremObject(geometricObject.ConfigurationObject, line.Points.Select(p => p.ConfigurationObject)),

                // In circle case we need to take points into account
                CircleObject circle => new CircleTheoremObject(geometricObject.ConfigurationObject, circle.Points.Select(p => p.ConfigurationObject)),

                // Default case
                _ => throw new TheoremsFinderException($"Unhandled type of geometric object: {geometricObject.GetType()}"),
            })
            // Enumerate to an array
            .ToArray();

            // Use helper method to construct the theorem
            return Theorem.DeriveFromFlattenedObjects(configuration, _type, allTheoremObjects);
        }

        #endregion

        #region ITheoremsFinder implementation

        /// <summary>
        /// Finds all theorems that hold true in the configuration represented by a given
        /// contextual picture.
        /// </summary>
        /// <param name="contextualPicture">The contextual picture that represents the configuration.</param>
        /// <returns>The enumerable of true theorems in the configuration.</returns>
        public abstract IEnumerable<Theorem> FindAllTheorems(ContextualPicture contextualPicture);

        /// <summary>
        /// Finds all theorems that hold true in the configuration and in their statement use the last 
        /// object of the configuration represented by a given contextual picture.
        /// </summary>
        /// <param name="contextualPicture">The contextual picture that represents the configuration.</param>
        /// <returns>The enumerable of true theorems in the configuration that use the last object.</returns>
        public abstract IEnumerable<Theorem> FindNewTheorems(ContextualPicture contextualPicture);

        #endregion
    }
}
