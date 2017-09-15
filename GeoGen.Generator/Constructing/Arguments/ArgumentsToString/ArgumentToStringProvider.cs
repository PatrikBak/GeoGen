using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString;

namespace GeoGen.Generator.Constructing.Arguments.ArgumentsToString
{
    /// <summary>
    /// A default implementation of <see cref="IArgumentToStringProvider"/>
    /// that uses a given <see cref="IObjectToStringProvider"/> and
    /// cashes results.
    /// </summary>
    internal class ArgumentToStringProvider : IArgumentToStringProvider
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

        /// <summary>
        /// The cache dictionary mapping argument's ids to their string versions.
        /// </summary>
        private readonly Dictionary<int, string> _cache = new Dictionary<int, string>();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a new argument to string provider that uses a given
        /// object to string provider and a given interset separator.
        /// This constructor is supposed to be used only in testing. 
        /// </summary>
        /// <param name="objectToString">The object to string provider.</param>
        /// <param name="intersetSeparator">The interset separator.</param>
        public ArgumentToStringProvider(IObjectToStringProvider objectToString, string intersetSeparator)
        {
            _objectToString = objectToString ?? throw new ArgumentNullException(nameof(objectToString));
            _intersetSeparator = intersetSeparator ?? throw new ArgumentNullException(nameof(intersetSeparator));
        }

        /// <summary>
        /// Constructs a new argument to string provider that uses a given
        /// object to string provider.
        /// </summary>
        /// <param name="objectToString">The object to string provider.</param>
        public ArgumentToStringProvider(IObjectToStringProvider objectToString)
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

            var hasId = argument.Id.HasValue;

            // If the argument has an id, we first check if it's not cached
            if (hasId)
            {
                var id = argument.Id.Value;

                if (_cache.ContainsKey(id))
                    return _cache[id];
            }

            string result;

            if (argument is ObjectConstructionArgument objectArgument)
            {
                // We let the object to string provider convert the passed object first
                result = _objectToString.ConvertToString(objectArgument.PassedObject);

                // If we have an id, we can cache
                if (hasId)
                    _cache.Add(argument.Id.Value, result);

                return result;
            }

            var setArgument = argument as SetConstructionArgument ?? throw new GeneratorException("Unhandled case.");

            var individualArgs = setArgument.PassedArguments
                    .Select(ConvertArgument)
                    .ToList();

            // We assure that the set is unique when we sort the representations of particular arguments
            individualArgs.Sort();

            result = $"{{{string.Join(_intersetSeparator, individualArgs)}}}";

            if (hasId)
                _cache.Add(argument.Id.Value, result);

            return result;
        }

        /// <summary>
        /// Caches a given string representation associated with the argument
        /// with an given id. We call this after converting an argument that
        /// didn't have an id while it was being converted.
        /// </summary>
        /// <param name="argumentId">The argument id.</param>
        /// <param name="stringRepresentation">The string representation.</param>
        public void Cache(int argumentId, string stringRepresentation)
        {
            if (_cache.ContainsKey(argumentId))
                throw new GeneratorException("The argument with this id has already been cached.");

            _cache.Add(argumentId, stringRepresentation);
        }

        #endregion
    }
}