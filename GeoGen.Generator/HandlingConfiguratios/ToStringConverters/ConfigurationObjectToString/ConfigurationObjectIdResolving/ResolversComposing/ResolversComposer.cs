using System;

namespace GeoGen.Generator
{
    internal class ResolversComposer : IResolversComposer
    {
        private readonly IDictionaryObjectIdResolversContainer _dictionaryResolversContainer;

        public ResolversComposer(IDictionaryObjectIdResolversContainer dictionaryResolversContainer)
        {
            _dictionaryResolversContainer = dictionaryResolversContainer;
        }

        public IObjectIdResolver Compose(IObjectIdResolver first, IObjectIdResolver second)
        {
            if (first == null)
                throw new ArgumentNullException(nameof(first));

            if (second == null)
                throw new ArgumentNullException(nameof(second));

            if (first is IDefaultObjectIdResolver)
                return second;

            if (second is IDefaultObjectIdResolver)
                return first;

            var dictionaryResolver1 = (DictionaryObjectIdResolver) first;
            var dictionaryResolver2 = (DictionaryObjectIdResolver) second;

            return _dictionaryResolversContainer.Compose(dictionaryResolver1, dictionaryResolver2);
        }
    }
}