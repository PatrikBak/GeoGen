using GeoGen.Core;
using GeoGen.TheoremProver;
using System.Linq;

namespace GeoGen.TheoremRanker
{
    /// <summary>
    /// The <see cref="IAspectTheoremRanker"/> of <see cref="RankedAspect.Symmetry"/>.
    /// </summary>
    public class SymmetryRanker : AspectTheoremRankerBase
    {
        /// <summary>
        /// Ranks a given theorem, potentially using all given provided context.
        /// </summary>
        /// <param name="theorem">The theorem to be ranked.</param>
        /// <param name="configuration">The configuration where the theorem holds.</param>
        /// <param name="allTheorems">The map of all theorems of the configuration.</param>
        /// <param name="proverOutput">The output from the theorem prover for all the theorems of the configuration.</param>
        /// <returns>A number representing the ranking of the theorem. The range of its values depends on the implementation.</returns>
        public override double Rank(Theorem theorem, Configuration configuration, TheoremMap allTheorems, TheoremProverOutput proverOutput)
        {
            // Find all possible mappings of loose objects
            var looseObjectsRemappings = configuration.LooseObjectsHolder.GetSymmetryMappings()
                // Exclude the identity
                .Where(mapping => mapping.Any(pair => pair.Key != pair.Value))
                // Enumerate
                .ToArray();

            // Prepare the variable indicating the number of mappings for which the configuration
            // and theorem are the same as the provided one
            var symmetryMappings = 0;

            // Go through the mappings
            foreach (var looseObjectsMapping in looseObjectsRemappings)
            {
                // Reconstruct constructed objects
                var constructedObjectsMapping = configuration.ConstructedObjects.ToDictionary(o => (ConfigurationObject)o, o => o.Remap(looseObjectsMapping));

                // Wrap them in a new configuration
                var newConfiguration = new Configuration(configuration.LooseObjectsHolder, constructedObjectsMapping.Values.Cast<ConstructedConfigurationObject>().ToList());

                // If the configurations are distinct, then this is not a symmetry mapping
                if (!configuration.Equals(newConfiguration))
                    continue;

                // Otherwise we have to remap the theorem
                // For that reason we merge the mapping of loose objects
                var finalMapping = looseObjectsMapping.Select(pair => ((ConfigurationObject)pair.Key, (ConfigurationObject)pair.Value))
                    // With the mapping of constructed objects
                    .Concat(constructedObjectsMapping.Select(pair => (pair.Key, pair.Value)))
                    // As a dictionary
                    .ToDictionary(pair => pair.Item1, pair => pair.Item2);

                // We can use this final mapping to remap the theorem
                var newTheorem = theorem.Remap(finalMapping);

                // If they are the same, then we happily have a symmetry mapping
                if (theorem.Equals(newTheorem))
                    symmetryMappings++;
            }

            // Return the final result (described in the documentation of RankedAspect.Symmetry)
            return (double)symmetryMappings / looseObjectsRemappings.Length;
        }
    }
}
