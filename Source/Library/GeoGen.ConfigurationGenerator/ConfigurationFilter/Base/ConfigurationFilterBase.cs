using GeoGen.Core;
using GeoGen.Utilities;

namespace GeoGen.ConfigurationGenerator
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

        /// <inheritdoc/>
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
            // Call the utility helper that does the job
            => EnumUtilities.ParseEnumValueFromClassName<ConfigurationFilterType>(GetType(), classNamePrefix: "ConfigurationFilter");

        #endregion

        #region IConfigurationFilter methods

        /// <inheritdoc/>
        public abstract bool ShouldBeExcluded(Configuration configuration);

        #endregion
    }
}