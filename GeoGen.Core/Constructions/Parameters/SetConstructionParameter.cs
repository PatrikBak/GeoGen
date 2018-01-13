using System;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a set of <see cref="ConstructionParameter"/>. It's given by the number of parameters, 
    /// and the type of parameters that it contains. The type of parameters of the set can possible be 
    /// another set. For example, a properly defined intersection construction has the signature 
    /// { {P, P}, {P, P} }, where P represents a point. It's not supposed to be used as a set of with one 
    /// element, since it's either a  <see cref="ObjectConstructionParameter"/>, or a set within a set 
    /// (which doesn't make sense in  our context). 
    /// </summary>
    public class SetConstructionParameter : ConstructionParameter
    {
        #region Public properties

        /// <summary>
        /// Gets the type of construction parameters that are contained within this set parameter type. 
        /// </summary>
        public ConstructionParameter TypeOfParameters { get; }

        /// <summary>
        /// Gets the number of needed parameters of the given type.
        /// </summary>
        public int NumberOfParameters { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="typeOfParameters">The type of construction parameters.</param>
        /// <param name="numberOfParameters">The number of needed parameters of the given type.</param>
        public SetConstructionParameter(ConstructionParameter typeOfParameters, int numberOfParameters)
        {
            TypeOfParameters = typeOfParameters ?? throw new ArgumentNullException(nameof(numberOfParameters));
            NumberOfParameters = numberOfParameters;
        }

        #endregion
    }
}