using GeoGen.Core;
using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.TheoremRanker
{
    /// <summary>
    /// The <see cref="IAspectTheoremRanker"/> of <see cref="RankedAspect.Simplification"/>.
    /// </summary>
    public class SimplificationRanker : AspectTheoremRankerBase
    {
        /// <inheritdoc/>
        public override double Rank(Theorem theorem, Configuration configuration, TheoremMap allTheorems)
        {
            // In order to do this ranking we need to find the levels of the objects described 
            // in the documentation of RankedAspect.Simplification. Prepare an empty dictionary
            var levels = new Dictionary<ConfigurationObject, int>();

            // Loose objects have a level of 0
            configuration.LooseObjects.ForEach(looseObject => levels.Add(looseObject, 0));

            // Calculate levels of constructed objects
            configuration.ConstructedObjects
                // In order to calculate the level of a given one take 
                .Select(constructedObject => (constructedObject, level:
                    // Take its arguments 
                    constructedObject.PassedArguments.FlattenedList
                    // Find their levels
                    .Select(argumentObject => levels[argumentObject])
                    // And take the maximal one, plus 1
                    .Max() + 1))
                // Add the calculated levels to the level dictionary
                .ForEach(pair => levels.Add(pair.constructedObject, pair.level));

            // Now we can rank the theorem by taking its inner objects
            return theorem.GetInnerConfigurationObjects()
                // Taking their levels
                .Select(innerObject => levels[innerObject])
                // Averaging them
                .Average()
                // And normalizing
                / configuration.ConstructedObjects.Count;
        }
    }
}