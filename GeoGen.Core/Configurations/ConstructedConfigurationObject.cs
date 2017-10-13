using System;
using System.Collections.Generic;
using GeoGen.Core.Constructions;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Utilities;

namespace GeoGen.Core.Configurations
{
    /// <summary>
    /// Represent a constructed <see cref="ConfigurationObject"/>. It's defined by a <see cref="Construction"/>,
    /// the list of <see cref="ConstructionArgument"/>s that have been passed to the construction, and an index
    /// (the construction could have more output objects).
    /// </summary>
    public sealed class ConstructedConfigurationObject : ConfigurationObject
    {
        #region Public properties

        /// <summary>
        /// Gets the construction that created this object.
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
        /// Constructs a new constructed configuration object. This object is defined by 
        /// a construction, a list of passed arguments and an index that corresponds 
        /// to the object type in the output type list of the construction. 
        /// </summary>
        /// <param name="construction">The construction.</param>
        /// <param name="arguments">The passed arguments.</param>
        /// <param name="index">The index.</param>
        public ConstructedConfigurationObject(Construction construction, IReadOnlyList<ConstructionArgument> arguments, int index)
        {
            Construction = construction ?? throw new ArgumentNullException(nameof(construction));
            PassedArguments = arguments ?? throw new ArgumentNullException(nameof(arguments));

            if (PassedArguments.Empty())
                throw new ArgumentException("Passed arguments can't be empty.");

            if (index < 0 || index >= Construction.OutputTypes.Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Index must be in range [0, Construction.OutputTypes.Count - 1].");

            Index = index;
            ObjectType = Construction.OutputTypes[index];
        }

        #endregion
    }
}