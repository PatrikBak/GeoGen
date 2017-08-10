using System;
using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Parameters;

namespace GeoGen.Core.Constructions
{
    /// <summary>
    /// Represents a composed <see cref="Construction"/> that is defined as an output of some configuration. 
    /// This is supposed to represent a complex constructions that user can define by themselves, for instance
    /// the construction that takes 3 points and returns the ortocenter of the triangle formed by those points.
    /// </summary>
    public class ComposedConstruction : Construction
    {
        #region Public properties

        /// <summary>
        /// Gets the constructed configuration object that represents a configuration output. 
        /// </summary>
        public ConstructedConfigurationObject ConfigurationOutput { get; }

        /// <summary>
        /// Gets the construction that was performed to obtain the final configuration output. 
        /// </summary>
        public Construction OutputConstruction => ConfigurationOutput.Construction;

        #endregion

        #region Construction properties

        /// <summary>
        /// Gets the output type of this construction (such as Point, Line...).
        /// </summary>
        public override ConfigurationObjectType OutputType => OutputConstruction.OutputType;

        /// <summary>
        /// Gets the construction signature, i.e. the unmodifiable list of construction parameters.
        /// </summary>
        public override IReadOnlyList<ConstructionParameter> ConstructionParameters => OutputConstruction.ConstructionParameters;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new composed construction given by the constructed configuration object representing the output. 
        /// </summary>
        /// <param name="configurationOutput">The configuration output</param>
        public ComposedConstruction(ConstructedConfigurationObject configurationOutput)
        {
            ConfigurationOutput = configurationOutput ?? throw new ArgumentNullException(nameof(configurationOutput));
        }

        #endregion
    }
}