using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a <see cref="TheoremObject"/> that might be defined by points, such as a line / circle.
    /// </summary>
    public abstract class TheoremObjectWithPoints : BaseTheoremObject
    {
        #region Public properties

        /// <summary>
        /// Gets the point configuration objects defining this object. 
        /// </summary>
        public IReadOnlyHashSet<ConfigurationObject> Points { get; }

        /// <summary>
        /// Indicates whether this object is defined explicitly via a <see cref="ConfigurationObject"/>
        /// of the particular type.
        /// </summary>
        public bool DefinedByExplicitObject => ConfigurationObject != null;

        /// <summary>
        /// Indicates whether this object is defined by the needed number of points.
        /// </summary>
        public bool DefinedByPoints => Points != null;

        #endregion

        #region Public abstract properties

        /// <summary>
        /// Gets the number of points that might define this type of object.
        /// </summary>
        public abstract int NumberOfDefiningPoints { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremObjectWithPoints"/> class
        /// defining by a <see cref="ConfigurationObject"/>.
        /// </summary>
        /// <param name="configurationObject">The configuration object representing this theorem object.</param>
        protected TheoremObjectWithPoints(ConfigurationObject configurationObject)
            : base(configurationObject)
        {
            // Make sure the defining object exists
            if (configurationObject == null)
                throw new GeoGenException("The defining configuration object cannot be null.");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremObjectWithPoints"/> class
        /// defined by <see cref="NumberOfDefiningPoints"/> points.
        /// </summary>
        /// <param name="points">The points defining this object.</param>
        protected TheoremObjectWithPoints(params ConfigurationObject[] points)
        {
            // Set the points
            Points = points?.ToReadOnlyHashSet() ?? throw new ArgumentNullException(nameof(points));

            // Make sure there count fits
            if (Points.Count != NumberOfDefiningPoints)
                throw new GeoGenException($"The {GetType()} must have {NumberOfDefiningPoints} points, but has {Points.Count}.");

            // Make sure they're points
            if (!Points.All(point => point.ObjectType == ConfigurationObjectType.Point))
                throw new GeoGenException("The passed object must be points");
        }

        #endregion

        #region Public abstract methods implementation

        /// <summary>
        /// Gets the configuration objects that internally define this theorem object.
        /// </summary>
        /// <returns>The enumerable of the internal configuration objects.</returns>
        public override IEnumerable<ConfigurationObject> GetInnerConfigurationObjects()
        {
            // If this is defined by an object, return it
            if (DefinedByExplicitObject)
                return new[] { ConfigurationObject };

            // Otherwise this object is defined by points
            return Points;
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
        protected (ConfigurationObject explicitObject, ConfigurationObject[] points) RemapObjectAndPoints(Dictionary<ConfigurationObject, ConfigurationObject> mapping)
        {
            // If it's defined by an explicit object
            if (DefinedByExplicitObject)
            {
                // Try to map it
                var mappedObject = Map(ConfigurationObject, mapping);

                // If it's okay, return it with null points
                if (mappedObject != null)
                    return (mappedObject, points: null);

                // Otherwise return the default value
                return default;
            }

            // Otherwise we are defined by points
            // Try to remap them
            var mappedPoints = Points.SelectIfNotDefault(point => Map(point, mapping))?.Distinct().ToArray();

            // If the points couldn't be mapped, return default
            if (mappedPoints == null)
                return default;

            // If they're count doesn't match, return default too
            if (mappedPoints.Length != NumberOfDefiningPoints)
                return default;

            // Otherwise return the tuple with no explicit object and the mapped points
            return (explicitObject: null, mappedPoints);
        }

        #endregion

        #region HashCode and Equals

        /// <summary>
        /// Gets the hash code of this object.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            // If the object has an explicit object, return its hash 
            if (DefinedByExplicitObject)
                return ConfigurationObject.GetHashCode();

            // Otherwise return the hash of the points
            return Points.GetHashCode();
        }

        /// <summary>
        /// Finds out if a passed object is equal to this one.
        /// </summary>
        /// <param name="otherObject">The passed object.</param>
        /// <returns>true, if they are equal; false otherwise.</returns>
        public override bool Equals(object otherObject)
        {
            // Either the references are equals
            return this == otherObject
                // Or the object is not null
                || otherObject != null
                // And it is an object with points
                && otherObject is TheoremObjectWithPoints objectWithPoints
                // And either they're both defined by objects
                && ((DefinedByExplicitObject && objectWithPoints.DefinedByExplicitObject
                    // And these objects are equal
                    && ConfigurationObject.Equals(objectWithPoints.ConfigurationObject))
                // Or they are both defined by points 
                || (DefinedByPoints && objectWithPoints.DefinedByPoints
                    // And their points are equal
                    && Points.Equals(objectWithPoints.Points)));
        }

        #endregion
    }
}