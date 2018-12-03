using System;
using System.Collections.Generic;
using GeoGen.AnalyticalGeometry;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// An <see cref="IObjectsConstructor"/> for <see cref="PredefinedConstructionType.CircumcenterFromPoints"/>>.
    /// </summary>
    internal class CircumcenterFromPointsConstructor : PredefinedConstructorBase
    {
        /// <summary>
        /// Constructs a list of analytical objects from a given list of 
        /// flattened objects from the arguments and a container that is used to 
        /// obtain the actual analytical versions of these objects.
        /// </summary>
        /// <param name="flattenedObjects">The flattened argument objects.</param>
        /// <param name="container">The objects container.</param>
        /// <returns>The list of constructed analytical objects.</returns>
        protected override List<AnalyticalObject> Construct(IReadOnlyList<ConfigurationObject> flattenedObjects, IObjectsContainer container)
        {
            // Pull points from the container
            var point1 = container.Get<Point>(flattenedObjects[0]);
            var point2 = container.Get<Point>(flattenedObjects[1]);
            var point3 = container.Get<Point>(flattenedObjects[2]);

            // If points are collinear, the construction can't be done
            if (AnalyticalHelpers.AreCollinear(point1, point2, point3))
                return null;

            // Otherwise construct the circle and take its center
            return new List<AnalyticalObject> { new Circle(point1, point2, point3).Center };
        }

        /// <summary>
        /// Constructs a list of default theorems using a newly constructed objects and
        /// flattened objects from the passed arguments.
        /// </summary>
        /// <param name="input">The constructed objects.</param>
        /// <param name="flattenedObjects">The flattened argument objects.</param>
        /// <returns>The list of default theorems.</returns>
        protected override List<Theorem> FindDefaultTheorms(IReadOnlyList<ConstructedConfigurationObject> input, IReadOnlyList<ConfigurationObject> flattenedObjects)
        {
            return new List<Theorem>
            {
                new Theorem(TheoremType.EqualLineSegments, new List<TheoremObject>
                {
                    new TheoremObject(input[0]),
                    new TheoremObject(flattenedObjects[0]),
                    new TheoremObject(input[0]),
                    new TheoremObject(flattenedObjects[1])
                }),
                new Theorem(TheoremType.EqualLineSegments, new List<TheoremObject>
                {
                    new TheoremObject(input[0]),
                    new TheoremObject(flattenedObjects[0]),
                    new TheoremObject(input[0]),
                    new TheoremObject(flattenedObjects[2])
                }),
                new Theorem(TheoremType.EqualLineSegments, new List<TheoremObject>
                {
                    new TheoremObject(input[0]),
                    new TheoremObject(flattenedObjects[1]),
                    new TheoremObject(input[0]),
                    new TheoremObject(flattenedObjects[2])
                }),
                new Theorem(TheoremType.EqualAngles, new List<TheoremObject>
                {
                    new TheoremObject(TheoremObjectSignature.LineGivenByPoints, flattenedObjects[0], flattenedObjects[1]),
                    new TheoremObject(TheoremObjectSignature.LineGivenByPoints, input[0], flattenedObjects[0]),
                    new TheoremObject(TheoremObjectSignature.LineGivenByPoints, flattenedObjects[0], flattenedObjects[1]),
                    new TheoremObject(TheoremObjectSignature.LineGivenByPoints, input[0], flattenedObjects[1]),
                }),
                new Theorem(TheoremType.EqualAngles, new List<TheoremObject>
                {
                    new TheoremObject(TheoremObjectSignature.LineGivenByPoints, flattenedObjects[1], flattenedObjects[2]),
                    new TheoremObject(TheoremObjectSignature.LineGivenByPoints, input[0], flattenedObjects[1]),
                    new TheoremObject(TheoremObjectSignature.LineGivenByPoints, flattenedObjects[1], flattenedObjects[2]),
                    new TheoremObject(TheoremObjectSignature.LineGivenByPoints, input[0], flattenedObjects[2]),
                }),
                new Theorem(TheoremType.EqualAngles, new List<TheoremObject>
                {
                    new TheoremObject(TheoremObjectSignature.LineGivenByPoints, flattenedObjects[2], flattenedObjects[0]),
                    new TheoremObject(TheoremObjectSignature.LineGivenByPoints, input[0], flattenedObjects[2]),
                    new TheoremObject(TheoremObjectSignature.LineGivenByPoints, flattenedObjects[2], flattenedObjects[0]),
                    new TheoremObject(TheoremObjectSignature.LineGivenByPoints, input[0], flattenedObjects[0]),
                }),
            };
        }
    }
}