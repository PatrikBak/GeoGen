using System;
using System.Collections;
using System.Collections.Generic;
using GeoGen.Core.Configurations;

namespace GeoGen.Generator.Constructing.Container
{
    internal class ConstructionsContainer : IConstructionsContainer
    {
        public IEnumerator<ConstructionWrapper> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public Dictionary<ConfigurationObjectType, int> GetObjectTypeToCountsMap(Core.Constructions.Construction construction)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
