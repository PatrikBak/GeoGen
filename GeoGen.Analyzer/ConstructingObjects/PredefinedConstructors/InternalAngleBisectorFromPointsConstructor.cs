using System;
using System.Collections.Generic;
using GeoGen.AnalyticalGeometry;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// An <see cref="IObjectsConstructor"/> for <see cref="PredefinedConstructionType.InternalAngleBisectorFromPoints"/>>.
    /// </summary>
    internal class InternalAngleBisectorFromPointsConstructor : PredefinedConstructorBase
    {
        #region Private fields

        /// <summary>
        /// The helper for determining collinearity.
        /// </summary>
        private IAnalyticalHelper _analyticalHelper;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="analyticalHelper">The helper for determining collinearity.</param>
        public InternalAngleBisectorFromPointsConstructor(IAnalyticalHelper analyticalHelper)
        {
            _analyticalHelper = analyticalHelper ?? throw new ArgumentNullException(nameof(analyticalHelper));
        }

        #endregion

        #region PredefinedConstructorBase implementation

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
            // Pull points on ray
            var point1 = container.Get<Point>(flattenedObjects[1]);
            var point2 = container.Get<Point>(flattenedObjects[2]);

            // Pull the rays intersection
            var intersection = container.Get<Point>(flattenedObjects[0]);

            // Check if our points are collinear
            if (_analyticalHelper.AreCollinear(point1, point2, intersection))
            {
                // If yes, we don't want to perform the construction (because 
                // then it's theoretically equivalent to the perpendicular line construction
                return null;
            }

            // Otherwise we can create the internal bisector and wrap it.
            return new List<AnalyticalObject> {intersection.InternalAngleBisector(point1, point2)};
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
            return new List<Theorem>();
        }

        #endregion
    }
}