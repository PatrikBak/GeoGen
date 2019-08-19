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
        /// Gets or sets the derived theorems.
        /// </summary>
        public Theorem DerivedTheorem { get; set; }

        /// <summary>
        /// Gets the theorem that was used to come up with this one.
        /// </summary>
        public Theorem TemplateTheorem { get; set; }

        /// <summary>
        /// Gets or sets the list of all objects equalities that were needed to derive this theorem.
        /// This property is currently not used and is here for possible further use.
        /// </summary>
        public List<(ConfigurationObject originalObject, ConfigurationObject equalObject)> UsedEqualities { get; set; }
    }
}