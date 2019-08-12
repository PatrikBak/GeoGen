using GeoGen.Core;
using System.Collections.Generic;

namespace GeoGen.Constructor
{
    /// <summary>
    /// Represents a query that can be asked of a <see cref="ContextualPicture"/>s.
    /// </summary>
    public class ContextualPictureQuery
    {
        #region ObjectType enum

        /// <summary>
        /// Represents a type of geometric objects that we're looking for.
        /// </summary>
        public enum ObjectsType
        {
            /// <summary>
            /// Objects that were added to the picture while adding the last configuration object.
            /// </summary>
            New,

            /// <summary>
            /// Objects that were added to the picture and aren't new.
            /// </summary>
            Old,

            /// <summary>
            /// All objects from the picture (new and old ones).
            /// </summary>
            All
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the type of geometric objects that we're asking for.
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

        /// <summary>
        /// Gets or sets the list of points that should line on every line or circle that we're queering.
        /// </summary>
        public IEnumerable<ConfigurationObject> ContainingPoints { get; set; }

        #endregion
    }
}