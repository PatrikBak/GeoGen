using System;
using System.Collections.Generic;

namespace GeoGen.Core
{
    /// <summary>
    /// Represent a constructed <see cref="ConfigurationObject"/>. It's defined by a construction
    /// the list of <see cref="ConstructionArgument"/>s that matches the construction signature, and 
    /// an index (the construction could have more output objects).
    /// </summary>
    public class ConstructedConfigurationObject : ConfigurationObject
    {
        #region Public properties

        /// <summary>
        /// Gets the construction that creates this object.
        /// </summary>
        public Construction Construction { get; }

        /// <summary>
        /// Gets the index of this object in the construction output list.
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// Gets the arguments that have been passed to the construction. 
        /// </summary>
        public IReadOnlyList<ConstructionArgument> PassedArguments { get; }

        #endregion

        #region Configuration Object properties

        /// <summary>
        /// Gets the actual geometrical type of this object (such as Point, Line...)
        /// </summary>
        public override ConfigurationObjectType ObjectType { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="construction">The construction.</param>
        /// <param name="arguments">The passed arguments to the construction.</param>
        /// <param name="index">The index indicating which output of the construction this object is.</param>
        public ConstructedConfigurationObject(Construction construction, IReadOnlyList<ConstructionArgument> arguments, int index)
        {
            Construction = construction ?? throw new ArgumentNullException(nameof(construction));
            PassedArguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
            Index = index;
            ObjectType = Construction.OutputTypes[index];
        }

        #endregion

        #region Overridden methods

        /// <summary>
        /// Executes an action on the configuration objects that are used to define this one
        /// (including this one). The action might get call for the same object more than once.
        /// </summary>
        /// <param name="action">The action to be performed on each object.</param>
        public override void Visit(Action<ConfigurationObject> action)
        {
            // Call the action on this object
            action(this);

            // Visit the objects within the arguments
            foreach (var argument in PassedArguments)
            {
                argument.Visit(action);
            }
        }

        #endregion
    }
}