using GeoGen.Core;
using System.Collections.Generic;

namespace GeoGen.ConfigurationGenerator
{
    /// <summary>
    /// An implementation of <see cref="IConfigurationFilter"/> for <see cref="ConfigurationFilterType.Simple"/>.
    /// </summary>
    public class SimpleConfigurationFilter : ConfigurationFilterBase
    {
        #region Private fields

        /// <summary>
        /// The set of already generated configurations.
        /// </summary>
        private readonly HashSet<Configuration> _generatedConfigurations = new HashSet<Configuration>();

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleConfigurationFilter"/> class.
        /// </summary>
        /// <param name="generatorInput">The input for the generator.</param>
        public SimpleConfigurationFilter(ConfigurationGeneratorInput generatorInput) : base(generatorInput)
        {
        }

        #endregion

        #region Public abstract methods implementation

        /// <inheritdoc/>
        public override bool ShouldBeExcluded(Configuration configuration)
        {
            // If the configuration is in the set, then yes
            if (_generatedConfigurations.Contains(configuration))
                return true;

            // Otherwise add the configuration to the set
            _generatedConfigurations.Add(configuration);

            // And it is new, i.e. it should not be excluded
            return false;
        }

        #endregion        
    }
}