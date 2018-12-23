using GeoGen.Core;
using GeoGen.Utilities;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// A base implementation of <see cref="IPredefinedConstructor"/> that infers the
    /// type of the construction from a name that should be in the form "{type}Constructor".
    /// </summary>
    public abstract class PredefinedConstructorBase : ObjectsConstructorBase, IPredefinedConstructor
    {
        #region IPredefinedConstructor properties

        /// <summary>
        /// Gets the type of the predefined construction that this constructor performs.
        /// </summary>
        public PredefinedConstructionType Type { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        protected PredefinedConstructorBase()
        {
            // Set the compulsory construction type
            Type = FindTypeFromClassName();
        }

        #endregion

        #region Finding type from class name

        /// <summary>
        /// Infers the type of the predefined constructor from the class name. 
        /// The class name should be in the form {type}Constructor.
        /// </summary>
        /// <returns>The type.</returns>
        private PredefinedConstructionType FindTypeFromClassName() => EnumUtilities.ParseEnumValueFromClassName<PredefinedConstructionType>(GetType(), "Constructor");

        #endregion
    }
}