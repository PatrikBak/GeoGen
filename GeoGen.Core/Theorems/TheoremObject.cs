using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Core.Utilities;

namespace GeoGen.Core.Theorems
{
    public sealed class TheoremObject
    {
        #region Public properties

        public TheoremObjectSignature Type { get; }

        public HashSet<ConfigurationObject> InternalObjects { get; }

        #endregion

        public TheoremObject(IEnumerable<ConfigurationObject> objects, TheoremObjectSignature type)
        {
            Type = type;
            InternalObjects = objects.ToSet();
        }

        public TheoremObject(ConfigurationObject configurationObject)
            : this(new List<ConfigurationObject> {configurationObject}, TheoremObjectSignature.SingleObject)
        {
        }
    }
}