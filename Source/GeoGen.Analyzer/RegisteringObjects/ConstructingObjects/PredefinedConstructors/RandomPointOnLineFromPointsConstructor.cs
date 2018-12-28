using GeoGen.AnalyticGeometry;
using GeoGen.Core;
using System;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// The <see cref="IObjectsConstructor"/> for <see cref="PredefinedConstructionType.RandomPointOnLineFromPoints"/>>.
    /// </summary>
    public class RandomPointOnLineFromPointsConstructor : PredefinedConstructorBase
    {
        #region Private fields

        /// <summary>
        /// The randomness provider.
        /// </summary>
        private readonly IRandomnessProvider _provider;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomPointOnLineFromPointsConstructor"/> class.
        /// </summary>
        /// <param name="provider">The randomness provided.</param>
        public RandomPointOnLineFromPointsConstructor(IRandomnessProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        #endregion

        #region PredefinedConstructorBase implementation

        /// <summary>
        /// Performs the actual construction of an analytic object based on the analytic objects given as an input.
        /// The order of the objects of the input is based on the <see cref="Arguments.FlattenedList"/>.
        /// </summary>
        /// <param name="input">The analytic objects to be used as an input.</param>
        /// <returns>The constructed analytic object, if the construction was successful; or null otherwise.</returns>
        protected override AnalyticObject Construct(AnalyticObject[] input)
        {
            // Get the points
            var point1 = (Point) input[0];
            var point2 = (Point) input[1];

            // Construct the result using the passed randomness provider
            return new Line(point1, point2).RandomPointOnLine(_provider);
        }

        #endregion
    }
}