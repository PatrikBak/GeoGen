using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Utilities;

namespace GeoGen.Generator
{
    /// <summary>
    /// A default implementation of <see cref="IArgumentsListToStringConverter"/>.
    /// It creates a unique representation of arguments lists by sorting the 
    /// string representations of the sets arguments. Since we don't expect
    /// to have long lists or arguments, this should be fast enough. 
    /// This sealed class is thread-safe.
    /// </summary>
    internal sealed class ArgumentsListToStringConverter : IArgumentsListToStringConverter
    {
        #region Private constants

        /// <summary>
        /// The arguments list separator.
        /// </summary>
        private const string ArgumentsListSeparator = ",";

        /// <summary>
        /// The arguments set separator.
        /// </summary>
        private const string ArgumentsSetSeparator = ";";

        #endregion

        #region IArgumentsToStringProvider methods

        /// <summary>
        /// Converts a given list of construction arguments to string, using
        /// a given configuration object to string provider.
        /// </summary>
        /// <param name="arguments">The arguments list.</param>
        /// <param name="objectToString">The configuration object to string provider.</param>
        /// <returns>The string representation of the list.</returns>
        public string ConvertToString(IReadOnlyList<ConstructionArgument> arguments, IObjectToStringConverter objectToString)
        {
            if (arguments == null)
                throw new ArgumentNullException(nameof(arguments));

            if (arguments.Empty())
                throw new ArgumentException("The arguments list can't be empty.");

            if (objectToString == null)
                throw new ArgumentNullException(nameof(objectToString));

            // We convert individual arguments to string
            var argumentsStrings = arguments.Select(args => ArgumentToString(args, objectToString));

            // And join them using the arguments list separator
            return $"({string.Join(ArgumentsListSeparator, argumentsStrings)})";
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Converts a given construction argument to string.
        /// </summary>
        /// <param name="constructionArgument">The construction argument.</param>
        /// <param name="objectToString">The configuration object to string provider.</param>
        /// <returns>The string representation of the argument.</returns>
        private string ArgumentToString(ConstructionArgument constructionArgument, IObjectToStringConverter objectToString)
        {
            // If we have an object argument
            if (constructionArgument is ObjectConstructionArgument objectArgument)
            {
                // Then we simply converts it's passed object to string
                return objectToString.ConvertToString(objectArgument.PassedObject);
            }

            // Otherwise we have a set construction argument
            var setArgument = (SetConstructionArgument) constructionArgument;

            // We'll recursively covert it's individual arguments to string
            var individualArgs = setArgument.PassedArguments
                    .Select(arg => ArgumentToString(arg, objectToString))
                    .ToList();

            // Sort them to obtain the unique result
            individualArgs.Sort();

            // And compose the result
            return $"{{{string.Join(ArgumentsSetSeparator, individualArgs)}}}";
        }

        #endregion
    }
}