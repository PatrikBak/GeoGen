using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Utilities;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString;

namespace GeoGen.Generator.Constructing.Arguments.ArgumentsToString
{
    /// <summary>
    /// A default implementation of <see cref="IArgumentsListToStringProvider"/>. 
    /// </summary>
    internal class ArgumentsListToStringProvider : IArgumentsListToStringProvider
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
        /// The argument to string provider factory.
        /// </summary>
        private readonly ICustomArgumentToStringProviderFactory _customArgumentToStringFactory;

        /// <summary>
        /// The default argument to string provider.
        /// </summary>
        private DefaultArgumentToStringProvider _provider;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs an arguments list to string provider with a given
        /// argument to string provider factory, a default argument to 
        /// string provider and an arguments list to string provider.
        /// This constructor is meant to be used in testing.
        /// </summary>
        /// <param name="factory">The custom argument to string provider factory.</param>
        /// <param name="provider">The default argument to string provider.</param>
        /// <param name="argumentsSeparator">The arguments separator.</param>
        public ArgumentsListToStringProvider
        (
            ICustomArgumentToStringProviderFactory factory,
            DefaultArgumentToStringProvider provider,
            string argumentsSeparator
        )
        {
            _customArgumentToStringFactory = factory ?? throw new ArgumentNullException(nameof(factory));
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _argumentsSeparator = argumentsSeparator;
        }

        /// <summary>
        /// Constructs an arguments list to string provider with a given
        /// arguments list to string provider factory and default argument to 
        /// string provider. 
        /// </summary>
        /// <param name="factory">The custom argument to string provider factory.</param>
        /// <param name="provider">The default argument to string provider.</param>
        public ArgumentsListToStringProvider
        (
            ICustomArgumentToStringProviderFactory factory,
            DefaultArgumentToStringProvider provider
        )
            : this(factory, provider, DefaultArgumentsSeparator)
        {
        }

        #endregion

        #region IArgumentsListToStringProvider methods

        /// <summary>
        /// Converts a given list of construction arguments to string,
        /// using a default configuration object to string provider.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        /// <returns>The string representation of the list.</returns>
        public string ConvertToString(IReadOnlyList<ConstructionArgument> arguments)
        {
            return ConvertToString(arguments, _provider);
        }

        /// <summary>
        /// Converts a given list of construction arguments to string, 
        /// using  a given configuration object to string provider.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        /// <param name="objectToString">The configuration object to string provider.</param>
        /// <returns>The string representation of the list.</returns>
        public string ConvertToString(IReadOnlyList<ConstructionArgument> arguments, IObjectToStringProvider objectToString)
        {
            if (objectToString == null)
                throw new ArgumentNullException(nameof(objectToString));

            // We ask factory for the right argument to string provider
            var provider = _customArgumentToStringFactory.GetProvider(objectToString);

            // And call the private method to do the conversion
            return ConvertToString(arguments, provider);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Converts a given argument list to string using a give
        /// argument to string provider.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        /// <param name="argumentToString">The argument to string provider.</param>
        /// <returns>The string representation of the list.</returns>
        private string ConvertToString(IReadOnlyList<ConstructionArgument> arguments, IArgumentToStringProvider argumentToString)
        {
            if (arguments == null)
                throw new ArgumentNullException(nameof(arguments));

            if (arguments.Empty())
                throw new ArgumentException("The arguments list can't be empty.");

            // Create arguments string enumerable
            var argumentsStrings = arguments.Select(argumentToString.ConvertArgument);

            // Join arguments
            return $"({string.Join(_argumentsSeparator, argumentsStrings)})";
        }

        #endregion
    }
}