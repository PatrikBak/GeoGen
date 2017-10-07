using GeoGen.Core.Utilities;

namespace GeoGen.Analyzer.Theorems
{
    internal interface ITheoremVerifier
    {
        VerifierOutput Verify(ConfigurationObjectsMap oldObjects, ConfigurationObjectsMap newObjects);
    }
}