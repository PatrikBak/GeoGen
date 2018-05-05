using System;
using System.Collections.Generic;

namespace GeoGen.Core
{
    /// <summary>
    /// Represent a constructed <see cref="ConfigurationObject"/>. It's defined by a <see cref="Construction"/>,
    /// <see cref="Arguments"/> that match the construction signature, and an index (the construction could have more output objects).
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
        public Arguments PassedArguments { get; }

        #endregion

        #region Configuration Object properties

        /// <summary>
        /// Gets the actual geometrical type of this object (such as Point, Line...)
        /// </summary>
        public override ConfigurationObjectType ObjectType { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor that uses arguments.
        /// </summary>
        /// <param name="construction">The construction.</param>
        /// <param name="arguments">The passed arguments to the construction.</param>
        /// <param name="index">The index indicating which output of the construction this object is, the default vaulue is 0.</param>
        public ConstructedConfigurationObject(Construction construction, Arguments arguments, int index = 0)
        {
            Construction = construction ?? throw new ArgumentNullException(nameof(construction));
            PassedArguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
            Index = index;
            ObjectType = Construction.OutputTypes[index];
        }

        /// <summary>
        /// Default constructor that uses an arguments list.
        /// </summary>
        /// <param name="construction">The construction.</param>
        /// <param name="arguments">The passed arguments to the construction.</param>
        /// <param name="index">The index indicating which output of the construction this object is, the default value is 0.</param>
        public ConstructedConfigurationObject(Construction construction, IReadOnlyList<ConstructionArgument> arguments, int index = 0)
                : this(construction, new Arguments(arguments), index)
        {
        }

        #endregion
    }
}