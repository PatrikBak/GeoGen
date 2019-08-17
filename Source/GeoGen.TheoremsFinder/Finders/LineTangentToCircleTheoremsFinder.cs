using GeoGen.AnalyticGeometry;
using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.TheoremsFinder
{
    /// <summary>
    /// A <see cref="ITheoremsFinder"/> for <see cref="TheoremType.LineTangentToCircle"/>.
    /// </summary>
    public class LineTangentToCircleTheoremsFinder : TrueInAllPicturesTheoremsFinder
    {
        /// <summary>
        /// Gets all options for a theorem represented as an array of geometric points.
        /// </summary>
        /// <param name="contextualPicture">The contextual picture that stores the geometric points.</param>
        /// <returns>An enumerable of all the options.</returns>
        protected override IEnumerable<GeometricObject[]> GetAllOptions(ContextualPicture contextualPicture)
        {
            // Combine all lines and all circles
            return new IEnumerable<GeometricObject>[] { contextualPicture.AllLines, contextualPicture.AllCircles }.Combine();
        }

        /// <summary>
        /// Gets all options for a new theorem represented as an array of geometric points.
        /// Such theorems cannot be stated without the last object of the configuration. 
        /// </summary>
        /// <param name="contextualPicture">The contextual picture that stores the geometric points.</param>
        /// <returns>An enumerable of all the options.</returns>
        protected override IEnumerable<GeometricObject[]> GetNewOptions(ContextualPicture contextualPicture)
        {
            // Find new circles
            var newCircles = contextualPicture.NewCircles.ToList();

            // Find new lines 
            var newLines = contextualPicture.NewLines.ToList();

            // Find all circles
            var allCircles = contextualPicture.AllCircles.ToList();

            // Find old lines
            var oldLines = contextualPicture.OldLines.ToList();

            // Combine the new lines with all the circles
            foreach (var newLine in newLines)
                foreach (var anyCircle in allCircles)
                    yield return new GeometricObject[] { newLine, anyCircle };

            // Combine the new circles with the old lines
            foreach (var newCircle in newCircles)
                foreach (var oldLine in oldLines)
                    yield return new GeometricObject[] { oldLine, newCircle };
        }

        /// <summary>
        /// Finds out if the theorem given in analytic objects holds true.
        /// </summary>
        /// <param name="objects">The analytic objects.</param>
        /// <returns>true, if the theorem holds true; false otherwise.</returns>
        protected override bool IsTrue(IAnalyticObject[] objects)
        {
            // Return if they are tangent to each other
            return ((Circle)objects[1]).IsTangentTo((Line)objects[0]);
        }
    }
}
