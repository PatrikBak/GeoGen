using GeoGen.AnalyticGeometry;
using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.TheoremsFinder
{
    /// <summary>
    /// A <see cref="ITheoremsFinder"/> for <see cref="TheoremType.ParallelLines"/>.
    /// </summary>
    public class ParallelLinesTheoremsFinder : TrueInAllPicturesTheoremsFinder
    {
        /// <summary>
        /// Gets all options for a theorem represented as an array of geometric points.
        /// </summary>
        /// <param name="contextualPicture">The contextual picture that stores the geometric points.</param>
        /// <returns>An enumerable of all the options.</returns>
        protected override IEnumerable<GeometricObject[]> GetAllOptions(ContextualPicture contextualPicture)
        {
            // Get all pairs of lines
            return contextualPicture.AllLines.Subsets(2);
        }

        /// <summary>
        /// Gets all options for a new theorem represented as an array of geometric points.
        /// Such theorems cannot be stated without the last object of the configuration. 
        /// </summary>
        /// <param name="contextualPicture">The contextual picture that stores the geometric points.</param>
        /// <returns>An enumerable of all the options.</returns>
        protected override IEnumerable<GeometricObject[]> GetNewOptions(ContextualPicture contextualPicture)
        {
            // Find new lines
            var newLines = contextualPicture.NewLines.ToList();

            // Find old lines
            var oldLines = contextualPicture.OldLines.ToList();

            // Combine the new lines with themselves
            foreach (var (newLine1, newLine2) in newLines.UnorderedPairs())
                yield return new[] { newLine1, newLine2 };

            // Combine the new lines with the old ones
            foreach (var newLine in newLines)
                foreach (var oldLine in oldLines)
                    yield return new[] { newLine, oldLine };
        }

        /// <summary>
        /// Finds out if the theorem given in analytic objects holds true.
        /// </summary>
        /// <param name="objects">The analytic objects.</param>
        /// <returns>true, if the theorem holds true; false otherwise.</returns>
        protected override bool IsTrue(IAnalyticObject[] objects)
        {
            // Return if they are parallel to each other
            return ((Line)objects[0]).IsParallelTo((Line)objects[1]);
        }
    }
}
