using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Utilities;

namespace GeoGen.Generator.Constructing.Arguments.ArgumentsToString
{
    /// <summary>
    /// An implementation of <see cref="IArgumentsToStringProvider"/>. It uses Id of configuration objects
    /// so it's supposed to be set to the objects inside arguments before use.
    /// </summary>
    public class ArgumentsToStringProvider : IArgumentsToStringProvider
    {
        #region Private fields

        /// <summary>
        /// The default arguments separator.
        /// </summary>
        private const string DefaultSeparator = ",";

        /// <summary>
        /// The arguments separator.
        /// </summary>
        private readonly string _separator;

        #endregion

        #region Constructors

        /// <summary>
        /// Construct an argument to string provider with a given separator. 
        /// This constructor is meant to be used in testing.
        /// </summary>
        /// <param name="separator">The separator.</param>
        public ArgumentsToStringProvider(string separator)
        {
            _separator = separator;
        }

        /// <summary>
        /// Construct an argument to string provider with the default separator.
        /// </summary>
        public ArgumentsToStringProvider()
            : this(DefaultSeparator)
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
            if (arguments == null)
                throw new ArgumentNullException(nameof(arguments));

            if (arguments.Empty())
                throw new ArgumentException("The arguments list can't be empty.");

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

        #endregion
    }
}