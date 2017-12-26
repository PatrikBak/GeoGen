using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoGen.Core.Configurations;

namespace GeoGen.Analyzer.Drawing
{
    public interface IGeometryRegistrar
    {
        RegistrationResult Register(List<ConstructedConfigurationObject> constructedObjects);
    }
}