﻿using GeoGen.AnalyticGeometry;
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

        /// <inheritdoc/>
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