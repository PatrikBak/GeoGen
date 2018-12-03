namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents a query that can be performed on a <see cref="IContextualContainer"/>.
    /// </summary>
    internal class ContexualContainerQuery
    {
        /// <summary>
        /// Represents a type of geometrical objects that we're looking for.
        /// </summary>
        public enum ObjectsType
        {
            /// <summary>
            /// Objects that were added to the container while adding the last
            /// group of constructed objects.
            /// </summary>
            New,

            /// <summary>
            /// Objects that were added to the container while adding objects distinct 
            /// from the ones contained in the last group of constructed objects.
            /// </summary>
            Old,

            /// <summary>
            /// All objects from the container (new and old ones).
            /// </summary>
            All
        }

        /// <summary>
        /// Gets or sets the type of geometrical objects that we're seeking.
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
    }
}