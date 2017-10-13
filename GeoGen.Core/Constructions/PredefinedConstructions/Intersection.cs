using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Parameters;

namespace GeoGen.Core.Constructions.PredefinedConstructions
{
    /// <summary>
    /// Represents an intersection construction of two lines given by two sets of points.
    /// </summary>
    public sealed class Intersection : PredefinedConstruction
    {
        #region Construction properties

        /// <summary>
        /// Gets the construction input signature, i.e. the unmodifiable list of construction parameters.
        /// </summary>
        public override IReadOnlyList<ConstructionParameter> ConstructionParameters { get; }

        /// <summary>
        /// Gets the construction output signature, i.e. the unmodifiable list of configuration object types.
        /// </summary>
        public override IReadOnlyList<ConfigurationObjectType> OutputTypes { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public Intersection()
        {
            ConstructionParameters = new List<ConstructionParameter>
            {
                new SetConstructionParameter
                (
                    new SetConstructionParameter
                    (
                        new ObjectConstructionParameter(ConfigurationObjectType.Point), 2
                    ), 2
                )
            };

            OutputTypes = new List<ConfigurationObjectType> {ConfigurationObjectType.Point};
        }

        #endregion
    }
}