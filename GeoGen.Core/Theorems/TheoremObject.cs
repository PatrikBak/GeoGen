using System;
using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Core.Utilities;
using GeoGen.Utilities;

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
            InternalObjects = objects?.ToSet() ?? throw new ArgumentNullException(nameof(objects));
        }

        public TheoremObject(ConfigurationObject configurationObject)
        {
            if (configurationObject == null)
                throw new ArgumentNullException(nameof(configurationObject));

            Type = TheoremObjectSignature.SingleObject;
            InternalObjects = new HashSet<ConfigurationObject> {configurationObject};
        }
    }
}