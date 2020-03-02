using GeoGen.Core;
using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.ConfigurationGenerator
{
    /// <summary>
    /// Represents a <see cref="IConfigurationFilter"/> that using an algorithm that caches
    /// every generated <see cref="ConstructedConfigurationObject"/> and <see cref="Configuration"/>
    /// in order to ensure we won't generate the same configuration twice. This might not be ideal
    /// during long-term runs (for example, 4000 configurations require about 1GB RAM).
    /// </summary>
    public class FastConfigurationFilter : ConfigurationFilterBase
    {
        #region Private fields

        /// <summary>
        /// The dictionary of already generated and identified constructed objects.
        /// </summary>
        private readonly Dictionary<ConstructedConfigurationObject, int> _objectIds = new Dictionary<ConstructedConfigurationObject, int>();

        /// <summary>
        /// The set of ids of objects of already generated configurations.
        /// </summary>
        private readonly HashSet<SortedSet<int>> _configurationObjectsIds = new HashSet<SortedSet<int>>(SortedSet<int>.CreateSetComparer());

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="FastConfigurationFilter"/> class.
        /// </summary>
        /// <param name="generatorInput">The input for the generator.</param>
        public FastConfigurationFilter(ConfigurationGeneratorInput generatorInput) : base(generatorInput)
        {
        }

        #endregion

        #region Public abstract methods implementation

        /// <inheritdoc/>
        public override bool ShouldBeExcluded(Configuration configuration)
        {
            // Helper function that makes sure the passed objects have ids in the ids dictionary, 
            // projects each to this id, and wraps them in a sorted set 
            SortedSet<int> MakeIdsSet(IEnumerable<ConstructedConfigurationObject> objects)
                // We simply take each object, get its id from the dictionary, or add a new one, based on its count
                => objects.Select(currentObject => _objectIds.GetValueOrCreateAddAndReturn(currentObject, () => _objectIds.Count))
                    // And enumerate to a sorted set
                    .ToSortedSet();

            // Find the ids set for the current configuration
            var currentSet = MakeIdsSet(configuration.ConstructedObjects);

            // If we already have this configuration, we won't generate it twice
            if (_configurationObjectsIds.Contains(currentSet))
                return true;

            // We need to find out if the current configuration if the representant
            // of its symmetry class. Therefore we take all the symmetric mappings                            
            var minIdsSet = configuration.LooseObjectsHolder.GetSymmetryMappings()
                    // For every make ids set from the constructed objects 
                    .Select(mapping => MakeIdsSet(configuration.ConstructedObjects
                        // that are remapped using this mapping
                        .Select(constructedObject => (ConstructedConfigurationObject)constructedObject.Remap(mapping))))
                    // Take the lexicographically minimal ids set
                    .MinItem(Comparer<SortedSet<int>>.Create((a1, a2) => a1.CompareToLexicographically(a2)));

            // If this configuration is not the representant of its symmetry class,
            // then we say it is not correct
            if (!minIdsSet.SequenceEqual(currentSet))
                return true;

            // Otherwise we mark it as a generated one
            _configurationObjectsIds.Add(currentSet);

            // And return that it's fine
            return false;
        }

        #endregion        
    }
}