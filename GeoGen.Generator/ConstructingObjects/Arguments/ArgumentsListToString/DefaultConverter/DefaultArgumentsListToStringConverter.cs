using System;
using System.Collections.Generic;
using GeoGen.Core.Constructions.Arguments;

namespace GeoGen.Generator
{
    internal class DefaultArgumentsListToStringConverter : IDefaultArgumentsListToStringConverter
    {
        private readonly IArgumentsListToStringConverter _argumentsToString;

        private readonly IObjectToStringConverter _objectToString;

        public DefaultArgumentsListToStringConverter(IArgumentsListToStringConverter argumentsToString, IDefaultObjectToStringConverter objectToString)
        {
            _argumentsToString = argumentsToString;
            _objectToString = objectToString;
        }

        public string ConvertToString(IReadOnlyList<ConstructionArgument> item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            return _argumentsToString.ConvertToString(item, _objectToString);
        }
    }
}