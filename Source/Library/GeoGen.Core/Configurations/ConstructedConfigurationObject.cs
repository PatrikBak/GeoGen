namespace GeoGen.Core
{
    /// <summary>
    /// Represent a <see cref="ConfigurationObject"/> that is composed of a <see cref="Core.Construction"/>, and 
    /// <see cref="Arguments"/> that hold actual configuration objects from which this object should be constructed.
    /// </summary>
    public class ConstructedConfigurationObject : ConfigurationObject
    {
        #region Public properties

        /// <summary>
        /// Gets the construction that should be used to draw this object.
        /// </summary>
        public Construction Construction { get; }

        /// <summary>
        /// Gets the arguments that should be passed to the construction function.
        /// </summary>
        public Arguments PassedArguments { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructedConfigurationObject"/> class.
        /// </summary>
        /// <param name="construction">The construction that should be used to draw this object.</param>
        /// <param name="arguments">The arguments that should be passed to the construction function.</param>
        public ConstructedConfigurationObject(Construction construction, Arguments arguments)
            : base(construction.OutputType)
        {
            Construction = construction;
            PassedArguments = arguments;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructedConfigurationObject"/> class
        /// using a predefined construction of a given type and input objects.
        /// </summary>
        /// <param name="type">The type of the predefined construction to be performed.</param>
        /// <param name="input">The input objects in the flattened order (see <see cref="Arguments.FlattenedList")/></param>
        public ConstructedConfigurationObject(PredefinedConstructionType type, params ConfigurationObject[] input)
            : this(Constructions.GetPredefinedconstruction(type), input)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructedConfigurationObject"/> class
        /// using a construction and input objects.
        /// </summary>
        /// <param name="construction">The construction that should be used to draw this object.</param>
        /// <param name="input">The input objects in the flattened order (see <see cref="Arguments.FlattenedList")/></param>
        public ConstructedConfigurationObject(Construction construction, params ConfigurationObject[] input)
            : this(construction, construction.Signature.Match(input))
        {
        }

        #endregion

        #region Public abstract methods implementation

        /// <inheritdoc/>
        public override ConstructedConfigurationObject Remap(IReadOnlyDictionary<LooseConfigurationObject, LooseConfigurationObject> mapping)
            // We reuse the construction and remap arguments
            => new ConstructedConfigurationObject(Construction, PassedArguments.Remap(mapping));

        #endregion

        #region Public methods

        /// <summary>
        /// Checks if given flattened reordered arguments would make the same object as this one.
        /// </summary>
        /// <param name="flattenedReorderedArguments">The flattened reordered arguments to be checked.</param>
        /// <returns>true, if the arguments would make the same object; false otherwise.</returns>
        public bool CanWeReorderArgumentsLikeThis(ConfigurationObject[] flattenedReorderedArguments)
        {
            // First find out if the types are correct
            var doTypesFit = flattenedReorderedArguments
                // For every passed object we take the type
                .Select(argument => argument.ObjectType)
                // These types must correspond to the signature
                .SequenceEqual(Construction.Signature.ObjectTypes);

            // If the types don't fit, then the answer is no
            if (!doTypesFit)
                return false;

            // Now we can do the check by creating an artificial object with the passed arguments
            return new ConstructedConfigurationObject(Construction, flattenedReorderedArguments)
                // And compare this object with this one
                .Equals(this);
        }

        #endregion

        #region HashCode and Equals

        /// <inheritdoc/>
        public override int GetHashCode() => (Construction, PassedArguments).GetHashCode();

        /// <inheritdoc/>
        public override bool Equals(object otherObject)
            // Either the references are equal
            => this == otherObject
                // Or the object is not null
                || otherObject != null
                // And is a constructed object
                && otherObject is ConstructedConfigurationObject constructedObject
                // And their constructions are equal
                && constructedObject.Construction.Equals(Construction)
                // And their arguments are equal
                && constructedObject.PassedArguments.Equals(PassedArguments);

        #endregion

        #region Debug-only to string

#if DEBUG

        /// <inheritdoc/>
        public override string ToString() => $"{Id}={Construction.Name}({PassedArguments})";

#endif

        #endregion
    }
}

