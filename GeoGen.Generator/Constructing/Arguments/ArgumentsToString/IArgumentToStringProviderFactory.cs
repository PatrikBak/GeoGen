using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString;

namespace GeoGen.Generator.Constructing.Arguments.ArgumentsToString
{
    interface IArgumentToStringProviderFactory
    {
        IArgumentToStringProvider GetProvider(IObjectToStringProvider provider);
    }
}
