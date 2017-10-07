﻿using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Parameters;

namespace GeoGen.Core.Constructions.PredefinedConstructions
{
    /// <summary>
    /// Represents a construction of the midpoint between two points.
    /// </summary>
    public class Midpoint : PredefinedConstruction
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
        public Midpoint()
        {
            ConstructionParameters = new List<ConstructionParameter>
            {
                new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 2)
            };

            OutputTypes = new List<ConfigurationObjectType> {ConfigurationObjectType.Point};
        }

        #endregion
    }
}