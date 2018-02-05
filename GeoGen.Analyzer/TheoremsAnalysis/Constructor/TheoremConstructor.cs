using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core;
using GeoGen.Utilities;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// A default implementation of <see cref="ITheoremConstructor"/>.
    /// </summary>
    internal class TheoremConstructor : ITheoremConstructor
    {
        /// <summary>
        /// Constructs a theorem.
        /// </summary>
        /// <param name="involvedObjects">The list of objects that this theorem is about.</param>
        /// <param name="type">The type of the theorem.</param>
        /// <returns>The theorem.</returns>
        public Theorem Construct(IEnumerable<GeometricalObject> involvedObjects, TheoremType type)
        {
            // Cast all involved objects to the theorem objects
            var theoremObjects = involvedObjects
                    .Select(Construct)
                    .ToList();

            // Construct the theorem
            return new Theorem(type, theoremObjects);
        }

        /// <summary>
        /// Constructs a theorem object from a given geometrical object.
        /// </summary>
        /// <param name="geometricalObject">The geometrical object.</param>
        /// <returns>The theorem object.</returns>
        private TheoremObject Construct(GeometricalObject geometricalObject)
        {
            // First we look if the configuration version of the object is present
            var configurationObject = geometricalObject.ConfigurationObject;

            // If it's present
            if (configurationObject != null)
            {
                // Then we simply wrap it
                return new TheoremObject(configurationObject);
            }

            // Otherwise the object is either a line, or a circle, so its definable by points
            var objectWithPoints = (DefinableByPoints) geometricalObject;

            // We pull the points that defines the object
            var points = objectWithPoints.Points;

            // And pull their configuration versions
            var involedObjects = points.Select(p => p.ConfigurationObject).ToList();

            // Determine the right signature of the theorem object (according to whether
            // it is a line or a circle)
            var objectType = objectWithPoints is LineObject
                    ? TheoremObjectSignature.LineGivenByPoints
                    : TheoremObjectSignature.CircleGivenByPoints;

            // And finally construct the theorem objects
            return new TheoremObject(objectType, involedObjects);
        }
    }
}