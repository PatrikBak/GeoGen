using System;
using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;

namespace GeoGen.Core.Utilities.ArgumentsToString
{
    public interface IArgumentToStringProvider
    {
        string ConvertToString(IReadOnlyList<ConstructionArgument> arguments, string separator, Func<ConfigurationObject, string> objectToString);
    }
}
