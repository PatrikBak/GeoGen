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

        /// <summary>
        /// Indicates if the construction construct an object whose construction is not defined deterministically.
        /// </summary>
        public bool IsRandom { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Construction"/> class.
        /// </summary>
        /// <param name="name">The name of the construction.</param>
        /// <param name="parameters">The parameters representing the signature of the construction.</param>
        /// <param name="outputType">The output type of the construction.</param>
        /// <param name="isRandom">Indicates if the construction construct an object whose construction is not defined deterministically.</param>
        protected Construction(string name, IReadOnlyList<ConstructionParameter> parameters, ConfigurationObjectType outputType, bool isRandom)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Signature = new Signature(parameters);
            OutputType = outputType;
            IsRandom = isRandom;
        }

        #endregion

        #region HashCode and Equals

        /// <summary>
        /// Gets the hash code of this object.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode() => Name.GetHashCode();

        /// <summary>
        /// Finds out if a passed object is equal to this one.
        /// </summary>
        /// <param name="otherObject">The passed object.</param>
        /// <returns>true, if they are equal; false otherwise.</returns>
        public override bool Equals(object otherObject)
        {
            // Either the references are equals
            return this == otherObject
                // Or the object is not null
                || otherObject != null
                // And is a construction
                && otherObject is Construction construction
                // And the names match
                && construction.Name.Equals(Name);
        }

        #endregion

        #region To String

        /// <summary>
        /// Converts the construction to a string. 
        /// NOTE: This method i used only for debugging purposes.
        /// </summary>
        /// <returns>A human-readable string representation of the construction.</returns>
        public override string ToString() => $"{Name}({Signature})";

        #endregion
    }
}