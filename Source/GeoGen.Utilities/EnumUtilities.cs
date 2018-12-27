using System;
using System.Text.RegularExpressions;

namespace GeoGen.Utilities
{
    /// <summary>
    /// Static utilities for <see cref="Enum"/> types.
    /// </summary>
    public static class EnumUtilities
    {
        /// <summary>
        /// Parses an enum value from a given type that represents a class name, that is supposed
        /// to be in the form {result}{classNamePrefix}, where the prefix is given as a parameter.
        /// For example: For class 'EqualAngelsVerifier' and the prefix 'Verifier' the result
        /// should be 'EqualAngels', as the value of the given enum type T (that must of course exist).
        /// </summary>
        /// <typeparam name="T">The enum type.</typeparam>
        /// <param name="type">The class type.</param>
        /// <param name="classNamePrefix">The expected prefix of the class name.</param>
        /// <returns>The parsed value.</returns>
        public static T ParseEnumValueFromClassName<T>(Type type, string classNamePrefix) where T : struct, Enum
        {
            // Construct the regex with one group that catches the name
            var regex = new Regex($"^(.*){classNamePrefix}");

            // Get the class name
            var className = type.Name;

            // Do the matching
            var match = regex.Match(className);

            // If we failed, we want to make the developer aware
            if (!match.Success)
                throw new Exception($"The class {className} doesn't match the name pattern '{{type}}{classNamePrefix}'");

            // Otherwise we pull the supposed type name
            var typeName = match.Groups[1].Value;

            // Try to parse (without ignoring the cases)
            var parsingSuccessful = Enum.TryParse(typeName, ignoreCase: false, out T result);

            // If the parsing failed, we want to make the developer aware
            if (!parsingSuccessful)
                throw new Exception($"Unable to parse the name '{typeName}' (inferred from the {className}) into a value of {typeof(T)}.");

            // Otherwise we're fine
            return result;
        }
    }
}
