using System;
using System.Collections.Generic;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a set of <see cref="ConstructionArgument"/>. The type of passed arguments 
    /// might be a <see cref="ObjectConstructionArgument"/>, or another set of arguments.
    /// It's size is not supposed to be 1, since it's either an <see cref="ObjectConstructionArgument"/>, 
    /// or a set within a set (which doesn't make sense in our context). The use-case for this are 
    /// constructions like Midpoint (with the signature {P, P}), or Intersection (with the signature { {P, P}, {P, P} }),
    /// where curly braces represents a set and P represents a point. 
    /// </summary>
    public class SetConstructionArgument : ConstructionArgument
    {
        #region Public properties

        /// <summary>
        /// Gets the list containing all passed arguments.
        /// </summary>
        public IReadOnlyList<ConstructionArgument> PassedArguments { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="passedArguments">The passed arguments</param>
        public SetConstructionArgument(IReadOnlyList<ConstructionArgument> passedArguments)
        {
            PassedArguments = passedArguments ?? throw new ArgumentNullException(nameof(passedArguments));
        }

        #endregion

        #region Overridden methods

        /// <summary>
        /// Executes an action on the configuration objects that are contained.
        /// inside the argument.
        /// </summary>
        /// <param name="action">The action to be performed on each object.</param>
        public override void Visit(Action<ConfigurationObject> action)
        {
            // Visit all internal arguments
            foreach (var argument in PassedArguments)
            {
                argument.Visit(action);
            }
        }

        #endregion
    }
}