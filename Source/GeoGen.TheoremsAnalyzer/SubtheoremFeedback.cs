using GeoGen.Core;
using System.Collections.Generic;

namespace GeoGen.TheoremsAnalyzer
{
    /// <summary>
    /// The <see cref="TheoremFeedback"/> that a theorem is a consequence of another theorem.
    /// </summary>
    public class SubtheoremFeedback : TheoremFeedback
    {
        /// <summary>
        /// Gets or sets the theorem that implies the examined one.
        /// </summary>
        public Theorem TemplateTheorem { get; set; }
    }
}
