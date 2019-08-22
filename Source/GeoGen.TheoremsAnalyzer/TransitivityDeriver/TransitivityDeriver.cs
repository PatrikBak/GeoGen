using GeoGen.Core;
using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.TheoremsAnalyzer
{
    /// <summary>
    /// The default implementation of <see cref="ITransitivityDeriver"/>.
    /// </summary>
    public class TransitivityDeriver : ITransitivityDeriver
    {
        /// <summary>
        /// Derives new theorems using transitivity.
        /// </summary>
        /// <param name="configuration">The configuration in which the theorems hold true.</param>
        /// <param name="trueTheorems">All theorems that are true and contain the last object of the configuration.</param>
        /// <param name="assumedTheorems">The theorems that have already been ruled out as not interesting.</param>
        /// <returns>The enumerable of tuples consisting of two used fact to draw a conclusion and the conclusion itself.</returns>
        public IEnumerable<(Theorem fact1, Theorem fact2, Theorem conclusion)> Derive(Configuration configuration, IEnumerable<Theorem> trueTheorems, IEnumerable<Theorem> assumedTheorems)
        {
            // First find equivalences of old objects that are assumed true
            var oldEquivalencies = OldObjectsEquivalencies(trueTheorems, configuration.LastConstructedObject);

            // Go through all the theorems
            foreach (var theorem in assumedTheorems)
            {
                // Get the type for comfort
                var type = theorem.Type;

                // Which based on the type
                switch (type)
                {
                    // Cases where there is one new point 
                    case TheoremType.CollinearPoints:
                    case TheoremType.ConcyclicPoints:
                    {
                        // Get the new point
                        var newPoint = configuration.LastConstructedObject;

                        // If it's not a point, we can't do more
                        if (newPoint.ObjectType != ConfigurationObjectType.Point)
                            continue;

                        // Otherwise get the equivalence set
                        var equivalencySets = oldEquivalencies[type];

                        // Get the theorem object corresponding to the new point
                        var newTheoremObject = theorem.InvolvedObjects.Single(_point => _point is PointTheoremObject point && point.ConfigurationObject == newPoint);

                        // Get the old theorem objects
                        var oldTheoremObjects = theorem.InvolvedObjects.Where(theoremObject => theoremObject != newTheoremObject);

                        // Get the group corresponding to these old objects
                        var set = equivalencySets.Single(set => set.IsSupersetOf(oldTheoremObjects));

                        // If this group contains the new object, we can't yield anything new
                        if (set.Contains(newTheoremObject))
                            continue;

                        // Otherwise we generate the subsets of collinear / concyclic points with this point
                        foreach (var subset in set.Subsets(type == TheoremType.CollinearPoints ? 2 : 3))
                        {
                            // The fact that old objects are collinear / concyclic
                            var fact1 = new Theorem(configuration, type, set.ToArray());

                            // Together with the current fact
                            var fact2 = theorem;

                            // Gives us the conclusion the points from the subset are collinear with the new one
                            var conclusion = new Theorem(configuration, type, subset.Concat(newTheoremObject).ToArray());

                            // We can return it
                            yield return (fact1, fact2, conclusion);
                        }

                        // Don't forget to add the object to the set
                        set.Add(newTheoremObject);

                        break;
                    }

                    // Cases where there might be more new objects
                    case TheoremType.ParallelLines:
                    case TheoremType.EqualLineSegments:
                    case TheoremType.EqualAngles:
                    {
                        // Get the equivalence set
                        var equivalencySets = oldEquivalencies[type];

                        // Get the objects list
                        var objects = theorem.InvolvedObjects.ToList();

                        // Get particular objects for comfort
                        var object1 = objects[0];
                        var object2 = objects[1];

                        // Get the sets for the inner objects
                        var set1 = equivalencySets.FirstOrDefault(set => set.Contains(object1));
                        var set2 = equivalencySets.FirstOrDefault(set => set.Contains(object2));

                        // If is it the same set, we can't establish a new equivalence
                        if (set1 == set2 && set1 != null)
                            continue;

                        // If none of those set exist...
                        if (set1 == null && set2 == null)
                        {
                            // We can create a new one containing our objects
                            equivalencySets.Add(new HashSet<TheoremObject>(TheoremObject.EquivalencyComparer) { object1, object2 });

                            // And move further
                            continue;
                        }

                        #region Both sets exist

                        // If both these sets exist
                        if (set1 != null && set2 != null)
                        {
                            // We know we can merge them. Before doing so, we have some new discovered equalities
                            // First, the first object is equivalent to every other object from the second set
                            foreach (var set2Object in set2)
                            {
                                // We have to ignore the second object
                                if (set2Object.IsEquivalentTo(object2))
                                    continue;

                                // Otherwise we have some discovered transitivity
                                // The current fact says object1 = object2
                                var fact1 = theorem;

                                // Plus we know object2 = set2Object, because they're in the same set
                                var fact2 = new Theorem(theorem.Configuration, type, new[] { object2, set2Object });

                                // Therefore object1 = set2Object
                                var conclusion = new Theorem(theorem.Configuration, type, new[] { object1, set2Object });

                                // Return it
                                yield return (fact1, fact2, conclusion);
                            }

                            // Now we can combine every object from the first set
                            foreach (var set1Object in set1)
                            {
                                // Except for object1
                                if (set1Object.IsEquivalentTo(object1))
                                    continue;

                                // With every object from set2
                                foreach (var set2Object in set2)
                                {
                                    // We know that set1Object = object1, because they're in the same set
                                    var fact1 = new Theorem(theorem.Configuration, type, new[] { set1Object, object1 });

                                    // Plus we know object1 = set2Object, because we made this statement in the previous non-nested loop
                                    var fact2 = new Theorem(theorem.Configuration, type, new[] { object1, set2Object });

                                    // Thus we know set1Object = set2Object
                                    var conclusion = new Theorem(theorem.Configuration, type, new[] { set1Object, set2Object });

                                    // We can return it
                                    yield return (fact1, fact2, conclusion);
                                }
                            }

                            // Finally we merge the sets
                            set1.UnionWith(set2);

                            // And remove the other one
                            equivalencySets.Remove(set2);

                            // Continue with another theorem
                            continue;
                        }

                        #endregion

                        #region One set exists

                        // At this stage we know that one set exists and the other doesn't. Find out what is what
                        var existingSet = set1 ?? set2;
                        var objectFromExistingSet = existingSet == set1 ? object1 : object2;
                        var otherObject = objectFromExistingSet == object1 ? object2 : object1;

                        // Now we go through the objects of the set
                        foreach (var setObject in existingSet)
                        {
                            // That are not equivalent to our one
                            if (setObject.IsEquivalentTo(objectFromExistingSet))
                                continue;

                            // We know that setObject = objectFromExistingSet, because they're in the same set
                            var fact1 = new Theorem(theorem.Configuration, type, new[] { objectFromExistingSet, setObject });

                            // Plus we know objectFromExistingSet = otherObject, which is basically object1 = object2
                            var fact2 = theorem;

                            // Thus we know setObject = otherObject
                            var conclusion = new Theorem(theorem.Configuration, type, new[] { setObject, otherObject });

                            // We can return it
                            yield return (fact1, fact2, conclusion);
                        }

                        // Finally we update the set
                        existingSet.Add(otherObject);

                        #endregion

                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Finds the equivalence classes of old objects based on the true theorems of the configuration.
        /// </summary>
        /// <param name="theorems">All theorems that are true in the configuration.</param>
        /// <param name="newObject">The new object of the configuration (all the other objects are old).</param>
        /// <returns>The dictionary mapping each theorem type to a list of equivalent objects.</returns>
        private Dictionary<TheoremType, List<HashSet<TheoremObject>>> OldObjectsEquivalencies(IEnumerable<Theorem> theorems, ConfigurationObject newObject)
        {
            // Prepare the result
            var result = new Dictionary<TheoremType, List<HashSet<TheoremObject>>>();

            // Go through all the theorems
            theorems.ForEach(theorem =>
            {
                // Get the number of needed objects that identifies a theorem type
                var neededObjects = theorem.Type switch
                {
                    TheoremType.ConcyclicPoints => 3,
                    TheoremType.CollinearPoints => 2,
                    TheoremType.ParallelLines => 1,
                    TheoremType.EqualLineSegments => 1,
                    TheoremType.EqualAngles => 1,

                    // All other cases don't support transitivity
                    _ => 0
                };

                // If the theorem type doesn't support transitivity, we can't do more
                if (neededObjects == 0)
                    return;

                // Get the equivalence set
                var equivalencySets = result.GetOrAdd(theorem.Type, () => new List<HashSet<TheoremObject>>());

                // Find the first set that is identified by some n-tuples
                // of the current theorem objects, where 'n' is 'neededObjects'
                var (subset, currentSet) = theorem.InvolvedObjects.Subsets(neededObjects)
                    // For each n-tuple we try to find the group
                    .Select(subset => (subset, set: equivalencySets.FirstOrDefault(set => set.IsSupersetOf(subset))))
                    // Take the first successful one
                    .FirstOrDefault(pair => pair.set != null);

                // If there is no set, then our objects are new ones
                if (currentSet == null)
                {
                    // We need to add a new one
                    equivalencySets.Add(new HashSet<TheoremObject>(theorem.InvolvedObjects, TheoremObject.EquivalencyComparer));

                    // And we can't do more
                    return;
                }

                // Otherwise we need to get the theorem object that doesn't 
                // belong to the subset that identified the current set
                var newObject = theorem.InvolvedObjects.ToSet(TheoremObject.EquivalencyComparer).Except(subset).Single();

                // If the current object is currently in the set, we can't do more
                if (currentSet.Contains(newObject))
                    return;

                // Otherwise we add it to the set
                currentSet.Add(newObject);

                // Adding this element might have caused some sets represents
                // the same equivalence class. We need to gradually merge them
                equivalencySets.Where(set => set != currentSet
                    // Merging happen based on the number of needed common objects
                    && currentSet.Intersect(set).Count() >= neededObjects).ForEach(set =>
                    {
                        // We merge the set
                        currentSet.UnionWith(set);

                        // Make the other empty so it can be spotted and deleted
                        set.Clear();
                    });

                // Delete all the sets that have been merged
                equivalencySets.RemoveAll(set => set.Count == 0);
            });

            // Now we need to exclude new objects from every created set
            // Every equality of old objects is implicitly assumed, since
            // it holds true in a simpler configuration
            result.Values.ForEach(list => list.ForEach(set => set.RemoveWhere(theoremObject =>
            {
                // A theorem object is new if all its definitions contain the last object of the configuration of the original theorem
                return theoremObject.GetAllDefinitions().All(definition => definition.Contains(newObject));
            })));

            // Return the result
            return result;
        }
    }
}