namespace GeoGen.Constructor
{
    /// <summary>
    /// Represents a query that can be asked of a <see cref="IContextualContainer"/>.
    /// </summary>
    public class ContexualContainerQuery
    {
        #region ObjectType enum

        /// <summary>
        /// Represents a type of geometrical objects that we're looking for.
        /// </summary>
        public enum ObjectsType
        {
            /// <summary>
            /// Objects that were added to the container while adding the last configuration object.
            /// </summary>
            New,

            /// <summary>
            /// Objects that were added to the container and aren't new.
            /// </summary>
            Old,

            /// <summary>
            /// All objects from the container (new and old ones).
            /// </summary>
            All
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the type of geometrical objects that we're asking for.
        /// </summary>
        public ObjectsType Type { get; set; }

        /// <summary>
        /// Gets or sets if we should include points.
        /// </summary>
        public bool IncludePoints { get; set; }

        /// <summary>
        /// Gets or sets if we should include lines.
        /// </summary>
        public bool IncludeLines { get; set; }

        /// <summary>
        /// Gets or sets if we should include circles.
        /// </summary>
        public bool IncludeCirces { get; set; }

        #endregion
    }
}