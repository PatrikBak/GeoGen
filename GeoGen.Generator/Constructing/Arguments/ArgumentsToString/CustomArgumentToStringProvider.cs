using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString;

namespace GeoGen.Generator.Constructing.Arguments.ArgumentsToString
{
    internal class CustomArgumentToStringProvider : ArgumentToStringProviderBase
    {
        public CustomArgumentToStringProvider(IObjectToStringProvider objectToString, string intersetSeparator)
            : base(objectToString, intersetSeparator)
        {
        }

        public CustomArgumentToStringProvider(IObjectToStringProvider objectToString)
            : base(objectToString)
        {
        }

        protected override string ResolveCachedValue(ConstructionArgument constructionArgument)
        {
            // We must have an id
            var id = constructionArgument.Id ?? throw new GeneratorException("Value must be set");

            // Then we might or might have cached this argument.
            return Cache.ContainsKey(id) ? Cache[id] : string.Empty;
        }

        protected override void HandleResult(ConstructionArgument constructionArgument, string result)
        {
            Cache.Add(constructionArgument.Id ?? throw new GeneratorException(), result);
        }
    }
}
