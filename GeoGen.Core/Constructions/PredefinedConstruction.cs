namespace GeoGen.Core.Constructions
{
    /// <summary>
    /// Represents a construction that is supposed be predefined, such as Midpoint, Line intersection...
    /// </summary>
    public abstract class PredefinedConstruction : Construction
    {
        #region Public abstract properties

        /// <summary>
        /// Gets the type of tis predefined construction.
        /// </summary>
        public abstract PredefinedConstructionType ConstructionType { get; }

        #endregion
    }
}