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
        /// Determines if the configuration is symmetric, i.e. <see cref="LooseObjectHolder.GetSymmetricMappings"/>
        /// produces a mapping that keeps this configuration the same.
        /// </summary>
        /// <returns>true, if the configuration is symmetric; false otherwise.</returns>
        public bool IsSymmetric() => GetSymmetryMappings().Any();

        /// <summary>
        /// Determines if the configuration is fully symmetric, i.e. <see cref="LooseObjectHolder.GetSymmetricMappings"/>
        /// produces only mappings that keeps this configuration the same.
        /// </summary>
        /// <returns>true, if the configuration is fully symmetric; false otherwise.</returns>
        public bool IsFullySymmetric() => GetSymmetryMappings().Count() == LooseObjectsHolder.GetSymmetricMappings().Count();

        /// <summary>
        /// Find all possible mappings that would keep this configuration symmetric if all objects
        /// were remapping according to them. If the configuration is not symmetric, then there will
        /// not be any result. For more information see <see cref="LooseObjectHolder.GetSymmetricMappings"/>.
        /// </summary>
        /// <returns>The numerable of all possible mappings keeping the symmetry.</returns>
        public IEnumerable<IReadOnlyDictionary<ConfigurationObject, ConfigurationObject>> GetSymmetryMappings()
            // Take all symmetric mappings
            => LooseObjectsHolder.GetSymmetricMappings()
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
            // Take all symmetric mappings
            => LooseObjectsHolder.GetSymmetricMappings()
                // For a given mapping take the constructed objects
                .Select(mapping => ConstructedObjects
                    // Reconstruct them
                    .Select(construtedObject => (ConstructedConfigurationObject)construtedObject.Remap(mapping))
                    // Exclude the ones that we already have. This way we get the objects 
                    // that are missing in order for this mapping to prove symmetry. 
                    .Except(ConstructedObjects)
                    // Enumerate
                    .ToArray());

        /// <summary>
        /// Determines the objects that would make this configuration fully symmetric if they were added to it.
        /// Fully symmetric configuration means that every symmetry mappings provides the same configuration.
        /// (see <see cref="LooseObjectHolder.GetSymmetricMappings"/>).
        /// </summary>
        /// <returns>An enumerable of objects with which the configuration would be symmetric.</returns>
        public IReadOnlyList<ConstructedConfigurationObject> GetObjectsThatWouldMakeThisConfigurationFullySymmetric()
        {
            // Prepare the resulting objects of the final configuration
            var objects = new HashSet<ConstructedConfigurationObject>(ConstructedObjects);

            // Prepare the queue of new objects on which the symmetry should be applied
            var toProcces = new Queue<ConstructedConfigurationObject>(ConstructedObjects);

            // While there are any objects to be processed
            while (toProcces.Any())
            {
                // Take some
                var currentObject = toProcces.Dequeue();

                // Apply all the symmetries on it
                foreach (var symmetryMapping in LooseObjectsHolder.GetSymmetricMappings())
                {
                    // Get the new object
                    var newObject = (ConstructedConfigurationObject)currentObject.Remap(symmetryMapping);

                    // Try to add it
                    if (objects.Add(newObject))
                        // If it's new, we will need to process it
                        toProcces.Enqueue(newObject);
                }
            }

            // Return the objects without our original ones
            return objects.Except(ConstructedObjects).ToList();
        }

        /// <summary>
        /// Reorders the loose object of a configuration that have a triangle layout in such a way that 
        /// the first will be the one that is fixed in the symmetry remapping (the drawing will cause
        /// it to be 'above').
        /// </summary>
        /// <param name="theorem">The theorem that contributes to the symmetry.</param>
        /// <returns>The configuration with changed loose objects, or the same configuration if the change cannot be done.</returns>
        public Configuration NormalizeOrderOfLooseObjectsBasedOnSymmetry(Theorem theorem)
        {
            // We will make a use of a simple function that converts an object mapping to a loose object mapping
            static IReadOnlyDictionary<LooseConfigurationObject, LooseConfigurationObject> ExtractLooseObjects(IReadOnlyDictionary<ConfigurationObject, ConfigurationObject> mapping)
                // Take pairs of loose objects
                => mapping.Where(pair => pair.Key is LooseConfigurationObject)
                        // Recreate the dictionary
                        .ToDictionary(pair => (LooseConfigurationObject)pair.Key, pair => (LooseConfigurationObject)pair.Value);

            // Another useful function extracts a pair of exchanged objects from a mapping
            static (LooseConfigurationObject, LooseConfigurationObject) ExtractExchangedObjects(IReadOnlyDictionary<LooseConfigurationObject, LooseConfigurationObject> mapping)
                // First make tuples
                => mapping.Select(pair => (pair.Key, pair.Value))
                    // Exclude identities
                    .Where(pair => !pair.Key.Equals(pair.Value))
                    // Now take the first needed one, or default
                    .FirstOrDefault(pair => mapping[pair.Value].Equals(pair.Key));

            // Switch based on the layout
            switch (LooseObjectsHolder.Layout)
            {
                // In triangle cases symmetry means to put A above
                case LooseObjectLayout.Triangle:
                {
                    // Take a symmetry mapping of loose objects
                    var symmetryMapping = theorem.GetSymmetryMappings(this).Select(ExtractLooseObjects).FirstOrDefault();

                    // If there is no symmetry, no change
                    if (symmetryMapping == null)
                        return this;

                    // Otherwise there must be two exchanged points
                    var (point1, point2) = ExtractExchangedObjects(symmetryMapping);

                    // Find the last one as well
                    var point3 = symmetryMapping.Keys.Except(new[] { point1, point2 }).Single();

                    // Get the final order in which the fixed point goes first
                    var newLooseObjects = new LooseObjectHolder(new[] { point3, point1, point2 }, LooseObjectLayout.Triangle);

                    // Return the final result
                    return new Configuration(newLooseObjects, ConstructedObjects);
                }

                // In quadrilateral cases there are two mains to make symmetry
                case LooseObjectLayout.Quadrilateral:
                {
                    // Prepare a resulting configuration
                    var configuration = this;

                    // Go through the symmetry mappings 
                    foreach (var symmetryMapping in theorem.GetSymmetryMappings(this).Select(ExtractLooseObjects))
                    {
                        // There must be two exchanged points
                        var (point1, point2) = ExtractExchangedObjects(symmetryMapping);

                        // Get the other two points as well
                        var otherPoints = symmetryMapping.Keys.Except(new[] { point1, point2 }).ToArray();

                        // In the ideal case we want to place a side horizontally
                        // But that will look symmetric only if the other points are exchangeable as well
                        // That can be tested by testing just one point
                        if (symmetryMapping[otherPoints[0]].Equals(otherPoints[1]))
                        {
                            // If this is the case, then we will not get a better order
                            // We can even choose what will be a side, let's say point1, point2
                            var newLooseObjects = new LooseObjectHolder(new[] { otherPoints[0], point1, point2, otherPoints[1] }, LooseObjectLayout.Quadrilateral);

                            // Return the final result
                            return new Configuration(newLooseObjects, ConstructedObjects);
                        }
                        // Otherwise this means we cannot place a side horizontally, but we may try it with a diagonal
                        else
                        {
                            // The diagonal must be the second and the last point
                            var newLooseObjects = new LooseObjectHolder(new[] { otherPoints[0], point1, otherPoints[1], point2 }, LooseObjectLayout.Quadrilateral);

                            // Reset the configuration. We will not break because there still might be an order that allows
                            // us to place a side horizontally
                            configuration = new Configuration(newLooseObjects, ConstructedObjects);
                        }
                    }

                    // Return the configuration, no matter what happened to it
                    return configuration;
                }

                // In other cases we will not do anything
                default:
                    return this;
            }
        }

        /// <summary>
        /// Calculates the levels of objects defined as follows: 
        /// <list type="number">
        /// <item><see cref="LooseObjects"/> have levels of 0.</item>
        /// <item><see cref="ConstructedObjects"/> have levels calculated like this:
        /// If a constructed object has flattened argument objects o1,...,on, then the level
        /// is equal to the maximal level of the objects o1, ..., on, plus 1.
        /// </item>
        /// </list>
        /// </summary>
        /// <returns>The dictionary containing object levels.</returns>
        public IReadOnlyDictionary<ConfigurationObject, int> CalculateObjectLevels()
        {
            // Prepare a result
            var levels = new Dictionary<ConfigurationObject, int>();

            // Loose objects have a level of 0
            LooseObjects.ForEach(looseObject => levels.Add(looseObject, 0));

            // Calculate the levels of constructed objects
            ConstructedObjects
                // In order to calculate the level of a given one 
                .Select(constructedObject => (constructedObject, level:
                    // Take its arguments 
                    constructedObject.PassedArguments.FlattenedList
                    // Find their levels
                    .Select(argumentObject => levels[argumentObject])
                    // And take the maximal one, plus 1
                    .Max() + 1))
                // Add the calculated levels to the level dictionary
                .ForEach(pair => levels.Add(pair.constructedObject, pair.level));

            // Return the result
            return levels;
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