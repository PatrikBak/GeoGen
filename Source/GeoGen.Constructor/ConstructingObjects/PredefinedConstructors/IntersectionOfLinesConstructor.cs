using GeoGen.AnalyticGeometry;
using GeoGen.Core;

namespace GeoGen.Constructor
{
    /// <summary>
    /// The <see cref="IObjectsConstructor"/> for <see cref="PredefinedConstructionType.IntersectionOfLines"/>>.
    /// </summary>
    public class IntersectionOfLinesConstructor : PredefinedConstructorBase
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CircleWithCenterThroughPointConstructor"/> class.
        /// </summary>
        /// <param name="tracer">The tracer for unexpected analytic exceptions.</param>
        public IntersectionOfLinesConstructor(IConstructorFailureTracer tracer = null)
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
            // Get the lines
            var l = (Line)input[0];
            var m = (Line)input[1];

            // Intersect them. If there is no intersection, the result will be null
            return l.IntersectionWith(m);
        }

        #endregion
    }
}