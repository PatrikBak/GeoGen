using System;
using System.Collections.Generic;
using GeoGen.Core.Constructions;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Utilities;

namespace GeoGen.Core.Configurations
{
    /// <summary>
    /// Represent a constructed <see cref="ConfigurationObject"/>. It's defined by a <see cref="Constructions.Construction"/>
    /// and the list of <see cref="ConstructionArgument"/>s that have been passed to the construction.
    /// </summary>
    public class ConstructedConfigurationObject : ConfigurationObject
    {
        #region Public properties

        /// <summary>
        /// The construction that created this object.
        /// </summary>
        public Construction Construction { get; }

        /// <summary>
        /// Gets the index of this object in the construction output list.
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// The arguments that have been passed to the construction. 
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
        /// Constructs a new constructed configuration object from a given construction, arguments passed to it
        /// and a given type. There's no validaton that passed arguments match the actual construction signature. 
        /// </summary>
        /// <param name="construction">The construction.</param>
        /// <param name="passedArguments">The passed arguments.</param>
        /// <param name="index">The index of the output type.</param>
        public ConstructedConfigurationObject(Construction construction, IReadOnlyList<ConstructionArgument> passedArguments, int index)
        {
            Construction = construction ?? throw new ArgumentNullException(nameof(construction));
            PassedArguments = passedArguments ?? throw new ArgumentNullException(nameof(passedArguments));

            if (PassedArguments.Empty())
                throw new ArgumentException("Passed arguments can't be empty.");

            if (index < 0 || index >= Construction.OutputTypes.Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Index must be in range [0, Construction.OutputTypes.Count - 1].");

            Index = index;
            ObjectType = Construction.OutputTypes[index];

            // TODO: Debug check if the arguments are passable to the construction signature.
        }

        #endregion
    }
}