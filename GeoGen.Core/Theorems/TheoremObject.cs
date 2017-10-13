using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;

namespace GeoGen.Core.Theorems
{
    /// <summary>
    /// A base sealed class that represented a geometrical object used in <see cref="Theorem"/>.
    /// The idea behind this sealed class that we might have a theorem about a line that we don't
    /// have physically in the configuration (just two of its points). 
    /// </summary>
    public sealed class TheoremObject
    {
        #region Public properties

        public TheoremObjectSignature Type { get; }

        /// <summary>
        /// Gets the objects that this objects is made of. For example: If this is 
        /// a line, then this could for example a list of two points, or a list...
        /// </summary>
        public List<ConfigurationObject> InternalObjects { get; }

        #endregion

        public TheoremObject(IEnumerable<ConfigurationObject> objects, TheoremObjectSignature type)
        {
            Type = type;
            InternalObjects = objects.ToList();
        }

        public TheoremObject(ConfigurationObject configurationObject)
            : this(new List<ConfigurationObject> {configurationObject}, TheoremObjectSignature.SingleObject)
        {
        }
    }
}