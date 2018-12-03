using System.Collections.Generic;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a construction that is used to define <see cref="ConstructedConfigurationObject"/>s.
    /// It's given by the input signature, which is a list of <see cref="ConstructionParameter"/>s, 
    /// and the output signature, which is a list of <see cref="ConfigurationObjectType"/>s. 
    /// </summary>
    public abstract class Construction
    {
        #region Public properties

        /// <summary>
        /// Gets or sets the id of this construction. The id should be
        /// unique solely during the generation process. It will be reseted every time
        /// the process starts over.
        /// </summary>
        public int? Id { get; set; }

        #endregion

        #region Public abstract properties

        /// <summary>
        /// Gets the construction input signature, i.e. the list of construction parameters.
        /// </summary>
        public abstract IReadOnlyList<ConstructionParameter> ConstructionParameters { get; }

        /// <summary>
        /// Gets the construction output signature, i.e. the list of configuration object types.
        /// </summary>
        public abstract IReadOnlyList<ConfigurationObjectType> OutputTypes { get; }

        #endregion
    }
}