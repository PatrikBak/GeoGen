using GeoGen.Utilities;
using System.Linq;

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

        #region Public methods

        /// <summary>
        /// Finds out if this object is equivalent to the other one.
        /// </summary>
        /// <param name="otherObject">The other object.</param>
        /// <returns>true, if they are equivalent; false otherwise.</returns>
        public bool IsEquivalentTo(ConstructedConfigurationObject otherObject)
        {
            // They have to have equally named constructions
            if (Construction.Name != otherObject.Construction.Name)
                return false;

            // Otherwise return if the arguments are sequentially equivalent to each other
            return PassedArguments.Zip(otherObject.PassedArguments).All(pair => pair.Item1.IsEquivalentTo(pair.Item2));
        }

        #endregion
    }
}

