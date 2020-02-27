using GeoGen.Core;
using GeoGen.Utilities;
using System;

namespace GeoGen.Generator
{
    /// <summary>
    /// The base class for implementations of <see cref="IConfigurationFilter"/>.
    /// </summary>
    public abstract class ConfigurationFilterBase : IConfigurationFilter
    {
        #region Protected fields

        /// <summary>
        /// The input for the generator.
        /// </summary>
        protected readonly ConfigurationGeneratorInput GeneratorInput;

        #endregion

        #region IConfigurationFilter properties

        /// <summary>
        /// The type of the filter.
        /// </summary>
        public ConfigurationFilterType Type { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationFilterBase"/> class.
        /// </summary>
        /// <param name="generatorInput">The input for the generator.</param>
        protected ConfigurationFilterBase(ConfigurationGeneratorInput generatorInput)
        {
            GeneratorInput = generatorInput ?? throw new ArgumentNullException(nameof(generatorInput));

            // Find the type
            Type = FindTypeFromClassName();
        }

        #endregion

        #region Finding type from the class name

        /// <summary>
        /// Infers the type of the filter from the class name. The class name should be in the form {rule}ConfigurationFilter.
        /// </summary>
        /// <returns>The inferred type.</returns>
        private ConfigurationFilterType FindTypeFromClassName()
        {
            // Call the utility helper that does the job
            return EnumUtilities.ParseEnumValueFromClassName<ConfigurationFilterType>(GetType(), classNamePrefix: "ConfigurationFilter");
        }

        #endregion

        #region IConfigurationFilter methods

        /// <summary>
        /// Finds out if the configuration should be excluded by the algorithm, because it is 
        /// not the representant of the equivalence class of equal configurations.
        /// </summary>
        /// <param name="configuration">The configuration that should be tested for exclusion.</param>
        /// <returns>true, if the configuration should be excluded; false otherwise.</returns>
        public abstract bool ShouldBeExcluded(Configuration configuration);

        #endregion
    }
}