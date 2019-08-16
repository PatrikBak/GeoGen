using GeoGen.Core;
using GeoGen.Utilities;
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
                // Switch based on the argument type
                return argument switch
                {
                    // If we have an object argument, then we simply convert its object
                    // using the passed converter
                    ObjectConstructionArgument objectArgument => objectToString.ConvertToString(objectArgument.PassedObject),

                    // For set argument we wrap the result in curly braces, convert the 
                    // inner arguments, order them, and join with the separator
                    SetConstructionArgument setArgument => $"{{{setArgument.PassedArguments.Select(ArgumentToString).Ordered().ToJoinedString(ArgumentsSetSeparator)}}}",

                    // Default
                    _ => throw new GeneratorException($"Unhandled type of construction argument: {argument.GetType()}"),
                };
            }

            // Convert individual arguments using our local function, join them using the
            // list separator, and wrap into regular braces
            return $"({arguments.Select(ArgumentToString).ToJoinedString(ArgumentsListSeparator)})";
        }

        #endregion
    }
}