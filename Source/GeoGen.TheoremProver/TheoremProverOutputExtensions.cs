﻿using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Extension methods for <see cref="TheoremProverOutput"/>.
    /// </summary>
    public static class TheoremProverOutputExtensions
    {
        /// <summary>
        /// Takes the unproved theorem and splits them into groups in such a way that two theorems are in a group
        /// if and only if one appears in an attempt at some of the assumptions of the second one. Theorems
        /// that don't have any attempts and don't appear in any other attempt will have groups of size 1.
        /// </summary>
        /// <param name="output">The prover output.</param>
        /// <returns>The theorem groups.</returns>
        public static IReadOnlyList<IReadOnlyHashSet<Theorem>> GroupNewTheoremsIntoRelationGroups(this TheoremProverOutput output)
        {
            // If there are no unproven theorems, we're done
            if (output.UnprovenTheorems.IsEmpty())
                return Array.Empty<IReadOnlyHashSet<Theorem>>();

            // Otherwise prepare the result with all groups
            var groups = new HashSet<HashSet<Theorem>>();

            #region Building groups

            // Go through the unproven theorem attempts
            foreach (var pair in output.UnprovenTheorems)
            {
                // Deconstruct
                var (theorem, proofAttempts) = pair;

                // Create a new group for it
                var group = new HashSet<Theorem> { theorem };

                // Add it to the result
                groups.Add(group);

                // Now we need to examined all attempts at this theorem
                // We're going to perform the standard graph search
                // Prepare the set of examined proof attempts
                var examinedAttempts = new HashSet<TheoremProofAttempt>();

                // Prepare the queue of attempts to be examined
                var attemptsToExamine = new Queue<TheoremProofAttempt>();

                // Enqueue the attempts
                proofAttempts.ForEach(attemptsToExamine.Enqueue);

                // Until there is any attempt to examine...
                while (attemptsToExamine.Any())
                {
                    // Get the current attempt
                    var currentAttempt = attemptsToExamine.Dequeue();

                    // If the attempt it examined, we won't do it again
                    if (examinedAttempts.Contains(currentAttempt))
                        continue;

                    // Otherwise mark this attempt as examined one
                    examinedAttempts.Add(currentAttempt);

                    // Go through the unproven theorems of this attempt
                    foreach (var (assumption, assumptionAttempts) in currentAttempt.UnprovenAssumptions)
                    {
                        // This assumption belongs to the theorem group
                        // However, we don't want to include discovered theorems, i.e. ones
                        // that are discovered...Therefore if this is not a discovered theorem, add it
                        if (!output.UnprovenDiscoveredTheorems.ContainsKey(assumption))
                            group.Add(assumption);

                        // In any case we want to take the attempts for the examination
                        assumptionAttempts.ForEach(attemptsToExamine.Enqueue);
                    }
                }
            }

            #endregion

            #region Merging groups

            // It might happen that some of the groups are now identical, or have common theorems
            // We're going to apply the simplest merging strategy: If two groups have a common
            // theorem, then we merge them. This doesn't always correspond to the reality, because
            // two theorems might not be equivalent, but this solution is simple and should be okay 

            // The algorithm: We look at every pair of groups. If we can merge them, we do it and
            // repeat this step. If we can't merge any two groups, we're done. This is theoretically slow,
            // but practically enough, since the number of groups will rarely be even more than 5.

            // Do until we break
            while (true)
            {
                // Prepare the variable indicating that some two groups have been merged
                var someGroupsHaveBeenMerged = false;

                // Go through all possible pairs of groups
                foreach (var groupPair in groups.Subsets(2))
                {
                    // Get the groups for comfort
                    var group1 = groupPair[0];
                    var group2 = groupPair[1];

                    // If they have a common element...
                    if (group1.Intersect(group2).Any())
                    {
                        // Then we merge them for example to the first group
                        group1.UnionWith(group2);

                        // Mark that we have some merge
                        someGroupsHaveBeenMerged = true;

                        // Remove the merged group
                        groups.Remove(group2);

                        // And break the loop
                        break;
                    }
                }

                // If no two groups have been merged, then we're done
                if (!someGroupsHaveBeenMerged)
                    break;

                // Otherwise we do try to merge another two groups in the next iteration...
            }

            #endregion

            // Right now the groups are found, we just need wrap them in read only sets
            return groups.Select(group => group.ToReadOnlyHashSet()).ToList();
        }
    }
}