using GeoGen.Core;
using System.Linq;

namespace GeoGen.TheoremRanker
{
    /// <summary>
    /// The <see cref="IAspectTheoremRanker"/> of <see cref="RankedAspect.Symmetry"/>.
    /// </summary>
    public class SymmetryRanker : AspectTheoremRankerBase
    {
        /// <inheritdoc/>
        public override double Rank(Theorem theorem, Configuration configuration, TheoremMap allTheorems)
        {
            // Find all possible mappings of loose objects
            var looseObjectsRemappings = configuration.LooseObjectsHolder.GetSymmetryMappings()
                // Exclude the identity
                .Where(mapping => mapping.Any(pair => pair.Key != pair.Value))
                // Enumerate
                .ToArray();

            // Prepare the variable indicating the number of mappings for which the
            // configuration and theorem stay the same
            var symmetryMappings = 0;

            // Go through the mappings
            foreach (var looseObjectMapping in looseObjectsRemappings)
            {
                // Reconstruct constructed objects
                var constructedObjectMapping = configuration.ConstructedObjects
                    // Each is remapped with respect to the loose object mapping
                    .ToDictionary(constructedObject => (ConfigurationObject)constructedObject, constructedObject => constructedObject.Remap(looseObjectMapping));

                // Wrap them in a new configuration
                var newConfiguration = new Configuration(configuration.LooseObjectsHolder, constructedObjectMapping.Values.Cast<ConstructedConfigurationObject>().ToList());

                // If the configurations are distinct, then this is not a symmetry mapping
                if (!configuration.Equals(newConfiguration))
                    continue;

                // Otherwise we have to remap the theorem
                // For that reason we merge the mapping of loose objects
                var finalMapping = looseObjectMapping.Select(pair => ((ConfigurationObject)pair.Key, (ConfigurationObject)pair.Value))
                    // With the mapping of constructed objects
                    .Concat(constructedObjectMapping.Select(pair => (pair.Key, pair.Value)))
                    // As a dictionary
                    .ToDictionary(pair => pair.Item1, pair => pair.Item2);

                // We can use this final mapping to remap the theorem
                var newTheorem = theorem.Remap(finalMapping);

                // If they are the same, then we happily have a symmetry mapping
                if (theorem.Equals(newTheorem))
                    symmetryMappings++;
            }

            // Return the final result (described in the documentation)
            return symmetryMappings switch
            {
                // No symmetry case
                _ when symmetryMappings == 0 => 0,

                // Full symmetry case
                _ when symmetryMappings == looseObjectsRemappings.Length => 1,

                // Any other case means some sort of partial symmetry
                _ => 0.5
            };
        }
    }
}
