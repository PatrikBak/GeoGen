using System;
using System.Collections;
using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions;
using GeoGen.Generator.Wrappers;

namespace GeoGen.Generator.Constructor.Container
{
    internal class ConstructionsContainer : IConstructionsContainer
    {
        public IEnumerator<ConstructionWrapper> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public Dictionary<ConfigurationObjectType, int> GetObjectTypeToCountsMap(Construction construction)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
