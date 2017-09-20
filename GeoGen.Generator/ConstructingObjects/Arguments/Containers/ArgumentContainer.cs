using System;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Utilities.StringBasedContainer;
using GeoGen.Generator.ConstructingObjects.Arguments.ArgumentsToString;

namespace GeoGen.Generator.ConstructingObjects.Arguments.Containers
{
    /// <summary>
    /// A default implementation of <see cref="IArgumentsListContainer"/> as an 
    /// extension of <see cref="StringBasedContainer{T}"/>, where T is 
    /// <see cref="ConstructionArgument"/>.
    /// </summary>
    internal class ArgumentContainer : StringBasedContainer<ConstructionArgument>, IArgumentContainer
    {
        private readonly DefaultArgumentToStringProvider _defaultProvider;

        public ArgumentContainer(DefaultArgumentToStringProvider defaultProvider)
        {
            _defaultProvider = defaultProvider ?? throw new ArgumentNullException(nameof(defaultProvider));
        }

        public ConstructionArgument AddArgument(ConstructionArgument argument)
        {
            var stringRepresentation = ItemToString(argument);

            if (Items.ContainsKey(stringRepresentation))
                return Items[stringRepresentation];

            var newId = Items.Count + 1;
            argument.Id = newId;
            Items.Add(stringRepresentation, argument);
            _defaultProvider.CacheArgument(newId, stringRepresentation);

            return argument;
        }

        protected override string ItemToString(ConstructionArgument item)
        {
            return _defaultProvider.ConvertArgument(item);
        }
    }
}