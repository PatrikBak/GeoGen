using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a <see cref="Construction"/> that is defined as an output of some configuration. 
    /// This is supposed to represent a complex constructions that user can define by themselves, for instance
    /// the construction that takes 3 points and outputs the orthocenter of the triangle formed by those points.
    /// The output of a construction is defined using a <see cref="Configuration"/>. The actual 
    /// <see cref="ConstructionArgument"/>s will be mapped to the loose objects of the configuration.
    /// Some objects of this configuration will be then taken as the output of the construction.
    /// </summary>
    public class ComposedConstruction : Construction
    {
        #region Public properties

        /// <summary>
        /// Gets the parental configuration of the construction.
        /// </summary>
        public Configuration ParentalConfiguration { get; }

        /// <summary>
        /// Gets the constructed configuration objects that represent the output of this construction.
        /// </summary>
        public List<ConstructedConfigurationObject> ConstructionOutput { get; }

        /// <summary>
        /// Gets or sets the function that takes the list of constructed objects (that are created with
        /// this construction) and construct the list of default theorems that holds true for them 
        /// (usually combined with the input objects from the arguments passed to them).
        /// </summary>
        public Func<IReadOnlyList<ConstructedConfigurationObject>, List<Theorem>> DefaultTheoresFuncton { get; set; }

        #endregion

        #region Construction properties

        /// <summary>
        /// Gets the construction input signature, i.e. the list of construction parameters.
        /// </summary>
        public override IReadOnlyList<ConstructionParameter> ConstructionParameters { get; }

        /// <summary>
        /// Gets the construction output signature, i.e. the list of configuration object types.
        /// </summary>
        public override IReadOnlyList<ConfigurationObjectType> OutputTypes { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="configuration">The configuration that represents the composed construction.</param>
        /// <param name="indices">The indices of the outputted objects that are from the constructed objects of the given configuration.</param>
        /// <param name="parameters">The parameters of the construction (the signature must correspond to the loose objects of the configuration).</param>
        public ComposedConstruction(Configuration configuration, IEnumerable<int> indices, IReadOnlyList<ConstructionParameter> parameters)
        {
            ParentalConfiguration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            ConstructionParameters = parameters ?? throw new ArgumentNullException(nameof(parameters));

            // Find constructed objects in the configuration according to the indices enumerable
            ConstructionOutput = indices.Select(i => configuration.ConstructedObjects[i]).ToList();

            // Find the types of outputted objects
            OutputTypes = ConstructionOutput.Select(o => o.ObjectType).ToList();
        }

        #endregion
    }
}