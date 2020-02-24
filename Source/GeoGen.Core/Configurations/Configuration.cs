using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a configuration of geometric objects. It consists of a <see cref="Core.LooseObjectHolder"/>
    /// and a list of <see cref="ConstructedConfigurationObject"/>. The loose objects are the first objects to be 
    /// drawn (for example: in a triangle the loose objects are its 3 vertices. The constructed objects should to 
    /// be ordered so that it's possible to construct them in this order. The configuration should contain mutually
    /// distinct objects.
    /// </summary>
    public class Configuration
    {
        #region Public properties

        /// <summary>
        /// Gets the holder of the loose objects of this configurations.
        /// </summary>
        public LooseObjectHolder LooseObjectsHolder { get; }

        /// <summary>
        /// Gets the loose object of the configuration.
        /// </summary>
        public IReadOnlyList<LooseConfigurationObject> LooseObjects => LooseObjectsHolder.LooseObjects;

        /// <summary>
        /// Gets the list of constructed configuration objects ordered in a way that we can construct them in this order.
        /// </summary>
        public IReadOnlyList<ConstructedConfigurationObject> ConstructedObjects { get; }

        /// <summary>
        /// Gets the constructed objects of the configuration as a set.
        /// </summary>
        public IReadOnlyHashSet<ConstructedConfigurationObject> ConstructedObjectsSet { get; }

        /// <summary>
        /// Gets the last constructed object of the configuration.
        /// </summary>
        public ConstructedConfigurationObject LastConstructedObject => ConstructedObjects.LastOrDefault() ?? throw new GeoGenException("No constructed objects");

        /// <summary>
        /// Gets the configuration objects map containing all the objects of the configuration.
        /// </summary>
        public ConfigurationObjectMap ObjectMap { get; }

        /// <summary>
        /// Gets all configuration objects of the configuration.
        /// </summary>
        public IReadOnlyList<ConfigurationObject> AllObjects => ObjectMap.AllObjects;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Configuration"/> class.
        /// </summary>
        /// <param name="looseObjectsHolder">The holder of the loose objects of this configurations.</param>
        /// <param name="constructedObjects">The list of constructed configuration objects ordered in a way that we can construct them in this order.</param>
        public Configuration(LooseObjectHolder looseObjectsHolder, IReadOnlyList<ConstructedConfigurationObject> constructedObjects)
        {
            LooseObjectsHolder = looseObjectsHolder;
            ConstructedObjects = constructedObjects ?? throw new ArgumentNullException(nameof(constructedObjects));
            ObjectMap = new ConfigurationObjectMap(LooseObjects.Cast<ConfigurationObject>().Concat(constructedObjects));
            ConstructedObjectsSet = ConstructedObjects.ToReadOnlyHashSet();

            // Make sure there are no duplicated
            if (ConstructedObjectsSet.Count != constructedObjects.Count)
                throw new GeoGenException("Configuration contains equal constructed objects.");
        }

        /// <summary>
        /// Initializes  a new instance of the <see cref="Configuration"/> class that consists of given objects.
        /// The loose objects will be automatically detected and will have a specified layout.
        /// </summary>
        /// <param name="layout">The layout of the loose objects./>.</param>
        /// <param name="objects">The objects of the configuration.</param>
        private Configuration(LooseObjectLayout layout, params ConfigurationObject[] objects)
            : this(new LooseObjectHolder(objects.OfType<LooseConfigurationObject>().ToList(), layout), objects.OfType<ConstructedConfigurationObject>().ToList())
        {
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Determines if the configuration is symmetric, i.e. its loose objects can be 
        /// reorder to obtain the same configuration.
        /// </summary>
        /// <returns>true, if the configuration is symmetric; false otherwise.</returns>
        public bool IsSymmetric() => GetMinimalNumberOfObjectsToMakeThisSymmetric() == 0;

        /// <summary>
        /// Determines the minimal number of objects that have to be added to this configuration
        /// to make it symmetric. If the configuration is already symmetric, than the answer is 0.
        /// </summary>
        /// <returns>The minimal number of objects needed to be added to make this configuration symmetric.</returns>
        public int GetMinimalNumberOfObjectsToMakeThisSymmetric()
        {
            // Prepare the enumerable of needed counts of objects
            // to be added to make particular mappings the ones proving symmetry
            // Take all mappings
            var neededCounts = LooseObjectsHolder.GetSymmetryMappings()
                // Excluding the identity mapping
                .Where(mapping => mapping.Any(pair => pair.Key != pair.Value))
                // For a given mapping take the constructed objects
                .Select(mapping => ConstructedObjects
                    // Reconstruct them
                    .Select(construtedObject => construtedObject.Remap(mapping))
                    // Exclude the ones that we already have. This way we get the objects 
                    // that are missing in order for this mapping to prove symmetry. 
                    .Except(ConstructedObjects)
                    // We're interesting only in their count
                    .Count());

            // Prepare the resulting minimum
            var minimalCountOfObjects = int.MaxValue;

            // Go through the enumerable of needed counts...
            foreach (var currentNeededCount in neededCounts)
            {
                // If there is a 0, then we know we won't get a smaller value
                // This small optimization seems to work pretty well
                if (currentNeededCount == 0)
                    return 0;

                // If we found a smaller value, then we update the current minimum
                if (currentNeededCount < minimalCountOfObjects)
                    minimalCountOfObjects = currentNeededCount;
            }

            // Return the resulting minimum
            return minimalCountOfObjects;
        }

        #endregion

        #region Public static methods

        /// <summary>
        /// Creates a configuration that simulates the construction of given constructed objects.
        /// The loose objects will be automatically detected and will have the specified layout.
        /// </summary>
        /// <param name="layout">The layout for the automatically detected loose objects.</param>
        /// <param name="objects">The objects whose construction defines the configuration.</param>
        /// <returns>The configuration derived from the objects.</returns>
        public static Configuration DeriveFromObjects(LooseObjectLayout layout, params ConfigurationObject[] objects)
            // We use our helper method to find all the objects that define the passed ones
            => new Configuration(layout, objects.GetDefiningObjects().ToArray());

        #endregion

        #region HashCode and Equals

        /// <summary>
        /// Gets the hash code of this object.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode() => (LooseObjectsHolder, ConstructedObjectsSet).GetHashCode();

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
                // And is a configuration
                && otherObject is Configuration configuration
                // And the sets of constructed objects are equal
                && configuration.ConstructedObjectsSet.Equals(ConstructedObjectsSet)
                // And the loose objects are equal
                && LooseObjectsHolder.Equals(configuration.LooseObjectsHolder);
        }

        #endregion

        #region Debug-only to string

#if DEBUG

        /// <summary>
        /// Converts the configuration to a string. 
        /// </summary>
        /// <returns>A human-readable string representation of the configuration.</returns>
        public override string ToString() => $"{LooseObjectsHolder}{(ConstructedObjects.Any() ? $", {ConstructedObjects.ToJoinedString()}" : "")}";

#endif

        #endregion
    }
}