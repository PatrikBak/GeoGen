using System;

namespace GeoGen.Generator
{
    internal class ResolversComposer : IResolversComposer
    {
        private readonly IDictionaryObjectIdResolversContainer _dictionaryResolversContainer;

        private readonly IDefaultObjectIdResolver _defaultObjectIdResolver;

        public ResolversComposer(IDictionaryObjectIdResolversContainer dictionaryResolversContainer, IDefaultObjectIdResolver defaultObjectIdResolver)
        {
            _dictionaryResolversContainer = dictionaryResolversContainer;
            _defaultObjectIdResolver = defaultObjectIdResolver;
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

            var result = _dictionaryResolversContainer.Compose(dictionaryResolver1, dictionaryResolver2);

            //return result;
            return result.Identity ? (IObjectIdResolver) _defaultObjectIdResolver : result;
        }
    }
}