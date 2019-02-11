using GeoGen.Core;
using GeoGen.Utilities;

namespace GeoGen.GeometryRegistrar
{
    /// <summary>
    /// A base implementation of <see cref="IPredefinedConstructor"/> that infers the
    /// type of construction from the class name that should be in the form "{type}Constructor".
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
        /// Initializes a new instance of the <see cref="PredefinedConstructorBase"/> class.
        /// </summary>
        protected PredefinedConstructorBase()
        {
            // Find the type
            Type = FindTypeFromClassName();
        }

        #endregion

        #region Finding type from the class name

        /// <summary>
        /// Infers the type of the predefined constructor from the class name. 
        /// The class name should be in the form {type}Constructor.
        /// </summary>
        /// <returns>The inferred type.</returns>
        private PredefinedConstructionType FindTypeFromClassName()
        {
            // Call the utility helper that does the job
            return EnumUtilities.ParseEnumValueFromClassName<PredefinedConstructionType>(GetType(), classNamePrefix: "Constructor");
        }

        #endregion
    }
}