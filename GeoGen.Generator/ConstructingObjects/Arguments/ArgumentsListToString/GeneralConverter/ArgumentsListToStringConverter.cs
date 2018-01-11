using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Utilities;

namespace GeoGen.Generator
{
    /// <summary>
    /// A default implementation of <see cref="IArgumentsListToStringProvider"/>.
    /// It converts a given list by converting individual arguments and then joining
    /// them by a separator. The individual arguments are converted like this: 
    /// Object arguments are simply converted according to provided <see cref="IObjectToStringConverter"/>.
    /// Set arguments are converted by converting individual arguments and then sorting the results
    /// to obtain the unique result. 
    /// </summary>
    internal class ArgumentsListToStringProvider : IArgumentsListToStringProvider
    {
        #region Private constants

        /// <summary>
        /// The separator of individual arguments in the arguments list.
        /// </summary>
        private const string ArgumentsListSeparator = ",";

        /// <summary>
        /// The separator of arguments within a set of arguments.
        /// </summary>
        private const string ArgumentsSetSeparator = ";";

        #endregion

        #region IArgumentsToStringProvider implementation

        /// <summary>
        /// Converts a given list of construction arguments to string, using
        /// a given configuration object to string provider.
        /// </summary>
        /// <param name="arguments">The arguments list.</param>
        /// <param name="objectToString">The configuration object to string converter.</param>
        /// <returns>The string representation of the list.</returns>
        public string ConvertToString(IReadOnlyList<ConstructionArgument> arguments, IObjectToStringConverter objectToString)
        {
            // We convert individual arguments to string
            var argumentsStrings = arguments.Select(args => ArgumentToString(args, objectToString));

            // And join them using the arguments list separator
            return $"({string.Join(ArgumentsListSeparator, argumentsStrings)})";
        }

        /// <summary>
        /// Converts a single construction argument to string.
        /// </summary>
        /// <param name="constructionArgument">The construction argument.</param>
        /// <param name="objectToString">The object to string provider.</param>
        /// <returns>The string representation of the argument.</returns>
        private string ArgumentToString(ConstructionArgument constructionArgument, IObjectToStringConverter objectToString)
        {
            // If we have an object argument
            if (constructionArgument is ObjectConstructionArgument objectArgument)
            {
                // Then we simply convert it using the passed object to string converter
                return objectToString.ConvertToString(objectArgument.PassedObject);
            }

            // Otherwise we have a set construction argument
            var setArgument = (SetConstructionArgument) constructionArgument;

            // We'll recursively covert it's individual arguments to string
            var individualArgs = setArgument.PassedArguments
                    .Select(arg => ArgumentToString(arg, objectToString))
                    .ToList();

            // Sort them to obtain the unique result independent on the order
            individualArgs.Sort();

            // And join them using the arguments set separator
            return $"{{{string.Join(ArgumentsSetSeparator, individualArgs)}}}";
        }

        #endregion
    }
}