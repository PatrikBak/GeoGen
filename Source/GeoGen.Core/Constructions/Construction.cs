using System;
using System.Collections.Generic;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a construction that is used to define <see cref="ConstructedConfigurationObject"/>s.
    /// It's given by its <see cref="Core.Signature"/>, which is essentially a list of
    /// <see cref="ConstructionParameter"/>s, and the <see cref="ConfigurationObjectType"/> of the 
    /// output object. Every construction has a name that should be the unique identifier for it.
    /// </summary>
    public abstract class Construction 
    {
        #region Public properties

        /// <summary>
        /// Gets the construction input signature.
        /// </summary>
        public Signature Signature { get; }

        /// <summary>
        /// Gets the output type of the construction.
        /// </summary>
        public ConfigurationObjectType OutputType { get; }

        /// <summary>
        /// Gets the name of the construction. The name should be its unique identifier.
        /// </summary>
        public string Name { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Construction"/> class.
        /// </summary>
        /// <param name="name">The name of the construction.</param>
        /// <param name="parameters">The parameters representing the signature of the construction.</param>
        /// <param name="outputType">The output type of the construction.</param>
        protected Construction(string name, IReadOnlyList<ConstructionParameter> parameters, ConfigurationObjectType outputType)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Signature = new Signature(parameters);
            OutputType = outputType;
        }

        #endregion

        #region To String

        /// <summary>
        /// Converts the construction to a string. 
        /// NOTE: This method i used only for debugging purposes.
        /// </summary>
        /// <returns>A human-readable string representation of the construction.</returns>
        public override string ToString() => $"{Name} ({Signature})";

        #endregion
    }
}