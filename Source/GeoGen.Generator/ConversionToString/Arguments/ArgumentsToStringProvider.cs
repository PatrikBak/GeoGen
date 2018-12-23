using GeoGen.Core;
using System;
using System.Linq;

namespace GeoGen.Generator
{
    /// <summary>
    /// A default implementation of <see cref="IArgumentsToStringProvider"/>.
    /// It converts given arguments by converting individual arguments and then joining
    /// them by a separator. The individual arguments are converted like this: 
    /// Object arguments are simply converted according to provided <see cref="IObjectToStringConverter"/>.
    /// Set arguments are converted by converting individual arguments and then sorting the results
    /// to obtain the unique result. 
    /// </summary>
    public class ArgumentsToStringProvider : IArgumentsToStringProvider
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
        /// Converts given arguments to string, using a given configuration object to string provider.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        /// <param name="objectToString">The configuration object to string converter.</param>
        /// <returns>The string representation the arguments.</returns>
        public string ConvertToString(Arguments arguments, IToStringConverter<ConfigurationObject> objectToString)
        {
            // Local function that converts a single argument to string
            string ArgumentToString(ConstructionArgument constructionArgument)
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
                        .Select(ArgumentToString)
                        .ToList();

                // Sort them to obtain the unique result independent on the order
                individualArgs.Sort();

                // And join them using the arguments set separator
                return $"{{{string.Join(ArgumentsSetSeparator, individualArgs)}}}";
            }

            // We convert individual arguments to string
            var argumentsStrings = arguments.Select(ArgumentToString);

            // Return converted arguments joined with the arguments list separator
            return $"({string.Join(ArgumentsListSeparator, arguments.Select(ArgumentToString))})";
        }

        #endregion
    }
}