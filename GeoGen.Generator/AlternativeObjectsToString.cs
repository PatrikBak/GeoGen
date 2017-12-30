using System.Collections.Generic;
using GeoGen.Core.Configurations;

namespace GeoGen.Generator
{
    internal class AlternativeObjectsToString : IDefaultFullObjectToStringConverter
    {
        public IObjectIdResolver Resolver { get; }

        public readonly IArgumentsListToStringConverter _converter;

        private readonly IDefaultObjectToStringConverter _objectToString;

        public AlternativeObjectsToString(IArgumentsListToStringConverter listToString, IDefaultObjectToStringConverter objectToString)
        {
            Resolver = objectToString.Resolver;
            _converter = listToString;
            _objectToString = objectToString;
        }

        private readonly Dictionary<int, string> _cache = new Dictionary<int, string>();

        public string ConvertToString(ConfigurationObject item)
        {
            if (item is LooseConfigurationObject)
            {
                var r = _objectToString.ConvertToString(item);

                return r;
            }

            //var id = item.Id;

            //if (id != null &&  _cache.ContainsKey(id.Value))
            //    return _cache[id.Value];

            var constructedItem = (ConstructedConfigurationObject) item;

            var args = _converter.ConvertToString(constructedItem.PassedArguments, _objectToString);

            var result = $"{constructedItem.Construction.Id}{args}";

            //Console.WriteLine($"Simple: {result}, Full: {s}");

            return result;
        }
        
        public void CacheObject(int configurationObjectId, string stringVersion)
        {
            _cache.Add(configurationObjectId, stringVersion);
        }
    }
}
