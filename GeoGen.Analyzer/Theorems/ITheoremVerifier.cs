using System.Collections.Generic;
using GeoGen.Core.Theorems;
using GeoGen.Core.Utilities;

namespace GeoGen.Analyzer.Theorems
{
    internal interface ITheoremVerifier
    {
        TheoremType TheoremType { get; }

        IEnumerable<VerifierOutput> GetOutput(ConfigurationObjectsMap oldObjects, ConfigurationObjectsMap newObjects);
    }
}