using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Parameters;

namespace GeoGen.Core.Constructions.PredefinedConstructions
{
    public class InternalAngelBisectorFromPoints : PredefinedConstruction
    {
        public override IReadOnlyList<ConstructionParameter> ConstructionParameters { get; }

        public override IReadOnlyList<ConfigurationObjectType> OutputTypes { get; }

        public InternalAngelBisectorFromPoints()
        {
            ConstructionParameters = new List<ConstructionParameter>
            {
                new ObjectConstructionParameter(ConfigurationObjectType.Point),
                new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 2)
            };

            OutputTypes = new List<ConfigurationObjectType> {ConfigurationObjectType.Line};
        }
    }
}