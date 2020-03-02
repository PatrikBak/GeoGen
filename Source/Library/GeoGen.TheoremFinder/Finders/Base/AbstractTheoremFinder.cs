using GeoGen.Constructor;
using GeoGen.Core;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.TheoremFinder
{
    /// <summary>
    /// An implementation of <see cref="TheoremFinderBase"/> that servers as a template 
    /// for most (not all) theorem finders.
    /// </summary>
    public abstract class AbstractTheoremFinder : TheoremFinderBase
    {
        #region TheoremFinderBase implementation

        /// <inheritdoc/>
        public override IEnumerable<Theorem> FindAllTheorems(ContextualPicture contextualPicture)
            // Take all options
            => GetAllOptions(contextualPicture)
                // That represent a true theorem
                .Where(objects => RepresentsTrueTheorem(contextualPicture, objects))
                // Cast each to theorems
                .SelectMany(ToTheorems);

        /// <inheritdoc/>
        public override IEnumerable<Theorem> FindNewTheorems(ContextualPicture contextualPicture)
            // Take new options
            => GetNewOptions(contextualPicture)
                // That represent a true theorem
                .Where(objects => RepresentsTrueTheorem(contextualPicture, objects))
                // Cast each to theorems
                .SelectMany(ToTheorems);

        #endregion

        #region Protected abstract methods

        /// <summary>
        /// Gets all options for a theorem represented as an array of geometric points.
        /// </summary>
        /// <param name="contextualPicture">The contextual picture that stores the geometric points.</param>
        /// <returns>An enumerable of all the options.</returns>
        protected abstract IEnumerable<GeometricObject[]> GetAllOptions(ContextualPicture contextualPicture);

        /// <summary>
        /// Gets all options for a new theorem represented as an array of geometric points.
        /// Such theorems cannot be stated without the last object of the configuration. 
        /// </summary>
        /// <param name="contextualPicture">The contextual picture that stores the geometric points.</param>
        /// <returns>An enumerable of all the options.</returns>
        protected abstract IEnumerable<GeometricObject[]> GetNewOptions(ContextualPicture contextualPicture);

        /// <summary>
        /// Finds out if given geometric objects represent a true theorem.
        /// </summary>
        /// <param name="contextualPicture">The contextual picture that stores the geometric points.</param>
        /// <param name="objects">The geometric objects that represent the theorem.</param>
        /// <returns>true, if the theorem holds true; false otherwise.</returns>
        protected abstract bool RepresentsTrueTheorem(ContextualPicture contextualPicture, GeometricObject[] objects);

        #endregion
    }
}
