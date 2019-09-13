using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a <see cref="TheoremObject"/> that might be defined by points, like a line / circle.
    /// </summary>
    public abstract class TheoremObjectWithPoints : BaseTheoremObject
    {
        #region Public properties

        /// <summary>
        /// Gets the point configuration objects that are contained in this object.
        /// </summary>
        public IReadOnlyHashSet<ConfigurationObject> Points { get; }

        #endregion

        #region Public abstract properties

        /// <summary>
        /// Gets the minimal number of distinct points that are needed to define this type of object.
        /// </summary>
        public abstract int NumberOfNeededPoints { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremObjectWithPoints"/> class.
        /// </summary>
        /// <param name="configurationObject">The optional configuration object representing this theorem object.</param>
        /// <param name="points">The optional points that define / lie on this object.</param>
        protected TheoremObjectWithPoints(ConfigurationObject configurationObject, IEnumerable<ConfigurationObject> points = null)
            : base(configurationObject)
        {
            Points = (points ?? Enumerable.Empty<ConfigurationObject>()).ToReadOnlyHashSet();

            // Make sure we have enough points in the case where the object is not defined
            if (configurationObject == null && NumberOfNeededPoints > Points.Count)
                throw new GeoGenException("There are not enough distinct points to define this object.");
        }

        #endregion

        #region Protected helper methods

        /// <summary>
        /// Remaps the <see cref="base.ConfigurationObject"/> and <see cref="Points"/> with
        /// respect to the mapping. If the it would result to an undefinable object, this 
        /// method returns  the default value, i.e. (null, null).
        /// </summary>
        /// <param name="mapping">The dictionary representing the mapping.</param>
        /// <returns>The remapped objects, if the mapping can be done; otherwise the default value, i.e. (null, null).</returns>
        protected (ConfigurationObject, List<ConfigurationObject>) RemapObjectAndPoints(Dictionary<ConfigurationObject, ConfigurationObject> mapping)
        {
            // Try to remap the object, if it's not null
            var configurationObject = ConfigurationObject == null ? null : Map(ConfigurationObject, mapping);

            // Try to remap points
            var points = Points.SelectIfNotDefault(point => Map(point, mapping))?.Distinct().ToList();

            // If the points couldn't be mapped, return default
            if (points == null)
                return default;

            // Make sure we can define the object, i.e. either there is an object or enough points
            return configurationObject != null || points.Count >= NumberOfNeededPoints
                // If yes, return the remapped tuple
                ? (configurationObject, points)
                // If not, return null
                : default;
        }

        #endregion
    }
}