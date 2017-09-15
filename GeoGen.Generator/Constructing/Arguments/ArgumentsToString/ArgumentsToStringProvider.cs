using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Utilities;
using GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString;
using GeoGen.Generator.Constructing.Arguments.Container;

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
        /// The arguments separator.
        /// </summary>
        private readonly string _argumentsSeparator;

        /// <summary>
        /// The default configuration object to string provider.
        /// </summary>
        private readonly DefaultObjectToStringProvider _objectToString;

        private readonly IArgumentToStringProviderFactory _argumentToStringFactory;

        #endregion

        #region Constructors

        /// <summary>
        /// Construct an argument to string provider with a given arguments
        /// separator, a given interset separator and a given default
        /// configuration object to string provider. This constructor is 
        /// meant to be used in testing.
        /// </summary>
        /// <param name="objectToString">The configuration object to string.</param>
        /// <param name="argumentsSeparator">The arguments separator.</param>
        /// TODO: FIX DOC
        public ArgumentsToStringProvider
        (
            IArgumentToStringProviderFactory argumentToStringFactory,
            DefaultObjectToStringProvider objectToString,
            string argumentsSeparator
        )
        {
            _objectToString = objectToString;
            _argumentsSeparator = argumentsSeparator;
            _argumentToStringFactory = argumentToStringFactory;
        }

        public ArgumentsToStringProvider
        (
            IArgumentToStringProviderFactory argumentToStringFactory,
            DefaultObjectToStringProvider objectToString
        )
            : this(argumentToStringFactory, objectToString, DefaultArgumentsSeparator)
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
            return ConvertToString(arguments, _objectToString);
        }

        /// <summary>
        /// Converts a given list of construction arguments to string, using
        /// a provided configuration object to string provider.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        /// <param name="objectToString">The configuration object to string provider.</param>
        /// <returns>The string representation of the list.</returns>
        public string ConvertToString(IReadOnlyList<ConstructionArgument> arguments, IObjectToStringProvider objectToString)
        {
            if (arguments == null)
                throw new ArgumentNullException(nameof(arguments));

            if (arguments.Empty())
                throw new ArgumentException("The arguments list can't be empty.");

            if (objectToString == null)
                throw new ArgumentNullException(nameof(objectToString));

            // Ask factory for the right container
            var argumentContainer = _argumentToStringFactory.GetProvider(objectToString);

            // Create arguments string enumerable
            var argumentsStrings = arguments.Select(arg => argumentContainer.ConvertArgument(arg));

            // Join arguments
            return $"({string.Join(_argumentsSeparator, argumentsStrings)})";
        }

        #endregion
    }
}