using System;
using System.Collections;
using System.Collections.Generic;
using GeoGen.Core.Constructions.Arguments;

namespace GeoGen.Generator.Constructor.Arguments.Container
{
    internal class ArgumentsContainer : IArgumentsContainer
    {
        private readonly HashSet<IReadOnlyList<ConstructionArgument>> _internalSet;

        public ArgumentsContainer(IEqualityComparer<IReadOnlyList<ConstructionArgument>> construcionArgumentsComparer)
        {
            if (construcionArgumentsComparer == null)
                throw new ArgumentNullException(nameof(construcionArgumentsComparer));

            _internalSet = new HashSet<IReadOnlyList<ConstructionArgument>>(construcionArgumentsComparer);
        }

        public IEnumerator<IReadOnlyList<ConstructionArgument>> GetEnumerator()
        {
            return _internalSet.GetEnumerator();
        }

        public void Add(IReadOnlyList<ConstructionArgument> arguments)
        {
            _internalSet.Add(arguments);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}