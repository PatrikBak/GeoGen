using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.AnalyticalGeometry;
using GeoGen.Core;
using GeoGen.Utilities;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// An <see cref="ITheoremVerifier"/> for <see cref="TheoremType.TangentCircles"/>.
    /// </summary>
    internal class TangentCirclesVerifier : TheoremVerifierBase
    {
        #region ITheoremVerifier implementation

        /// <summary>
        /// Gets the enumerable of verifier outputs that pulls objects from
        /// a given contextual container (that represents the configuration)
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns>The outputs.</returns>
        public override IEnumerable<VerifierOutput> GetOutput(IContextualContainer container)
        {
            // Find all new circles. At least one of them must be included in a new theorem
            var newLines = container.GetGeometricalObjects<GeometricalObject>(new ContexualContainerQuery
            {
                Type = ContexualContainerQuery.ObjectsType.New,
                IncludePoints = false,
                IncludeLines = false,
                IncludeCirces = true
            }).ToList();

            // Find all circles. These will be combined with the new ones
            var allLines = container.GetGeometricalObjects<GeometricalObject>(new ContexualContainerQuery
            {
                Type = ContexualContainerQuery.ObjectsType.All,
                IncludePoints = false,
                IncludeLines = false,
                IncludeCirces = true
            }).ToList();

            // Prepare set of all pairs of perpendicular lines in the first container
            var circles = new HashSet<Tuple<GeometricalObject, GeometricalObject>>();

            // Pull first container
            var firstContainer = container.Manager.First();

            // Iterate over lines to get all pairs
            foreach (var circle1 in newLines)
            {
                foreach (var circle2 in allLines)
                {
                    // If they are equal, we skip them
                    if (Equals(circle1, circle2))
                        continue;

                    // Pull their analytical version
                    var analytical1 = (Circle)container.GetAnalyticalObject(circle1, firstContainer);
                    var analytical2 = (Circle)container.GetAnalyticalObject(circle2, firstContainer);

                    // If the lines are not perpendicular, skip 
                    if (!analytical1.IsTangentTo(analytical2))
                        continue;

                    // We find the line with the smaller id
                    var withSmallerId = circle1.Id < circle2.Id ? circle1 : circle2;
                    var withLargerId = withSmallerId == circle1 ? circle2 : circle1;

                    // And add this pair of lines to the lines set
                    circles.Add(new Tuple<GeometricalObject, GeometricalObject>(withSmallerId, withLargerId));
                }
            }

            // After finding the lines paralellel in the first container, we want to have
            // a look to the others. We go through the lines set
            foreach (var pair in circles)
            {
                // Construct involved objects
                var involvedObjects = pair.Item1.AsEnumerable().Concat(pair.Item2.AsEnumerable()).ToList();

                // Construct the verifier function
                bool Verify(IObjectsContainer objectsContainer)
                {
                    // If the container is the first one, we're sure the lines are fine
                    if (ReferenceEquals(firstContainer, objectsContainer))
                        return true;

                    // Otherwise we find analytical versions of our lines
                    var analytical1 = (Circle)container.GetAnalyticalObject(involvedObjects[0], firstContainer);
                    var analytical2 = (Circle)container.GetAnalyticalObject(involvedObjects[1], firstContainer);

                    // And return if they are parallel
                    return analytical1.IsTangentTo(analytical2);
                }

                // Construct the output
                yield return new VerifierOutput
                {
                    Type = Type,
                    InvoldedObjects = involvedObjects,
                    AlwaysTrue = false,
                    VerifierFunction = Verify
                };
            }
        }

        #endregion
    }
}