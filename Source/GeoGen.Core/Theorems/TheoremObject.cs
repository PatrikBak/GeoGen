using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a geometric object of certain <see cref="ConfigurationObjectType"/> 
    /// that is be used to express a <see cref="Theorem"/>.
    /// </summary>
    public abstract class TheoremObject
    {
        #region Public static properties

        /// <summary>
        /// Gets the single instance of the equality comparer of two theorem objects that uses the 
        /// <see cref="AreTheoremObjectsEquivalent(TheoremObject, TheoremObject)"/> method and 
        /// a constant hash code function (i.e. using it together with a hash map / hash set would 
        /// make all the operations O(n)).
        /// </summary>
        public static readonly IEqualityComparer<TheoremObject> EquivalencyComparer = new SimpleEqualityComparer<TheoremObject>((t1, t2) => AreTheoremObjectsEquivalent(t1, t2), t => 0);

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the type of the object.
        /// </summary>
        public ConfigurationObjectType Type { get; }

        /// <summary>
        /// Gets the object that directly represents this theorem object. This value
        /// can't be null for a point, but can be null for a line or a circle. 
        /// </summary>
        public ConfigurationObject ConfigurationObject { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremObject"/> class.
        /// </summary>
        /// <param name="type">The type of the theorem object.</param>
        /// <param name="configurationObject">The configuration object.</param>
        protected TheoremObject(ConfigurationObjectType type, ConfigurationObject configurationObject = null)
        {
            Type = type;
            ConfigurationObject = configurationObject;

            // Make sure the types are consistent, if the configuration object is specified
            if (ConfigurationObject != null && ConfigurationObject.ObjectType != type)
                throw new GeoGenException("The type of the inner configuration object doesn't match the type of the theorem object");
        }

        #endregion

        #region Public static methods

        /// <summary>
        /// Determines if two theorem objects are equivalent, i.e. if they have the same type and geometrically
        /// represent the same statements in the same geometric situation. 
        /// </summary>
        /// <param name="theoremObject1">The first theorem object.</param>
        /// <param name="theoremObject2">The second theorem object.</param>
        /// <returns>true, if they are equivalent; false otherwise.</returns>
        public static bool AreTheoremObjectsEquivalent(TheoremObject theoremObject1, TheoremObject theoremObject2)
        {
            // Check their types
            if (theoremObject1.Type != theoremObject2.Type)
                return false;

            // Handle the point case in which we need to have equal configuration objects
            if (theoremObject1 is TheoremPointObject)
                return theoremObject1.ConfigurationObject == theoremObject2.ConfigurationObject;

            // Otherwise we have the line/circle case
            var lineOrCircle1 = (TheoremObjectWithPoints) theoremObject1;
            var lineOrCircle2 = (TheoremObjectWithPoints) theoremObject2;

            // If their configuration objects are defined and matches, then they are equivalent
            if (lineOrCircle1.ConfigurationObject != null && lineOrCircle1.ConfigurationObject == lineOrCircle2.ConfigurationObject)
                return true;

            // If the number of their common points is enough to define them, then they are equivalent
            if (lineOrCircle1.Points.Intersect(lineOrCircle2.Points).Count() >= lineOrCircle1.NumberOfNeededPoints)
                return true;

            // Otherwise we don't have enough information to say they are equivalent for sure
            return false;
        }

        #endregion
    }
}