using GeoGen.Analyzer;
using GeoGen.Core;

namespace GeoGen.Generator.IntegrationTest
{
    public class DummyGeometryRegistrar : IGeometryRegistrar
    {
        public RegistrationResult Register(Configuration configuration)
        {
            return new RegistrationResult();
        }
    }
}