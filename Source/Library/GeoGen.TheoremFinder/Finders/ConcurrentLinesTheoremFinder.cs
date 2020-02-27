using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.TheoremFinder
{
    /// <summary>
    /// A <see cref="ITypedTheoremFinder"/> for <see cref="TheoremType.ConcurrentLines"/>.
    /// </summary>
    public class ConcurrentLinesTheoremFinder : IntersectingTheoremFinder
    {
        #region Protected overridden properties

        /// <summary>
        /// Indicates whether we want intersected objects to have at least one 
        /// intersection that lies outside of the picture.
        /// </summary>
        protected override bool ExpectAnyExternalIntersection => true;

        #endregion

        #region Protected overridden methods

        /// <summary>
        /// Gets all options for a theorem represented as an array of geometric points.
        /// </summary>
        /// <param name="contextualPicture">The contextual picture that stores the geometric points.</param>
        /// <returns>An enumerable of all the options.</returns>
        protected override IEnumerable<GeometricObject[]> GetAllOptions(ContextualPicture contextualPicture)
        {
            // Get lines as a list
            var allLines = contextualPicture.AllLines.ToList();

            // Go through all unordered pairs 
            for (var i = 0; i < allLines.Count; i++)
            {
                for (var j = i + 1; j < allLines.Count; j++)
                {
                    // Get the lines for comfort
                    var line1 = allLines[i];
                    var line2 = allLines[j];

                    // If they have a common point, exclude them right away
                    if (line1.CommonPointsWith(line2).Any())
                        continue;

                    // Otherwise go through the remaining lines
                    for (var k = j + 1; k < allLines.Count; k++)
                    {
                        // Get the third line for comfort
                        var line3 = allLines[k];

                        // If it has any common point with our two, skip it
                        if (line3.CommonPointsWith(line1).Any() || line3.CommonPointsWith(line2).Any())
                            continue;

                        // Otherwise return this triple
                        yield return new[] { line1, line2, line3 };
                    }
                }
            }
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

            #region Three new lines

            // Go through all unordered pairs 
            for (var i = 0; i < newLines.Count; i++)
            {
                for (var j = i + 1; j < newLines.Count; j++)
                {
                    // Get the lines for comfort
                    var line1 = newLines[i];
                    var line2 = newLines[j];

                    // If they have a common point, exclude them right away
                    if (line1.CommonPointsWith(line2).Any())
                        continue;

                    // Otherwise go through the remaining lines
                    for (var k = j + 1; k < newLines.Count; k++)
                    {
                        // Get the third line for comfort
                        var line3 = newLines[k];

                        // If it has any common point with our two, skip it
                        if (line3.CommonPointsWith(line1).Any() || line3.CommonPointsWith(line2).Any())
                            continue;

                        // Otherwise return this triple
                        yield return new[] { line1, line2, line3 };
                    }
                }
            }

            #endregion

            #region Two new lines, one old

            // Take new line pairs and combine them with the old ones
            foreach (var (newLine1, newLine2) in newLines.UnorderedPairs())
                if (newLine1.CommonPointsWith(newLine2).IsEmpty())
                    foreach (var oldLine in oldLines)
                        if (oldLine.CommonPointsWith(newLine1).IsEmpty() && oldLine.CommonPointsWith(newLine2).IsEmpty())
                            yield return new[] { newLine1, newLine2, oldLine };

            #endregion

            #region One new line, two old

            // Take old line pairs and combine them with the new ones
            foreach (var (oldLine1, oldLine2) in oldLines.UnorderedPairs())
                if (oldLine1.CommonPointsWith(oldLine2).IsEmpty())
                    foreach (var newLine in newLines)
                        if (newLine.CommonPointsWith(oldLine1).IsEmpty() && newLine.CommonPointsWith(oldLine2).IsEmpty())
                            yield return new[] { oldLine1, oldLine2, newLine };

            #endregion
        }

        /// <summary>
        /// Returns if a given number intersections is allowed for this type of theorem.
        /// </summary>
        /// <param name="numberOfIntersections">The number of intersections to be questioned.</param>
        /// <returns>true, if this number is allowed; false otherwise.</returns>
        protected override bool IsNumberOfIntersectionsAllowed(int numberOfIntersections) => numberOfIntersections == 1;

        #endregion
    }
}
