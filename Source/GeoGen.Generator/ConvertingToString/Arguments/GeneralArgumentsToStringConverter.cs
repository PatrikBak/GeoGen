using GeoGen.Core;
using System.Linq;

namespace GeoGen.Generator
{
    /// <summary>
    /// The default implementation of <see cref="IGeneralArgumentsToStringConverter"/>.
    /// It converts given arguments by converting individual arguments, joining
    /// them with the ',' separator, and finally wrapping them into braces. Individual 
    /// arguments are converted like this: Object arguments are converted according to 
    /// the passed object to string converter. Set arguments are converted by converting
    /// individual arguments, sorting them to obtain an order-independent result, joining 
    /// them with the ';' separator, and finally wrapping them inside curly braces.
    /// </summary>
    public class GeneralArgumentsToStringConverter : IGeneralArgumentsToStringConverter
    {
        #region Private constants

        /// <summary>
        /// The separator of individual converted arguments in the arguments list.
        /// </summary>
        private const string ArgumentsListSeparator = ",";

        /// <summary>
        /// The separator of individual converted arguments within a set of arguments.
        /// </summary>
        private const string ArgumentsSetSeparator = ";";

        #endregion

        #region IGeneralArgumentsToStringConverter implementation

        /// <summary>
        /// Converts given arguments to a string, using a given configuration object to string converter.
        /// </summary>
        /// <param name="arguments">The arguments to be converted.</param>
        /// <param name="objectToString">The configuration object to string converter.</param>
        /// <returns>A string representation of the arguments.</returns>
        public string ConvertToString(Arguments arguments, IToStringConverter<ConfigurationObject> objectToString)
        {
            // Local function that converts a single argument to a string
            string ArgumentToString(ConstructionArgument argument)
            {
                // If we have an object argument
                if (argument is ObjectConstructionArgument objectArgument)
                {
                    // Then we simply convert its object using the passed object to string converter
                    return objectToString.ConvertToString(objectArgument.PassedObject);
                }

                // Otherwise we have a set argument
                var setArgument = (SetConstructionArgument)argument;

                // We'll recursively convert its individual arguments to strings
                var individualArgs = setArgument.PassedArguments.Select(ArgumentToString).ToList();

                // Sort them to obtain the unique result independent on the order
                individualArgs.Sort();

                // Join them using the set separator a wrap into curly braces
                return $"{{{string.Join(ArgumentsSetSeparator, individualArgs)}}}";
            }

            // Convert individual arguments using our local function, join them using the list separator, and wrap into regular braces
            return $"({string.Join(ArgumentsListSeparator, arguments.Select(ArgumentToString))})";
        }

        #endregion
    }
}