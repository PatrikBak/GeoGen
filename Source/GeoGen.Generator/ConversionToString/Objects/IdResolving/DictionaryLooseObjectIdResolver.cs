using GeoGen.Core;
using System;
using System.Collections.Generic;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents an <see cref="ILooseObjectsIdResolver"/> wrapping a dictionary mapping each object 
    /// to the id to which it should be resolved.
    /// </summary>
    public class DictionaryLooseObjectIdResolver : ILooseObjectsIdResolver
    {
        private Dictionary<LooseConfigurationObject, LooseConfigurationObject> _dictionary;

        public DictionaryLooseObjectIdResolver(Dictionary<LooseConfigurationObject, LooseConfigurationObject> dictionary)
        {
            _dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
        }

        public int ResolveId(LooseConfigurationObject looseObject)
        {
            return _dictionary[looseObject].Id;
        }
    }
}
