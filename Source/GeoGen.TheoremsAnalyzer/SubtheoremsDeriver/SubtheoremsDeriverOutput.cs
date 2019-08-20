using GeoGen.Core;
using System.Collections.Generic;

namespace GeoGen.TheoremsAnalyzer
{
    /// <summary>
    /// Represents an output of the <see cref="ISubtheoremsDeriver"/>.
    /// </summary>
    public class SubtheoremsDeriverOutput
    {
        /// <summary>
        /// Gets or sets the list of pairs of derived and template theorems (i.e. ones that
        /// have been used to come up with the derived ones).
        /// </summary>
        public List<(Theorem derivedTheorem, Theorem templateTheorem)> DerivedTheorems { get; set; }

        /// <summary>
        /// Gets or sets the list of all objects equalities that were needed to derive this theorem.
        /// This property is currently not used and is here for possible further use.
        /// </summary>
        public List<(ConfigurationObject originalObject, ConstructedConfigurationObject equalObject)> UsedEqualities { get; set; }
    }
}