using GeoGen.AnalyticGeometry;
using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.TheoremFinder
{
    /// <summary>
    /// A <see cref="ITypedTheoremFinder"/> for <see cref="TheoremType.EqualAngles"/>.
    /// </summary>
    public class EqualAnglesTheoremFinder : TrueInAllPicturesTheoremFinder
    {
        /// <summary>
        /// Gets all options for a theorem represented as an array of geometric points.
        /// </summary>
        /// <param name="contextualPicture">The contextual picture that stores the geometric points.</param>
        /// <returns>An enumerable of all the options.</returns>
        protected override IEnumerable<GeometricObject[]> GetAllOptions(ContextualPicture contextualPicture)
        {
            // Get all lines
            return contextualPicture.AllLines.ToList()
                // And all its pairs
                .UnorderedPairs().ToList()
                // And all pairs of these pairs
                .UnorderedPairs()
                // Each represents 4 lines
                .Select(lines => new[] { lines.Item1.Item1, lines.Item1.Item2, lines.Item2.Item1, lines.Item2.Item2 });
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

            #region Getting new line segments

            // Prepare the list of new angles
            var newAngles = new List<(LineObject, LineObject)>();

            // Combine the new lines with themselves
            foreach (var pairOfLines in newLines.UnorderedPairs())
                newAngles.Add(pairOfLines);

            // Combine the new lines with the old ones
            foreach (var newLine in newLines)
                foreach (var oldLine in oldLines)
                    newAngles.Add((newLine, oldLine));

            #endregion

            // Get the old angles
            var oldAngles = oldLines.UnorderedPairs().ToList();

            // Combine the new angles with themselves
            foreach (var ((line1, line2), (line3, line4)) in newAngles.UnorderedPairs())
                yield return new[] { line1, line2, line3, line4 };

            // Combine the angles with the old ones
            foreach (var (line1, line2) in newAngles)
                foreach (var (line3, line4) in oldAngles)
                    yield return new[] { line1, line2, line3, line4 };
        }

        /// <summary>
        /// Finds out if the theorem given in analytic objects holds true.
        /// </summary>
        /// <param name="objects">The analytic objects.</param>
        /// <returns>true, if the theorem holds true; false otherwise.</returns>
        protected override bool IsTrue(IAnalyticObject[] objects)
        {
            // Find their angles
            var angle1 = AnalyticHelpers.AngleBetweenLines((Line)objects[0], (Line)objects[1]).Rounded();
            var angle2 = AnalyticHelpers.AngleBetweenLines((Line)objects[2], (Line)objects[3]).Rounded();

            // Return if they match 
            return angle1 == angle2
                // and are not equal to 0 (i.e. parallelity)
                && angle1 != 0
                // and are not equal to PI / 2 (i.e. perpendicularity)
                && angle1 != (Math.PI / 2).Rounded();
        }
    }
}
