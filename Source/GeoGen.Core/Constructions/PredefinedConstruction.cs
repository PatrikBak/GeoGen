using System;
using System.Collections.Generic;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a construction that is supposed be predefined, i.e. the constructor
    /// for this constructions should be implemented directly in the code. For the list
    /// of predefined constructions, see <see cref="PredefinedConstructionType"/>.
    /// </summary>
    public class PredefinedConstruction : Construction
    {
        #region Public properties

        /// <summary>
        /// Gets the type of this predefined construction.
        /// </summary>
        public PredefinedConstructionType Type { get; }

        #endregion

        #region Construction properties

        /// <summary>
        /// Gets the construction input signature, i.e. the list of construction parameters.
        /// </summary>
        public override IReadOnlyList<ConstructionParameter> ConstructionParameters { get; }

        /// <summary>
        /// Gets the construction output signature, i.e. the list of configuration object types.
        /// </summary>
        public override ConfigurationObjectType OutputType { get; }

        /// <summary>
        /// Gets the name of the construction.
        /// </summary>
        public override string Name => Type.ToString();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="type">The type of the construction.</param>
        /// <param name="parameters">The parameters representing the signature of the construction.</param>
        /// <param name="outputTypes">The output types of the construction.</param>
        public PredefinedConstruction(PredefinedConstructionType type, IReadOnlyList<ConstructionParameter> parameters, ConfigurationObjectType outputType)
        {
            Type = type;
            ConstructionParameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            OutputType = outputType;
        }

        #endregion
    }
}