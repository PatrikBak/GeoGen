using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Parameters;
using GeoGen.Core.Utilities;

namespace GeoGen.Core.Constructions
{
    /// <summary>
    /// Represents a composed <see cref="Construction"/> that is defined as an output of some configuration. 
    /// This is supposed to represent a complex constructions that user can define by themselves, for instance
    /// the construction that takes 3 points and outputs the orthocenter of the triangle formed by those points.
    /// </summary>
    public sealed class ComposedConstruction : Construction
    {
        #region Public properties

        /// <summary>
        /// Gets the parental configuration of the construction.
        /// </summary>
        public Configuration ParentalConfiguration { get; }

        /// <summary>
        /// Gets the constructed configuration objects that represents a configuration output. 
        /// </summary>
        public List<ConstructedConfigurationObject> ConstructionOutput { get; }

        #endregion

        #region Construction properties

        /// <summary>
        /// Gets the construction signature, i.e. the unmodifiable list of construction parameters.
        /// </summary>
        public override IReadOnlyList<ConstructionParameter> ConstructionParameters { get; }

        /// <summary>
        /// Gets the construction output signature, i.e. the unmodifiable list of configuration object types.
        /// </summary>
        public override IReadOnlyList<ConfigurationObjectType> OutputTypes { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new composed construction from a given parental configuration, a set
        /// of indices of objects that are supposed be the output of the construction signature, 
        /// and a signature of the construction. It must be possible to pass the loose configuration
        /// objects of the parental configuration to the signature. This may or may not be order-Dependant.
        /// (the incenter of a triangle is order-independent, unlike an excenter). 
        /// </summary>
        public ComposedConstruction
        (
            Configuration parentalConfiguration,
            ISet<int> outputObjectsIndices,
            IReadOnlyList<ConstructionParameter> constructionParameters
        )
        {
            ParentalConfiguration = parentalConfiguration ?? throw new ArgumentNullException(nameof(parentalConfiguration));

            if (outputObjectsIndices == null)
                throw new ArgumentNullException(nameof(outputObjectsIndices));

            if (outputObjectsIndices.Empty())
                throw new ArgumentException("Output object indices can't be empty.");

            try
            {
                ConstructionOutput = outputObjectsIndices.Select(i => parentalConfiguration.ConstructedObjects[i]).ToList();
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ArgumentException("Incorrect indices, can't retrieve constructed objects.");
            }

            OutputTypes = ConstructionOutput.Select(o => o.ObjectType).ToList();

            ConstructionParameters = constructionParameters ?? throw new ArgumentNullException(nameof(constructionParameters));

            if (constructionParameters.Empty())
                throw new ArgumentException("Construction parameters can't be empty");
        }

        #endregion
    }
}