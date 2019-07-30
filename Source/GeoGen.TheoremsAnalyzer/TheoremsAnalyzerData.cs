using GeoGen.Core;
using System.Collections.Generic;

namespace GeoGen.TheoremsAnalyzer
{
    /// <summary>
    /// Represents the data used by <see cref="TheoremsAnalyzer"/>.
    /// </summary>
    public class TheoremsAnalyzerData
    {
        /// <summary>
        /// The template theorems for <see cref="ISubtheoremAnalyzer"/>.
        /// </summary>
        public IReadOnlyList<Theorem> TemplateTheorems { get; set; }
    }
}
