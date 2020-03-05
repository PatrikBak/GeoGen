using GeoGen.AnalyticGeometry;
using GeoGen.TheoremRanker;

namespace GeoGen.TheoremSorter
{
    /// <summary>
    /// Represents a tracer of failures while running <see cref="TheoremSorter"/>.
    /// </summary>
    public interface ISortingGeometryFailureTracer
    {
        /// <summary>
        /// Traces that while examining a given ranked theorem there has been a problem with an object that
        /// could not be constructed (and should have been).
        /// </summary>
        /// <param name="rankedTheorem">The ranked theorem that had been examined.</param>
        void TraceInconstructibleObject(RankedTheorem rankedTheorem);

        /// <summary>
        /// Traces that while examining a given ranked theorem there has been a problem with an isomorphic
        /// version of that theorem that couldn't be constructed analytically.
        /// </summary>
        /// <param name="rankedTheorem">The ranked theorem that had been examined.</param>
        /// <param name="exception">The exception that has been thrown while drawing.</param>
        void TraceInconstructibleTheorem(RankedTheorem rankedTheorem, AnalyticException exception);
    }
}
