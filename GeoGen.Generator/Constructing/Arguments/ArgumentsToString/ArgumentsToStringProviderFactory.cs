using System;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString;

namespace GeoGen.Generator.Constructing.Arguments.ArgumentsToString
{
    internal class ArgumentsToStringProviderFactory : IArgumentsToStringProviderFactory
    {
        public IArgumentsToStringProvider CreateProvider(DefaultObjectToStringProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            return new ArgumentsToStringProvider(provider);
        }
    }
}