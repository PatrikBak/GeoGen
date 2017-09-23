namespace GeoGen.Core.Constructions.PredefinedConstructions
{
    /// <summary>
    /// Represents a construction that is supposed be predefined, such as Midpoint, Line intersection...
    /// </summary>
    public abstract class PredefinedConstruction : Construction
    {
        #region Public abstract properties

        /// <summary>
        /// Gets the type of this predefined construction.
        /// </summary>
        public abstract PredefinedConstructionType ConstructionType { get; }

        #endregion
    }
}