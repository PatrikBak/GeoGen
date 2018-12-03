using GeoGen.Analyzer;
using GeoGen.Core;

namespace GeoGen.Generator.IntegrationTest
{
    internal class DummyGeometryRegistrar : IGeometryRegistrar
    {
        public RegistrationResult Register(Configuration configuration)
        {
            return new RegistrationResult();
        }
    }
}