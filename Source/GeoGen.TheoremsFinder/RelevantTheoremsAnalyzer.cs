using GeoGen.Constructor;
using GeoGen.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.TheoremsFinder
{
    /// <summary>
    /// The default implementation of <see cref="IRelevantTheoremsAnalyzer"/>. 
    /// </summary>
    public class RelevantTheoremsAnalyzer : IRelevantTheoremsAnalyzer
    {
        #region Dependencies

        /// <summary>
        /// The array of all the available potential theorems analyzers.
        /// </summary>
        private readonly IPotentialTheoremsAnalyzer[] _theoremsAnalyzers;

        #endregion

        #region Private fields

        /// <summary>
        /// The settings for the analyzer.
        /// </summary>
        private readonly TheoremAnalysisSettings _settings;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RelevantTheoremsAnalyzer"/> class.
        /// </summary>
        /// <param name="settings">The settings for the analyzer.</param>
        /// <param name="theoremsAnalyzers">The array of all the available potential theorems analyzers.</param>
        public RelevantTheoremsAnalyzer(TheoremAnalysisSettings settings, IPotentialTheoremsAnalyzer[] theoremsAnalyzers)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _theoremsAnalyzers = theoremsAnalyzers ?? throw new ArgumentNullException(nameof(theoremsAnalyzers));
        }

        #endregion

        #region ITheoremsAnalyzer implementation

        /// <summary>
        /// Performs the theorem analysis for a given configuration.
        /// </summary>
        /// <param name="configuration">The configuration where we're looking for theorems.</param>
        /// <param name="manager">The manager of all the pictures where the theorems should be tested.</param>
        /// <param name="contextualPicture">The contextual picture where the configuration is drawn.</param>
        /// <returns>The output of the analyzer holding the theorems.</returns>
        public TheoremAnalysisOutput Analyze(Configuration configuration, IPicturesManager manager, IContextualPicture contextualPicture)
        {
            // Perform the verification of the theorems. We use the fact that potential verifiers generate only theorems 
            // containing new objects (other would be automatically excluded). 
            var testedTheorems = _theoremsAnalyzers.SelectMany(verifier => verifier.FindPotentialTheorems(contextualPicture))
                    // It can still happen that these theorems contains needless objects, i.e. are sub-theorems
                    // Take those potential theorems that turn out not to contain needless objects
                    .Where(potentialTheorem => !potentialTheorem.ContainsNeedlessObjects(expectedMinimalNumberOfNeededObjects: configuration.ObjectsMap.AllObjects.Count))
                    // Now we test each theorem in all the pictures. We exclude the ones
                    .Select(potentialTheorem => (potentialTheorem, truePictures: manager.Count(potentialTheorem.VerificationFunction)))
                    // Exclude the ones that are false in fewer than the given minimal number of pictures
                    .Where(tuple => tuple.truePictures >= _settings.MinimalNumberOfTruePicturesToRevalidate)
                    // Enumerate them to a list which will perform the actual validations
                    .ToList();

            // Prepare a list of theorems that turns out to be true, possible after revalidation
            var trueTheorems = new List<AnalyzedTheorem>();

            // Prepare a list of theorems that are potentially false negatives, i.e. true in some reasonable number of pictures
            var potentialFalseNegatives = new List<AnalyzedTheorem>();

            // Prepare a variable indicated if we attempted to recreate the picture yet
            // null means we didn't, false means we did and it wasn't successful, true means it went fine
            bool? wasPictureRecreated = null;

            // Handle all the theorems we're left with
            foreach (var (potentialTheorem, truePictures) in testedTheorems)
            {
                // If the theorem is true in enough pictures...
                if (truePictures >= _settings.MinimalNumberOfTruePictures)
                {
                    // Then we add the theorem to the list of certain ones
                    trueTheorems.Add(new AnalyzedTheorem(configuration, potentialTheorem, truePictures, numberOfTruePicturesAfterSecondTest: null));

                    // And continue to the next one
                    continue;
                }

                // Otherwise we have a theorem to retest
                // If we haven't recreated the picture yet...
                if (wasPictureRecreated == null)
                {
                    // We do it
                    contextualPicture.TryReconstruct(out var successful);

                    // And set if it was successful
                    wasPictureRecreated = successful;
                }

                // If the picture couldn't be recreated, then we can't perform the second phase :(
                if (!wasPictureRecreated.Value)
                {
                    // We just add the theorem to potentially false negatives
                    potentialFalseNegatives.Add(new AnalyzedTheorem(configuration, potentialTheorem, truePictures, numberOfTruePicturesAfterSecondTest: null));

                    // And continue to the next one
                    continue;
                }

                // Otherwise we can first the number of true pictures in the second phase
                var truePicturesAfterRevalidation = manager.Count(potentialTheorem.VerificationFunction);

                // Construct the resulting theorem
                var theorem = new AnalyzedTheorem(configuration, potentialTheorem, truePictures, truePicturesAfterRevalidation);

                // And decide to which list we're going to add it according to the settings
                (truePicturesAfterRevalidation >= _settings.MinimalNumberOfTruePictures ? trueTheorems : potentialFalseNegatives).Add(theorem);
            }

            // Return the final output
            return new TheoremAnalysisOutput
            {
                // Set theorems
                Theorems = trueTheorems,

                // Set false negatives
                PotentialFalseNegatives = potentialFalseNegatives
            };
        }

        #endregion
    }
}