using System.Collections;
using System.Collections.Generic;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Utilities.ArgumentsToString;

namespace GeoGen.Generator.Constructing.Arguments.Container
{
    internal class ArgumentsContainer : IArgumentsContainer
    {
        private readonly List<IReadOnlyList<ConstructionArgument>> _distinctArguments = new List<IReadOnlyList<ConstructionArgument>>();

        private readonly HashSet<string> _argumentsStringHashes = new HashSet<string>();

        private readonly IArgumentToStringProvider _argumentToStringProvider;

        public ArgumentsContainer(IArgumentToStringProvider argumentToStringProvider)
        {
            _argumentToStringProvider = argumentToStringProvider;
        }

        public IEnumerator<IReadOnlyList<ConstructionArgument>> GetEnumerator()
        {
            return _distinctArguments.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(IReadOnlyList<ConstructionArgument> arguments)
        {
            if (_argumentsStringHashes.Add(_argumentToStringProvider.ConvertToString(arguments)))
            {
                _distinctArguments.Add(arguments);
            }
        }

        public void Clear()
        {
            _distinctArguments.Clear();
            _argumentsStringHashes.Clear();
        }
    }
}