using System;
using System.Collections.Generic;
using GeoGen.AnalyticGeometry;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// An <see cref="IObjectsConstructor"/> for <see cref="PredefinedConstructionType.RandomPointOnLineFromPoints"/>>.
    /// </summary>
    public class RandomPointOnLineFromPointsConstructor : PredefinedConstructorBase
    {
        #region Private fields

        /// <summary>
        /// The randomness provided.
        /// </summary>
        private readonly IRandomnessProvider _provider;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="provider">The randomness provided.</param>
        public RandomPointOnLineFromPointsConstructor(IRandomnessProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        #endregion

        #region PredefinedConstructorBase methods

        /// <summary>
        /// Constructs a list of analytic objects from a given list of 
        /// flattened objects from the arguments and a container that is used to 
        /// obtain the actual analytic versions of these objects.
        /// </summary>
        /// <param name="flattenedObjects">The flattened argument objects.</param>
        /// <param name="container">The objects container.</param>
        /// <returns>The list of constructed analytic objects.</returns>
        protected override List<AnalyticObject> Construct(IReadOnlyList<ConfigurationObject> flattenedObjects, IObjectsContainer container)
        {
            // Pull line points
            var point1 = container.Get<Point>(flattenedObjects[0]);
            var point2 = container.Get<Point>(flattenedObjects[1]);

            // Create the line from them
            var line = new Line(point1, point2);

            // Try to find a distinct point from our given ones
            while (true)
            {
                // Get random point
                var randomPoint = line.RandomPointOnLine(_provider);

                // Check if it's correct, i.e. distinct from out all points from the container
                if (!container.Contains(randomPoint))
                {
                    // If yes, return it
                    return new List<AnalyticObject> { randomPoint };
                }
            }
        }

        #endregion
    }
}