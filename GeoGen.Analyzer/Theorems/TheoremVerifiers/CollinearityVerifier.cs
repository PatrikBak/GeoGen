using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Theorems;
using GeoGen.Utilities;

namespace GeoGen.Analyzer
{
    internal class CollinearityVerifier : ITheoremVerifier
    {
        public TheoremType TheoremType { get; } = TheoremType.CollinearPoints;

        public IEnumerable<VerifierOutput> GetOutput(VerifierInput verifierInput)
        {
            if (verifierInput == null)
                throw new ArgumentNullException(nameof(verifierInput));
            
            // Take all points from the container
            var allPoints = verifierInput.AllPoints;

            // Take new points
            var newPoints = verifierInput.NewPoints;

            // Get all non-physical lines
            var lineObjects = verifierInput.AllLines;

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
                    InvoldedObjects = containingPoints.Cast<GeometricalObject>().ToList(),
                    VerifierFunction = null
                };
            }
        }
    }
}