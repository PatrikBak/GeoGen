using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString;

namespace GeoGen.Generator.ConstructingObjects.Arguments.ArgumentsToString
{
    /// <summary>
    /// A default implementation of <see cref="IArgumentToStringProvider"/>
    /// that uses a given <see cref="IObjectToStringProvider"/> and
    /// cashes results.
    /// </summary>
    internal abstract class ArgumentToStringProviderBase : IArgumentToStringProvider
    {
        #region Private fields

        /// <summary>
        /// The default interset separator.
        /// </summary>
        private const string DefaultIntersetSeparator = ";";

        /// <summary>
        /// The interset separator.
        /// </summary>
        private readonly string _intersetSeparator;

        /// <summary>
        /// The object to string provider.
        /// </summary>
        private readonly IObjectToStringProvider _objectToString;

        #endregion

        #region Protected fields

        /// <summary>
        /// The cache dictionary mapping argument's ids to their string versions.
        /// </summary>
        protected readonly Dictionary<int, string> Cache = new Dictionary<int, string>();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a new argument to string provider that uses a given
        /// object to string provider and a given interset separator.
        /// This constructor is supposed to be used only in testing. 
        /// </summary>
        /// <param name="objectToString">The object to string provider.</param>
        /// <param name="intersetSeparator">The interset separator.</param>
        protected ArgumentToStringProviderBase(IObjectToStringProvider objectToString, string intersetSeparator)
        {
            _objectToString = objectToString ?? throw new ArgumentNullException(nameof(objectToString));
            _intersetSeparator = intersetSeparator ?? throw new ArgumentNullException(nameof(intersetSeparator));
        }

        /// <summary>
        /// Constructs a new argument to string provider that uses a given
        /// object to string provider.
        /// </summary>
        /// <param name="objectToString">The object to string provider.</param>
        protected ArgumentToStringProviderBase(IObjectToStringProvider objectToString)
            : this(objectToString, DefaultIntersetSeparator)
        {
        }

        #endregion

        #region IArgumentToString methods

        /// <summary>
        /// Converts a given argument to string.
        /// </summary>
        /// <param name="argument">The construction argument.</param>
        /// <returns>The string represtantation of the argument.</returns>
        public string ConvertArgument(ConstructionArgument argument)
        {
            if (argument == null)
                throw new ArgumentNullException(nameof(argument));

            // Let the abstract method resolve the cached value
            var result = ResolveCachedValue(argument);

            // If there is any non-empty cached value, return it immediately
            if (result != string.Empty)
                return result;

            // If we have an object argument
            if (argument is ObjectConstructionArgument objectArgument)
            {
                // We let the object to string provider convert the passed object first
                result = _objectToString.ConvertToString(objectArgument.PassedObject);

                // Let the abstract method handle the cached result
                HandleResult(argument, result);

                // And return it
                return result;
            }

            // Otherwise we must have the set argument
            var setArgument = argument as SetConstructionArgument ?? throw new GeneratorException("Unhandled case.");

            // We recursively convert individual arguments
            var individualArgs = setArgument.PassedArguments
                    .Select(ConvertArgument)
                    .ToList();

            // We assure that the set is unique when we sort the representations of particular arguments
            individualArgs.Sort();

            // Compose the result
            result = $"{{{string.Join(_intersetSeparator, individualArgs)}}}";

            // Let the abstract method handle the cached result
            HandleResult(argument, result);

            // Return the result
            return result;
        }

        #endregion

        #region Protected abstract methods

        /// <summary>
        /// Resolves if a given argument has it's string value already cached.
        /// </summary>
        /// <param name="constructionArgument">The construction argument.</param>
        /// <returns>The cached value, if exists, otherwise an empty string.</returns>
        protected abstract string ResolveCachedValue(ConstructionArgument constructionArgument);

        /// <summary>
        /// Handles the resulting string value after constructing it, before returning it.
        /// </summary>
        /// <param name="constructionArgument">The .</param>
        /// <param name="result">The object's string value.</param>
        protected abstract void HandleResult(ConstructionArgument constructionArgument, string result);

        #endregion
    }
}