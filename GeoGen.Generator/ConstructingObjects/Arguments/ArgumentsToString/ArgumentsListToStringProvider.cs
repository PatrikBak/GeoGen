using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Utilities;

namespace GeoGen.Generator.ConstructingObjects.Arguments.ArgumentsToString
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
        /// The default argument to string provider.
        /// </summary>
        private readonly DefaultArgumentToStringProvider _provider;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs an arguments list to string provider with
        /// a given default argument to string provider and an
        /// arguments separator.
        /// </summary>
        /// <param name="provider">The default argument to string provider.</param>
        /// <param name="argumentsSeparator">The arguments separator.</param>
        public ArgumentsListToStringProvider(DefaultArgumentToStringProvider provider, string argumentsSeparator)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _argumentsSeparator = argumentsSeparator;
        }

        /// <summary>
        /// Constructs an arguments list to string provider with a given
        /// default argument to string provider. 
        /// </summary>
        /// <param name="provider">The default argument to string provider.</param>
        public ArgumentsListToStringProvider(DefaultArgumentToStringProvider provider)
            : this(provider, DefaultArgumentsSeparator)
        {
        }

        #endregion

        #region IArgumentsListToStringProvider methods

        /// <summary>
        /// Converts a given list of construction arguments to string,
        /// using a default argument to string provider.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        /// <returns>The string representation of the list.</returns>
        public string ConvertToString(IReadOnlyList<ConstructionArgument> arguments)
        {
            return ConvertToString(arguments, _provider);
        }

        /// <summary>
        /// Converts a given list of construction arguments to string, 
        /// using  a given construction argument to string provider.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        /// <param name="provider">The argument to string provider.</param>
        /// <returns>The string representation of the list.</returns>
        public string ConvertToString(IReadOnlyList<ConstructionArgument> arguments, IArgumentToStringProvider provider)
        {
            if (arguments == null)
                throw new ArgumentNullException(nameof(arguments));

            if (arguments.Empty())
                throw new ArgumentException("The arguments list can't be empty.");

            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            // Create arguments string enumerable
            var argumentsStrings = arguments.Select(provider.ConvertArgument);

            // Join arguments
            return $"({string.Join(_argumentsSeparator, argumentsStrings)})";
        }

        #endregion
    }
}