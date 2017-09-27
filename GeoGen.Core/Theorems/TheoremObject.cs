using System.Collections.Generic;
using GeoGen.Core.Configurations;

namespace GeoGen.Core.Theorems
{
    /// <summary>
    /// A base class that represented a geometrical object used in <see cref="Theorem"/>.
    /// The idea behind this class that we might have a theorem about a line that we don't
    /// have physically in the configuration (just two of its points). 
    /// </summary>
    public abstract class TheoremObject
    {
        #region Public properties

        /// <summary>
        /// Gets the type of this object.
        /// </summary>
        public abstract ConfigurationObjectType Type { get; }

        /// <summary>
        /// Gets the objects that this objects is made of. For example: If this is 
        /// a line, then this could for example a list of two points, or a list...
        /// </summary>
        public abstract List<ConfigurationObject> InternalObjects { get; }

        #endregion
    }
}