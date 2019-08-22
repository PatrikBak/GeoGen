using System.Collections.Generic;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a construction whose constructor is implemented in the code. For the list
    /// of available predefined constructions see <see cref="PredefinedConstructionType"/>.
    /// </summary>
    public class PredefinedConstruction : Construction
    {
        #region Public properties

        /// <summary>
        /// Gets the type of this predefined construction.
        /// </summary>
        public PredefinedConstructionType Type { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PredefinedConstruction"/> class.
        /// </summary>
        /// <param name="type">The type of the predefined construction.</param>
        /// <param name="parameters">The parameters representing the signature of the construction.</param>
        /// <param name="outputType">The output type of the construction.</param>
        /// <param name="isRandom">Indicates if the construction construct an object whose construction is not defined deterministically.</param>
        public PredefinedConstruction(PredefinedConstructionType type, IReadOnlyList<ConstructionParameter> parameters, ConfigurationObjectType outputType, bool isRandom)
            : base(type.ToString(), parameters, outputType, isRandom)
        {
            Type = type;
        }

        #endregion
    }
}