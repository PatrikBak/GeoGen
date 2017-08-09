using System.Collections.Generic;
using GeoGen.Core.Constructions;
using GeoGen.Core.Constructions.Arguments;

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
        /// The arguments that have been passed to the construction. 
        /// </summary>
        public IReadOnlyList<ConstructionArgument> PassedArguments { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new constructed configuration object from a given construction and arguments passed to it. 
        /// There's no validaton that passed arguments match the actual construction signature. The <see cref="ConfigurationObjectType"/>
        /// is determined from the constrution.
        /// </summary>
        /// <param name="construction"></param>
        /// <param name="passedArguments"></param>
        public ConstructedConfigurationObject(Construction construction, IReadOnlyList<ConstructionArgument> passedArguments)
            : base(construction.OutputType)
        {
            Construction = construction;
            PassedArguments = passedArguments;
        }

        #endregion
    }
}