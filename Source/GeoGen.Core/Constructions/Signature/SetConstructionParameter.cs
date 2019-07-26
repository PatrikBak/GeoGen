using System;
using System.Linq;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a set of <see cref="ConstructionParameter"/>s. It's defined by a number of parameters, 
    /// and the type of these parameters. The type of parameters can therefore be another set. For example, 
    /// a properly defined intersection construction has the signature { {P, P}, {P, P} }, where P represents
    /// a point. This parameter corresponds to an <see cref="SetConstructionArgument"/>.
    /// </summary>
    public class SetConstructionParameter : ConstructionParameter
    {
        #region Public properties

        /// <summary>
        /// Gets the type of construction parameter represented in this set.
        /// </summary>
        public ConstructionParameter TypeOfParameters { get; }

        /// <summary>
        /// Gets the number of needed parameters of the given type.
        /// </summary>
        public int NumberOfParameters { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SetConstructionParameter"/> class.
        /// </summary>
        /// <param name="typeOfParameters">The type of construction parameter represented in this set.</param>
        /// <param name="numberOfParameters">The number of needed parameters of the given type.</param>
        public SetConstructionParameter(ConstructionParameter typeOfParameters, int numberOfParameters)
        {
            TypeOfParameters = typeOfParameters ?? throw new ArgumentNullException(nameof(numberOfParameters));
            NumberOfParameters = numberOfParameters;
        }

        #endregion

        #region To String

        /// <summary>
        /// Converts the set construction parameter to a string. 
        /// NOTE: This method is used only for debugging purposes.
        /// </summary>
        /// <returns>A human-readable string representation of the configuration.</returns>
        public override string ToString() => $"{{{string.Join(", ", Enumerable.Repeat(TypeOfParameters.ToString(), NumberOfParameters))}}}";

        #endregion
    }
}