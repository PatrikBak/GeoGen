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

        #region HashCode and Equals

        /// <inheritdoc/>
        public override int GetHashCode() => Name.GetHashCode();

        /// <inheritdoc/>
        public override bool Equals(object otherObject)
            // Either the references are equals
            => this == otherObject
                // Or the object is not null
                || otherObject != null
                // And is a construction
                && otherObject is Construction construction
                // And the names match
                && construction.Name.Equals(Name);

        #endregion

        #region To String

        /// <inheritdoc/>
        public override string ToString() => $"{Name}({Signature})";

        #endregion
    }
}