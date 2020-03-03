using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.TheoremFinder
{
    /// <summary>
    /// A <see cref="ITypedTheoremFinder"/> for <see cref="TheoremType.LineTangentToCircle"/>.
    /// </summary>
    public class LineTangentToCircleTheoremFinder : IntersectingTheoremFinder
    {
        #region Private fields

        /// <summary>
        /// The settings for the finder.
        /// </summary>
        private readonly LineTangentToCircleTheoremFinderSettings _settings;

        #endregion

        #region Protected overridden properties

        /// <inheritdoc/>
        protected override bool ExpectAnyExternalIntersection => _settings.ExcludeTangencyInsidePicture;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="LineTangentToCircleTheoremFinder"/> class.
        /// </summary>
        /// <param name="settings">The settings for the finder.</param>
        public LineTangentToCircleTheoremFinder(LineTangentToCircleTheoremFinderSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        #endregion

        #region Protected overridden methods

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        #endregion
    }
}
