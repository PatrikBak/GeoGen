using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Utilities;
using GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString;

namespace GeoGen.Generator.Constructing.Arguments.ArgumentsToString
{
    /// <summary>
    /// An implementation of <see cref="IArgumentsToStringProvider"/>. It defaulty 
    /// uses Id of configuration objects so it's supposed to be set to the objects 
    /// inside arguments before use.
    /// </summary>
    internal class ArgumentsToStringProvider : IArgumentsToStringProvider
    {
        #region Private fields

        /// <summary>
        /// The default arguments separator.
        /// </summary>
        private const string DefaultArgumentsSeparator = ",";

        /// <summary>
        /// The default arguments separator.
        /// </summary>
        private const string DefaultIntersetSeparator = ";";

        /// <summary>
        /// The arguments separator.
        /// </summary>
        private readonly string _argumentsSeparator;

        /// <summary>
        /// The interset separator.
        /// </summary>
        private readonly string _intersetSeparator;

        /// <summary>
        /// The default configuration object to string provider.
        /// </summary>
        private readonly DefaultConfigurationObjectToStringProvider _configurationObjectToString;

        #endregion

        #region Constructors

        /// <summary>
        /// Construct an argument to string provider with a given arguments
        /// separator, a given interset separator and a given default
        /// configuration object to string provider. This constructor is 
        /// meant to be used in testing.
        /// </summary>
        /// <param name="configurationObjectToString">The configuration object to string.</param>
        /// <param name="argumentsSeparator">The arguments separator.</param>
        /// <param name="intersetSeparator">The interset separator.</param>
        public ArgumentsToStringProvider(DefaultConfigurationObjectToStringProvider configurationObjectToString,
            string argumentsSeparator, string intersetSeparator)
        {
            _configurationObjectToString = configurationObjectToString;
            _argumentsSeparator = argumentsSeparator;
            _intersetSeparator = intersetSeparator;
        }

        /// <summary>
        /// Construct an argument to string provider with default separators
        /// and a given default configuration object to string provider.
        /// </summary>
        /// <param name="configurationObjectToString">The configuration object to string.</param>
        public ArgumentsToStringProvider(DefaultConfigurationObjectToStringProvider configurationObjectToString)
            : this(configurationObjectToString, DefaultArgumentsSeparator, DefaultIntersetSeparator)
        {
        }

        #endregion

        #region IArgumentsToStringProvider implementation

        /// <summary>
        /// Converts a given list of construction arguments to string. 
        /// Arguments must have objects with unique ids inside them
        /// (this is verified in a debug mode).
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        /// <returns>The string representation of the list.</returns>
        public string ConvertToString(IReadOnlyList<ConstructionArgument> arguments)
        {
            return ConvertToString(arguments, _configurationObjectToString);
        }

        /// <summary>
        /// Converts a given list of construction arguments to string, using
        /// a provided configuration object to string provider.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        /// <param name="objectToString">The configuration object to string provider.</param>
        /// <returns>The string representation of the list.</returns>
        public string ConvertToString(IReadOnlyList<ConstructionArgument> arguments,
            IConfigurationObjectToStringProvider objectToString)
        {
            if (arguments == null)
                throw new ArgumentNullException(nameof(arguments));

            if (arguments.Empty())
                throw new ArgumentException("The arguments list can't be empty.");

            if (objectToString == null)
                throw new ArgumentNullException(nameof(objectToString));

            var argumentsStrings = arguments.Select(args => ArgumentToString(args, objectToString));

            return $"({string.Join(_argumentsSeparator, argumentsStrings)})";
        }

        /// <summary>
        /// Converts a given construction argument to string.
        /// </summary>
        /// <param name="constructionArgument">The given construction argument.</param>
        /// <param name="objectToString">The configuration object to string converted.</param>
        /// <returns>The string representation of the argument.</returns>
        private string ArgumentToString(ConstructionArgument constructionArgument,
            IConfigurationObjectToStringProvider objectToString)
        {
            if (constructionArgument is ObjectConstructionArgument objectArgument)
            {
                return objectToString.ConvertToString(objectArgument.PassedObject);
            }

            var setArgument = constructionArgument as SetConstructionArgument ?? throw new NullReferenceException();

            var individualArgs = setArgument.PassedArguments
                    .Select(arg => ArgumentToString(arg, objectToString))
                    .ToList();

            individualArgs.Sort();

            return $"{{{string.Join(_intersetSeparator, individualArgs)}}}";
        }

        #endregion
    }
}