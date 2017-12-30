using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Parameters;

namespace GeoGen.Core.Constructions.PredefinedConstructions
{
    public class CircumcircleFromPoints : PredefinedConstruction
    {
        public override IReadOnlyList<ConstructionParameter> ConstructionParameters { get; }

        public override IReadOnlyList<ConfigurationObjectType> OutputTypes { get; }

        public CircumcircleFromPoints()
        {
            ConstructionParameters = new List<ConstructionParameter>
            {
                new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 3)
            };

            OutputTypes = new List<ConfigurationObjectType> {ConfigurationObjectType.Circle};
        }
    }
}