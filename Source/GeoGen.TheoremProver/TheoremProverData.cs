using GeoGen.Core;
using System;
using System.Collections.Generic;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Represents the data used by <see cref="TheoremProver"/>.
    /// </summary>
    public class TheoremProverData
    {
        #region Public properties

        /// <summary>
        /// The list of configurations together with their theorems that are supposed to be 
        /// known and used to derive new theorems via <see cref="ISubtheoremDeriver"/>.
        /// </summary>
        public IReadOnlyList<(Configuration, TheoremsMap)> TemplateTheorems { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremProverData"/> class.
        /// </summary>
        /// <param name="templateTheorems">The list of template configurations together with their theorems.</param>
        public TheoremProverData(IReadOnlyList<(Configuration, TheoremsMap)> templateTheorems)
        {
            TemplateTheorems = templateTheorems ?? throw new ArgumentNullException(nameof(templateTheorems));
        }

        #endregion
    }
}
