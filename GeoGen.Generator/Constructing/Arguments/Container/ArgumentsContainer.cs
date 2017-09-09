using System;
using System.Collections;
using System.Collections.Generic;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Generator.Constructing.Arguments.ArgumentsToString;

namespace GeoGen.Generator.Constructing.Arguments.Container
{
    /// <summary>
    /// An implementation of <see cref="IArgumentsContainer"/> that uses <see cref="IArgumentToStringProvider"/>.
    /// It basically compares two ConstructionArguments such that there are first converted to the string 
    /// (in a unique way) and then these strings are compared. It turns out to be pretty fast. 
    /// </summary>
    internal class ArgumentsContainer : IArgumentsContainer
    {
        #region Private fields

        /// <summary>
        /// The container's content collection.
        /// </summary>
        private readonly List<IReadOnlyList<ConstructionArgument>> _distinctArguments = new List<IReadOnlyList<ConstructionArgument>>();

        /// <summary>
        /// The set of string versions of all arguments of the collection.
        /// </summary>
        private readonly HashSet<string> _argumentsStringHashes = new HashSet<string>();

        /// <summary>
        /// The argument to string provider.
        /// </summary>
        private readonly IArgumentToStringProvider _argumentToStringProvider;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new arguments container that uses a given argument to string provider.
        /// </summary>
        /// <param name="argumentToStringProvider">The argument to string provider.</param>
        public ArgumentsContainer(IArgumentToStringProvider argumentToStringProvider)
        {
            _argumentToStringProvider = argumentToStringProvider ?? throw new ArgumentNullException(nameof(argumentToStringProvider));
        }

        #endregion

        #region IArgumentsContainer methods

        /// <summary>
        /// Adds arguments to the container.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        public void Add(IReadOnlyList<ConstructionArgument> arguments)
        {
            if (_argumentsStringHashes.Add(_argumentToStringProvider.ConvertToString(arguments)))
            {
                _distinctArguments.Add(arguments);
            }
        }

        /// <summary>
        /// Clears the container.
        /// </summary>
        public void Clear()
        {
            _distinctArguments.Clear();
            _argumentsStringHashes.Clear();
        }

        /// <summary>
        /// Checks if the container contains given arguments.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        /// <returns>true, if the container container the arguments, false otherwise.</returns>
        public bool Contains(IReadOnlyList<ConstructionArgument> arguments)
        {
            return _distinctArguments.Contains(arguments);
        }

        #endregion

        #region IEnumerable methods

        public IEnumerator<IReadOnlyList<ConstructionArgument>> GetEnumerator()
        {
            return _distinctArguments.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}