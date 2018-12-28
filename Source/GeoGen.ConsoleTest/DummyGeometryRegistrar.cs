using GeoGen.Analyzer;
using GeoGen.Core;

namespace GeoGen.ConsoleTest
{
    public class DummyGeometryRegistrar : IGeometryRegistrar
    {
        public IObjectsContainersManager GetContainersManager(Configuration configuration)
        {
            return null;
        }

        public RegistrationResult Register(Configuration configuration)
        {
            return new RegistrationResult();
        }
    }
}