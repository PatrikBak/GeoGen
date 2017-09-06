using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Parameters;

namespace GeoGen.Core.Constructions
{
    /// <summary>
    /// Represents a construction that we use to add <see cref="ConstructedConfigurationObject"/> to a <see cref="Configuration"/>.
    /// It's given by the signature, which is a list of <see cref="ConstructionParameter"/>, and the output type 
    /// of <see cref="ConfigurationObjectType"/>.
    /// </summary>
    public abstract class Construction
    {
        #region Public properties

        /// <summary>
        /// Gets or sets the ID of this construction. 
        /// </summary>
        public int Id { get; set; }

        #endregion

        #region Public abstract properties

        /// <summary>
        /// Gets the output type of this construction (such as Point, Line...).
        /// </summary>
        public abstract ConfigurationObjectType OutputType { get; }

        /// <summary>
        /// Gets the construction signature, i.e. the unmodifiable list of construction parameters.
        /// </summary>
        public abstract IReadOnlyList<ConstructionParameter> ConstructionParameters { get; }

        #endregion
    }
}