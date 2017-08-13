using System;
using System.Collections;
using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions;

namespace GeoGen.Generator.Constructor
{
    internal class ConstructionsContainer : IConstructionsContainer
    {
        public IEnumerator<Construction> GetEnumerator()
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
