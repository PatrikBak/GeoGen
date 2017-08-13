using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions;

namespace GeoGen.Generator.Constructor
{
    internal interface IConstructionsContainer : IEnumerable<Construction>
    {
        Dictionary<ConfigurationObjectType, int> GetObjectTypeToCountsMap(Construction construction);
    }
}