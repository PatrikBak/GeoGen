using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Constructions.Arguments;

namespace GeoGen.Generator.Constructor.Arguments.Container
{
    internal class ArgumentsContainer : IArgumentsContainer
    {
        private readonly List<IReadOnlyList<ConstructionArgument>> _distinctArguments = new List<IReadOnlyList<ConstructionArgument>>();

        private readonly HashSet<string> _argumentsStringHashes = new HashSet<string>();

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
            if (_argumentsStringHashes.Add(CreateStringHash(arguments)))
            {
                _distinctArguments.Add(arguments);
            }
        }

        private static string CreateStringHash(IReadOnlyList<ConstructionArgument> arguments)
        {
            return string.Join(", ", arguments.Select(ArgToString));
        }

        private static string ArgToString(ConstructionArgument argument)
        {
            if (argument is ObjectConstructionArgument objectArgument)
            {
                return objectArgument.PassedObject.Id.ToString();
            }

            var setArgument = argument as SetConstructionArgument ?? throw new NullReferenceException();

            var individualArgs = setArgument.PassedArguments.Select(ArgToString).ToList();
            individualArgs.Sort();

            return $"{{{string.Join(",", individualArgs)}}}";
        }

        public void Clear()
        {
            _distinctArguments.Clear();
            _argumentsStringHashes.Clear();
        }
    }
}