using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.TheoremsFinder
{
    /// <summary>
    /// A <see cref="ITheoremsFinder"/> for <see cref="TheoremType.LineTangentToCircle"/>.
    /// </summary>
    public class LineTangentToCircleTheoremsFinder : IntersectingTheoremsFinder
    {
        #region Private fields

        /// <summary>
        /// The settings for the finder.
        /// </summary>
        private readonly LineTangentToCircleTheoremsFinderSettings _settings;

        #endregion

        #region Protected overridden properties

        /// <summary>
        /// Indicates whether we want intersected objects to have at least one 
        /// intersection that lies outside of the picture.
        /// </summary>
        protected override bool ExpectAnyExternalIntersection => _settings.ExcludeTangencyInsidePicture;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="LineTangentToCircleTheoremsFinder"/> class.
        /// </summary>
        /// <param name="settings">The settings for the finder.</param>
        public LineTangentToCircleTheoremsFinder(LineTangentToCircleTheoremsFinderSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        #endregion

        #region Protected overridden methods

        /// <summary>
        /// Gets all options for a theorem represented as an array of geometric points.
        /// </summary>
        /// <param name="contextualPicture">The contextual picture that stores the geometric points.</param>
        /// <returns>An enumerable of all the options.</returns>
        protected override IEnumerable<GeometricObject[]> GetAllOptions(ContextualPicture contextualPicture)
        {
            // If we are excluding tangencies inside picture, we 
            // don't have to consider lines and circles with a common point
            if (_settings.ExcludeTangencyInsidePicture)
            {
                // Combine the new lines with all the circles
                foreach (var line in contextualPicture.AllLines)
                    foreach (var circle in contextualPicture.AllCircles)
                        if (line.CommonPointsWith(circle).IsEmpty())
                            yield return new GeometricObject[] { line, circle };
            }
            // Otherwise we don't have to consider lines and circles with 2 common points
            else
            {
                // Combine the new lines with all the circles
                foreach (var line in contextualPicture.AllLines)
                    foreach (var circle in contextualPicture.AllCircles)
                        if (line.CommonPointsWith(circle).Count() != 2)
                            yield return new GeometricObject[] { line, circle };
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
            // Find new circles
            var newCircles = contextualPicture.NewCircles.ToList();

            // Find new lines 
            var newLines = contextualPicture.NewLines.ToList();

            // Find all circles
            var allCircles = contextualPicture.AllCircles.ToList();

            // Find old lines
            var oldLines = contextualPicture.OldLines.ToList();

            // If we are excluding tangencies inside picture, we 
            // don't have to consider lines and circles with a common point
            if (_settings.ExcludeTangencyInsidePicture)
            {
                // Combine the new lines with all the circles
                foreach (var newLine in newLines)
                    foreach (var anyCircle in allCircles)
                        if (newLine.CommonPointsWith(anyCircle).IsEmpty())
                            yield return new GeometricObject[] { newLine, anyCircle };

                // Combine the old lines with new circles
                foreach (var oldLine in oldLines)
                    foreach (var newCircle in newCircles)
                        if (oldLine.CommonPointsWith(newCircle).IsEmpty())
                            yield return new GeometricObject[] { oldLine, newCircle };
            }
            // Otherwise we don't have to consider lines and circles with 2 common points
            else
            {
                // Combine the new lines with all the circles
                foreach (var newLine in newLines)
                    foreach (var anyCircle in allCircles)
                        if (newLine.CommonPointsWith(anyCircle).Count() != 2)
                            yield return new GeometricObject[] { newLine, anyCircle };

                // Combine the old lines with new circles
                foreach (var oldLine in oldLines)
                    foreach (var newCircle in newCircles)
                        if (oldLine.CommonPointsWith(newCircle).Count() != 2)
                            yield return new GeometricObject[] { oldLine, newCircle };
            }
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
