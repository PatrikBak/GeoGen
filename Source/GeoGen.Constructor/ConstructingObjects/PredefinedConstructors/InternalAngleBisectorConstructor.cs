using GeoGen.AnalyticGeometry;
using GeoGen.Core;

namespace GeoGen.Constructor
{
    /// <summary>
    /// The <see cref="IObjectConstructor"/> for <see cref="PredefinedConstructionType.InternalAngleBisector"/>>.
    /// </summary>
    public class InternalAngleBisectorConstructor : PredefinedConstructorBase
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CircleWithCenterThroughPointConstructor"/> class.
        /// </summary>
        /// <param name="tracer">The tracer for unexpected analytic exceptions.</param>
        public InternalAngleBisectorConstructor(IConstructorFailureTracer tracer)
            : base(tracer)
        {
        }

        #endregion

        #region Construct implementation

        /// <summary>
        /// Performs the actual construction of an analytic object based on the analytic objects given as an input.
        /// The order of the objects of the input is based on the <see cref="Arguments.FlattenedList"/>.
        /// </summary>
        /// <param name="input">The analytic objects to be used as an input.</param>
        /// <returns>The constructed analytic object, if the construction was successful; or null otherwise.</returns>
        protected override IAnalyticObject Construct(IAnalyticObject[] input)
        {
            // Get the points
            var A = (Point)input[0];
            var B = (Point)input[1];
            var C = (Point)input[2];

            // If they are collinear, don't perform the construction
            // (it would be possible, but it would be unnecessarily 
            // equivalent to the perpendicular line construction)
            if (AnalyticHelpers.AreCollinear(A, B, C))
                return null;

            // Otherwise construct the result
            return AnalyticHelpers.InternalAngleBisector(A, B, C);
        }

        #endregion
    }
}