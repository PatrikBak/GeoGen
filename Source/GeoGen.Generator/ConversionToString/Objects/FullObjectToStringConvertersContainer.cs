using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Generator
{
    class Comparer : IEqualityComparer<LooseConfigurationObject>
    {
        public bool Equals(LooseConfigurationObject x, LooseConfigurationObject y)
        {
            return x == y;
        }

        public int GetHashCode(LooseConfigurationObject obj)
        {
            return 1;
        }
    }

    public class FullObjectToStringConvertersContainer : IFullObjectToStringConvertersContainer
    {
        #region Dependencies

        private readonly IFullObjectToStringConverterFactory _factory;
        private readonly IVariationsProvider _provider;

        #endregion

        #region Private fields

        private readonly LooseObjectsHolder _looseObjectsHolder;

        /// <summary>
        /// The list of all the converters.
        /// </summary>
        private readonly List<IFullObjectToStringConverter> _converters;

        #endregion

        #region IFullObjectToStringConvertersContainer properties

        public IFullObjectToStringConverter DefaultFullConverter { get; }

        #endregion

        #region Constructor
        
        public FullObjectToStringConvertersContainer(LooseObjectsHolder looseObjectsHolder, IFullObjectToStringConverterFactory factory, IVariationsProvider provider)
        {
            _looseObjectsHolder = looseObjectsHolder ?? throw new ArgumentNullException(nameof(looseObjectsHolder));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));

            // Create the default converter using the identity resolver
            //DefaultFullConverter = _factory.CreateConverter(new IdentityLooseObjectIdResolver());
            DefaultFullConverter = _factory.CreateConverter(new DictionaryLooseObjectIdResolver(
                looseObjectsHolder.LooseObjects.ToDictionary(obj => obj, new Comparer())
                ));

            // Make sure it's in the list
            _converters = new List<IFullObjectToStringConverter> { DefaultFullConverter };
        }

        #endregion

        #region Private methods

        private void EnsureConvertersAreCreated()
        {
            // If there is any other than the default converter, then they have been created yet
            if (_converters.Count != 1)
                return;

            // Otherwise create a conversation for each permutations of the loose objects
            foreach (var permutation in _provider.GetVariations(_looseObjectsHolder.LooseObjects, _looseObjectsHolder.LooseObjects.Count))
            {
                // Create the dictionary mapping the loose objects to the 
                var dictionary = permutation.ToDictionary((looseObject, index) => looseObject, (looseObject, index) => _looseObjectsHolder.LooseObjects[index]);

                // If the dictionary identically maps each other to itself, 
                // then we have an equalivalnt to the default converter, 
                // so we can skip it
                if (dictionary.All(pair => pair.Key.Id == pair.Value.Id))
                    continue;

                // Otherwise create the full converter for it through the factory
                var converter = _factory.CreateConverter(new DictionaryLooseObjectIdResolver(dictionary));

                // Add it
                _converters.Add(converter);                
            }
        }

        #endregion

        #region IEnumerable implementation

        public IEnumerator<IToStringConverter<ConfigurationObject>> GetEnumerator()
        {
            // First ensure the converters are created
            EnsureConvertersAreCreated();

            // Then return the enumerator for their list
            return _converters.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

    }
}
