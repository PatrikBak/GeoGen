using GeoGen.Core;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.TheoremsAnalyzer
{
    /// <summary>
    /// Represents the data used by <see cref="TheoremsAnalyzer"/>.
    /// </summary>
    public class TheoremsAnalyzerData
    {
        /// <summary>
        /// Gets or sets the list of configurations together with their theorems 
        /// that are supposed to be known and used to derive new theorems. 
        /// The order is important because the configurations will be examined in this order
        /// (i.e. the most common sub-configurations should go earlier).
        /// </summary>
        public IReadOnlyList<(Configuration, TheoremsMap)> TemplateTheorems { get; set; }
    }
}
