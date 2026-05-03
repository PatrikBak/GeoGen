using GeoGen.AnalyticGeometry;
using GeoGen.Core;

namespace GeoGen.Constructor
{
    /// <summary>
    /// The <see cref="IObjectConstructor"/> for <see cref="PredefinedConstructionType.CircleWithRadius"/>>.
    /// </summary>
    public class CircleWithRadiusConstructor : PredefinedConstructorBase
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CircleWithRadiusConstructor"/> class.
        /// </summary>
        /// <param name="tracer">The tracer for unexpected analytic exceptions.</param>
        public CircleWithRadiusConstructor(IConstructorFailureTracer tracer)
            : base(tracer)
        {
        }

        #endregion

        #region Construct implementation

        /// <inheritdoc/>
        protected override IAnalyticObject Construct(IAnalyticObject[] input)
        {
            // Get the points
            var A = (Point)input[0];
            var B = (Point)input[1];
            var C = (Point)input[2];

            // Perform the construction
            return new Circle(A, B.DistanceTo(C));
        }

        #endregion
    }
}
