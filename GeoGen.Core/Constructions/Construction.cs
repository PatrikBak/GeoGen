using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Parameters;

namespace GeoGen.Core.Constructions
{
    /// <summary>
    /// An abstract representation of a construction that we use to add 
    /// <see cref="ConstructedConfigurationObject"/> to a <see cref="Configuration"/>. 
    /// It's given by the input signature, which is a list of <see cref="ConstructionParameter"/>, 
    /// and the output signature, which is a list of <see cref="ConfigurationObjectType"/>. 
    /// </summary>
    public abstract class Construction
    {
        #region Public properties

        /// <summary>
        /// Gets or sets the ID of this construction. 
        /// </summary>
        public virtual int? Id { get; set; }

        #endregion

        #region Public abstract properties

        /// <summary>
        /// Gets the construction input signature, i.e. the unmodifiable list of construction parameters.
        /// </summary>
        public abstract IReadOnlyList<ConstructionParameter> ConstructionParameters { get; }

        /// <summary>
        /// Gets the construction output signature, i.e. the unmodifiable list of configuration object types.
        /// </summary>
        public abstract IReadOnlyList<ConfigurationObjectType> OutputTypes { get; }

        #endregion
    }
}