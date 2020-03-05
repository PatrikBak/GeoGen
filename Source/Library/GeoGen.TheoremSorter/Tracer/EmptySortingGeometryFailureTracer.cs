using GeoGen.AnalyticGeometry;
using GeoGen.TheoremRanker;

namespace GeoGen.TheoremSorter
{
    /// <summary>
    /// Represents an empty <see cref="ISortingGeometryFailureTracer"/> that does nothing.
    /// </summary>
    public class EmptySortingGeometryFailureTracer : ISortingGeometryFailureTracer
    {
        /// <inheritdoc/>
        public void TraceInconstructibleObject(RankedTheorem rankedTheorem)
        {
        }

        /// <inheritdoc/>
        public void TraceInconstructibleTheorem(RankedTheorem rankedTheorem, AnalyticException exception)
        {
        }
    }
}
