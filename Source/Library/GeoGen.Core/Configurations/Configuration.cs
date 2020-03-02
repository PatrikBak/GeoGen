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
        /// Determines if the configuration is symmetric, i.e. its loose objects can be renamed to
        /// obtain the same configuration.
        /// </summary>
        /// <returns>true, if the configuration is symmetric; false otherwise.</returns>
        public bool IsSymmetric() => GetSymmetryMappings().Any();

        /// <summary>
        /// Find all possible mappings that would keep this configuration symmetric if all objects
        /// were remapping according to them. If the configuration is not symmetric, then there will
        /// not be any result.
        /// </summary>
        /// <returns>The numerable of all possible mappings keeping the symmetry.</returns>
        public IEnumerable<IReadOnlyDictionary<ConfigurationObject, ConfigurationObject>> GetSymmetryMappings()
            // Take all possible mappings of loose objects
            => LooseObjectsHolder.GetSymmetryMappings()
                // Exclude the identity
                .Where(mappedLooseObjects => mappedLooseObjects.Any(pair => pair.Key != pair.Value))
                // Remap the constructed objects as well
                .Select(mappedLooseObjects => (mappedLooseObjects, mappedConstructedObjects: ConstructedObjects
                    // Prepare a mapping dictionary for them
                    .ToDictionary(constructedObject => (ConfigurationObject)constructedObject,
                        // Each object is remapped with respect to the loose object mapping
                        constructedObject => constructedObject.Remap(mappedLooseObjects))))
                // Take only those mappings where the set of remapped constructed objects is the same as current ones
                .Where(pair => pair.mappedConstructedObjects.Values.OrderlessEquals(ConstructedObjectsSet))
                // These are correct symmetry mappings. Now we just complete them with the loose objects for comfort
                .Select(pair =>
                {
                    // Deconstruct
                    var (mappedLooseObjects, mappedConstructedObjects) = pair;

                    // Add the loose objects to the mapping
                    mappedLooseObjects.ForEach(pair => mappedConstructedObjects.Add(pair.Key, pair.Value));

                    // Return the final mapping
                    return mappedConstructedObjects;
                });

        /// <summary>
        /// Determines the objects that would make this configuration symmetric if they were added to it.
        /// </summary>
        /// <returns>An enumerable of objects with which the configuration would be symmetric.</returns>
        public IEnumerable<IReadOnlyList<ConstructedConfigurationObject>> GetObjectsThatWouldMakeThisConfigurationSymmetric()
            // Take all mappings
            => LooseObjectsHolder.GetSymmetryMappings()
                // Excluding the identity mapping
                .Where(mapping => mapping.Any(pair => pair.Key != pair.Value))
                // For a given mapping take the constructed objects
                .Select(mapping => ConstructedObjects
                    // Reconstruct them
                    .Select(construtedObject => (ConstructedConfigurationObject)construtedObject.Remap(mapping))
                    // Exclude the ones that we already have. This way we get the objects 
                    // that are missing in order for this mapping to prove symmetry. 
                    .Except(ConstructedObjects)
                    // Enumerate
                    .ToArray());

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

        /// <inheritdoc/>
        public override int GetHashCode() => (LooseObjectsHolder, ConstructedObjectsSet).GetHashCode();

        /// <inheritdoc/>
        public override bool Equals(object otherObject)
            // Either the references are equals
            => this == otherObject
                // Or the object is not null
                || otherObject != null
                // And is a configuration
                && otherObject is Configuration configuration
                // And the sets of constructed objects are equal
                && configuration.ConstructedObjectsSet.Equals(ConstructedObjectsSet)
                // And the loose objects are equal
                && LooseObjectsHolder.Equals(configuration.LooseObjectsHolder);

        #endregion

        #region Debug-only to string

#if DEBUG

        /// <inheritdoc/>
        public override string ToString() => $"{LooseObjectsHolder}{(ConstructedObjects.Any() ? $", {ConstructedObjects.ToJoinedString()}" : "")}";

#endif

        #endregion
    }
}