using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;

namespace GeoGen.Core.Utilities.ArgumentsToString
{
    public class ArgumentToStringProvider : IArgumentToStringProvider
    {
        private string _separator;

        private Func<ConfigurationObject, string> _objectToString;

        public string ConvertToString(IReadOnlyList<ConstructionArgument> arguments, string separator,
                                      Func<ConfigurationObject, string> objectToString)
        {
            _separator = separator;
            _objectToString = objectToString;

            return string.Join(separator, arguments.Select(ArgumentToString));
        }

        private string ArgumentToString(ConstructionArgument constructionArgument)
        {
            if (constructionArgument is ObjectConstructionArgument objectArgument)
            {
                return _objectToString(objectArgument.PassedObject);
            }

            var setArgument = constructionArgument as SetConstructionArgument ?? throw new NullReferenceException();

            var individualArgs = setArgument.PassedArguments.Select(ArgumentToString).
            ToList();
            individualArgs.Sort();

            return $"{{{string.Join(_separator, individualArgs)}}}";
        }
    }
}