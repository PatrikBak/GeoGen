using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Analyzer.Objects.GeometricalObjects;
using GeoGen.Analyzer.Objects.GeometricalObjects.Container;
using GeoGen.Core.Theorems;
using GeoGen.Core.Utilities;

namespace GeoGen.Analyzer.Theorems.TheoremVerifiers
{
    internal class CollinearityVerifier : ITheoremVerifier
    {
        public TheoremType TheoremType { get; } = TheoremType.CollinearPoints;

        private readonly IContextualContainer _container;

        public CollinearityVerifier(IContextualContainer container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
        }

        public IEnumerable<VerifierOutput> GetOutput(ConfigurationObjectsMap oldObjects, ConfigurationObjectsMap newObjects)
        {
            if (oldObjects == null)
                throw new ArgumentNullException(nameof(oldObjects));

            if (newObjects == null)
                throw new ArgumentNullException(nameof(newObjects));

            // Create all objects dictionary
            var allObjects = oldObjects.Merge(newObjects);

            // Take all points from the container
            var allPoints = _container.GetObjects<PointObject>(allObjects).ToList();

            // Take new points
            var newPoints = _container.GetNewObjects<PointObject>(oldObjects, newObjects).ToList();

            // Get all non-physical lines
            var lineObjects = _container.GetObjects<LineObject>(allObjects);

            // Iterate over lines
            foreach (var lineObject in lineObjects)
            {
                // Take points that line on this line (there should be at least one)
                var containingPoints = allPoints.Where(lineObject.Points.Contains).ToList();

                // If there are less than 3 points, skip
                if (containingPoints.Count < 3)
                    continue;

                // Find number of new points on the line
                var containingNewPoints = newPoints.Where(lineObject.Points.Contains).ToList();

                // If there is none, skip
                if (containingNewPoints.Empty())
                    continue;

                // Otherwise we have a non-trivial collinearity. We construct the theorem
                // The objects container is not even needed
                yield return new VerifierOutput
                {
                    AllObjects = allObjects,
                    InvoldedObjects = containingPoints.Cast<GeometricalObject>().ToList(),
                    VerifierFunction = container => true
                };
            }
        }
    }
}