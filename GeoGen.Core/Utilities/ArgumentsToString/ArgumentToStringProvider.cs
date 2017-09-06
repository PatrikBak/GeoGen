using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Constructions.Arguments;

namespace GeoGen.Core.Utilities.ArgumentsToString
{
    /// <summary>
    /// An implementation of <see cref="IArgumentToStringProvider"/>. It uses Id of configuration objects
    /// so it's supposed to be set to the objects inside arguments before use.
    /// </summary>
    public class ArgumentToStringProvider : IArgumentToStringProvider
    {
        /// <summary>
        /// The default arguments separator.
        /// </summary>
        private const string DefaultSeparator = ",";

        /// <summary>
        /// The arguments separator.
        /// </summary>
        private readonly string _separator;

        /// <summary>
        /// Construct an argument to string provider with a given separator. 
        /// This constructor is meant to be used in testing.
        /// </summary>
        /// <param name="separator">The separator.</param>
        public ArgumentToStringProvider(string separator)
        {
            _separator = separator;
        }

        /// <summary>
        /// Construct an argument to string provider with the default separator.
        /// </summary>
        public ArgumentToStringProvider()
            : this(DefaultSeparator)
        {
        }

        /// <summary>
        /// Converts a given list of construction arguments to string. 
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        /// <returns>The string representation of the list.</returns>
        public string ConvertToString(IReadOnlyList<ConstructionArgument> arguments)
        {
            if(arguments == null)
                throw new ArgumentNullException(nameof(arguments));

            if(arguments.Empty())
                throw new ArgumentException("The arguments list can't be empty.");

            // TODO: Debug method to check unique IDS
            return string.Join(_separator, arguments.Select(ArgumentToString));
        }

        /// <summary>
        /// Converts a given construction argument to string.
        /// </summary>
        /// <param name="constructionArgument">The given construction argument.</param>
        /// <returns>The string representation of the argument.</returns>
        private string ArgumentToString(ConstructionArgument constructionArgument)
        {
            if (constructionArgument is ObjectConstructionArgument objectArgument)
            {
                return objectArgument.PassedObject.Id.ToString();
            }

            var setArgument = constructionArgument as SetConstructionArgument ?? throw new NullReferenceException();

            var individualArgs = setArgument.PassableArguments.Select(ArgumentToString).ToList();
            individualArgs.Sort();

            return $"{{{string.Join(_separator, individualArgs)}}}";
        }
    }
}