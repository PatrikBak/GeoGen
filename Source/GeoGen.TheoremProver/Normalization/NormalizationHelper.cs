using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using static GeoGen.Core.TheoremType;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Represents a helper service that takes of <see cref="ConfigurationObject"/>s, proved <see cref="Theorem"/>s, <see cref="Theorem"/>s
    /// to be proved, with respect to a <see cref="Configuration"/> to which these things are relevant. The main future is that it makes sure 
    /// that objects and theorems there have exactly one version -- the normal version. 
    /// <para>
    /// Objects of the initial <see cref="Configuration"/> are said to be normal and this will never change. When we start introducing new
    /// objects, either implicitly via <see cref="MarkProvedEquality(Theorem, out bool, out bool, out NewEqualityResult)"/> calls, or explicitly via
    /// <see cref="IntroduceNewObject(ConstructedConfigurationObject)"/>, then we might find that an object has more equal versions, but it
    /// will always have only one normal version that can be retrieved via <see cref="GetNormalVersionOfObjectOrNull(ConfigurationObject)"/>.
    /// </para>
    /// <para>
    /// The class also takes care that every <see cref="ConstructedConfigurationObject"/> contains within its arguments only objects that
    /// are equal to their normal version (i.e. normal objects). The reason is obvious -- if x = y, then certainly f(x) = f(y) and this is
    /// a redundant piece of information.
    /// </para>
    /// <para>
    /// <see cref="Theorem"/>s are normal if their inner <see cref="ConfigurationObject"/>s are normal. By keeping all proved theorems 
    /// normalized we will dramatically reduce the number of theorems saying the same thing and therefore the whole system will be 
    /// much faster. Proved theorems are added via <see cref="MarkProvedEquality(Theorem, out bool, out bool, out NewEqualityResult)"/> and 
    /// <see cref="MarkProvedNonequality(Theorem, out bool, out Theorem, out Theorem[])"/> methods and are supposed to contain only
    /// correct inner <see cref="ConstructedConfigurationObject"/>s, i.e. those whose inner objects are normalized. The helper checks 
    /// marked theorems geometrically via <see cref="IGeometricTheoremVerifier"/>.
    /// </para>
    /// <para>
    /// The helper can handle introduction of new objects via <see cref="IntroduceNewObject(ConstructedConfigurationObject)"/>, and even
    /// their removal via <see cref="RemoveIntroducedObject(ConstructedConfigurationObject, out ConstructedConfigurationObject[])"/>.
    /// </para>
    /// </summary>
    public class NormalizationHelper
    {
        #region Dependencies

        /// <summary>
        /// The service that verifiers whether theorems are true numerically.
        /// </summary>
        private readonly IGeometricTheoremVerifier _verifier;

        #endregion

        #region Private fields

        /// <summary>
        /// The pictures where the examined configuration is drawn.
        /// </summary>
        private readonly PicturesOfConfiguration _pictures;

        /// <summary>
        /// The set of theorems that are to be proved.
        /// </summary>
        private readonly HashSet<Theorem> _theoremsToProve = new HashSet<Theorem>();

        /// <summary>
        /// The set of proved theorems.
        /// </summary>
        private readonly HashSet<Theorem> _provedTheorems = new HashSet<Theorem>();

        /// <summary>
        /// The dictionary mapping theorem types to sets of proved theorems with this type. 
        /// </summary>
        private readonly Dictionary<TheoremType, HashSet<Theorem>> _provedTheoremsByType = new Dictionary<TheoremType, HashSet<Theorem>>();

        /// <summary>
        /// The dictionary mapping objects to set of theorems containing this object among their inner objects.
        /// </summary>
        private readonly Dictionary<ConfigurationObject, HashSet<Theorem>> _provedTheoremsByObject = new Dictionary<ConfigurationObject, HashSet<Theorem>>();

        /// <summary>
        /// The dictionary mapping constructions to sets of constructed objects with this configuration.
        /// </summary>
        private readonly Dictionary<Construction, HashSet<ConstructedConfigurationObject>> _objectsByConstruction = new Dictionary<Construction, HashSet<ConstructedConfigurationObject>>();

        /// <summary>
        /// The dictionary mapping objects to their normal version.
        /// </summary>
        private readonly Dictionary<ConfigurationObject, ConfigurationObject> _objectToItsNormalVersion = new Dictionary<ConfigurationObject, ConfigurationObject>();

        /// <summary>
        /// All objects in the normal version in a constructible order.
        /// </summary>
        private readonly List<ConfigurationObject> _orderedNormalVersions = new List<ConfigurationObject>();

        /// <summary>
        /// The dictionary mapping objects introduces via <see cref="IntroduceNewObject(ConstructedConfigurationObject)"/> method to their normal version.
        /// </summary>
        private readonly Dictionary<ConfigurationObject, ConfigurationObject> _introducedObjectsNormalVersions = new Dictionary<ConfigurationObject, ConfigurationObject>();

        /// <summary>
        /// The set of all objects that have ever been removed via <see cref="RemoveIntroducedObject(ConstructedConfigurationObject, out ConstructedConfigurationObject[])"/>.
        /// </summary>
        private readonly HashSet<ConstructedConfigurationObject> _removedIntroducedObjects = new HashSet<ConstructedConfigurationObject>();

        #endregion

        #region Public properties

        /// <summary>
        /// The read-only set of all proved theorems.
        /// </summary>
        public IReadOnlyHashSet<Theorem> ProvedTheorems => new ReadOnlyHashSet<Theorem>(_provedTheorems);

        /// <summary>
        /// Indicates whether there is any theorem to be proved.
        /// </summary>
        public bool AnythingLeftToProve => _theoremsToProve.Any();

        /// <summary>
        /// The constructed objects known to the helper.
        /// </summary>
        public IEnumerable<ConstructedConfigurationObject> AllObjects => _objectToItsNormalVersion.Keys.OfType<ConstructedConfigurationObject>();

        /// <summary>
        /// The set that have ever been removed via <see cref="RemoveIntroducedObject(ConstructedConfigurationObject, out ConstructedConfigurationObject[])"/>.
        /// </summary>
        public IEnumerable<ConstructedConfigurationObject> AllRemovedObjects => _removedIntroducedObjects;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="NormalizationHelper"/> class.
        /// </summary>
        /// <param name="verifier">The service that verifiers whether theorems are true numerically.</param>
        /// <param name="pictures">The pictures where the examined configuration is drawn.</param>
        /// <param name="provedTheorems">The theorems that are considered true up until this point. It cannot contain theorems with type <see cref="EqualObjects"/>.</param>
        /// <param name="theoremsToProve">The theorems that are to be proved.</param>
        public NormalizationHelper(IGeometricTheoremVerifier verifier, PicturesOfConfiguration pictures, IEnumerable<Theorem> provedTheorems, IEnumerable<Theorem> theoremsToProve)
        {
            _verifier = verifier ?? throw new ArgumentNullException(nameof(verifier));
            _pictures = pictures ?? throw new ArgumentNullException(nameof(pictures));

            // Add the theorems to prove
            theoremsToProve.ForEach(theorem => _theoremsToProve.Add(theorem));

            // Add the theorems to corresponding collections
            provedTheorems.ForEach(AddTheoremToCollections);

            // Mark the constructed objects of the configuration by construction
            pictures.Configuration.ConstructedObjects.ForEach(MarkObjectByConstruction);

            // Ensure every object is initially in its normal version
            pictures.Configuration.AllObjects.ForEach(initialObject => _objectToItsNormalVersion.Add(initialObject, initialObject));

            // Ensure these normal versions are ordered in the way they appeared in the configuration            
            pictures.Configuration.AllObjects.ForEach(_orderedNormalVersions.Add);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Gets the proved theorems that have a given type.
        /// </summary>
        /// <param name="theoremType">The type of theorems we're querying.</param>
        /// <returns>The proved theorems with the passed type.</returns>
        public IEnumerable<Theorem> GetProvedTheoremOfType(TheoremType theoremType)
            // Try to get it from the dictionary or return an empty enumerable
            => _provedTheoremsByType.GetValueOrDefault(theoremType) ?? Enumerable.Empty<Theorem>();

        /// <summary>
        /// Gets the constructed objects that have a given construction.
        /// </summary>
        /// <param name="construction">The construction used in constructed objects that we're looking for.</param>
        /// <returns>The constructed objects with the passed construction.</returns>
        public IEnumerable<ConstructedConfigurationObject> GetObjectsWithConstruction(Construction construction)
             // Try to get it from the dictionary or return an empty enumerable
             => _objectsByConstruction.GetValueOrDefault(construction) ?? Enumerable.Empty<ConstructedConfigurationObject>();

        /// <summary>
        /// Gets the normal version of a passed object, or null, if the object is no known to the helper.
        /// </summary>
        /// <param name="configurationObject">The object for which we're for the normal version.</param>
        /// <returns>Either the normal version of the object; if it is known to the helper, or null otherwise.</returns>
        public ConfigurationObject GetNormalVersionOfObjectOrNull(ConfigurationObject configurationObject)
            // Try to get it from the dictionary. If it's not there, null will be returned
            => _objectToItsNormalVersion.GetValueOrDefault(configurationObject);

        /// <summary>
        /// Gets the constructed objects that have a given construction and are equal to the passed object.
        /// </summary>
        /// <param name="configurationObject">The configuration object for which we're looking equal ones.</param>
        /// <param name="construction">The construction of all objects we're looking for.</param>
        /// <returns>The constructed objects equal to the passed one with the passed construction.</returns>
        public IEnumerable<ConstructedConfigurationObject> GetEqualObjects(ConfigurationObject configurationObject, Construction construction)
        {
            // Find the normal version of the passed object first
            var normalVersion = _objectToItsNormalVersion[configurationObject];

            // Take all the objects with the passed construction
            return _objectsByConstruction.GetValueOrDefault(construction)?
                // Whose normal version is equal to the normal version of the passed object, i.e. they are equal
                .Where(constructedObject => _objectToItsNormalVersion[constructedObject].Equals(normalVersion))
                // If there are no such subjects, return an empty enumerable
                ?? Enumerable.Empty<ConstructedConfigurationObject>();
        }

        /// <summary>
        /// Finds if a given constructed object is equal to an object of the original configuration.
        /// </summary>
        /// <param name="constructedObject">The constructed object to be checked.</param>
        /// <returns>true, if this object is equal to an object of the original configuration; false otherwise.</returns>
        public bool IsItOriginalConfiguration(ConstructedConfigurationObject constructedObject)
            // This is true if the object has a normal version
            => _objectToItsNormalVersion.ContainsKey(constructedObject)
                // And its normal version is in the original configuration
                && _pictures.Configuration.AllObjects.Contains(_objectToItsNormalVersion[constructedObject]);

        /// <summary>
        /// Finds out if a given theorem contains an incorrect object, i.e. a constructed object that has an argument
        /// that is not normalized.
        /// </summary>
        /// <param name="theorem">The theorem to be checked.</param>
        /// <returns>true, if the theorem contains incorrect objects; false otherwise.</returns>
        public bool DoesTheoremContainAnyIncorrectObject(Theorem theorem)
            // In order to find this out take the inner objects
            => theorem.GetInnerConfigurationObjects()
                // That are constructed
                .OfType<ConstructedConfigurationObject>()
                // Look for one that has an argument
                .Any(innerObject => innerObject.PassedArguments.FlattenedList
                    // That is not equal to its normalized version
                    .Any(argument => !argument.Equals(_objectToItsNormalVersion.GetValueOrDefault(argument))));

        /// <summary>
        /// Marks that a given theorem, whose type is <see cref="EqualObjects"/>, is proved, and performs normalization if needed.
        /// If the theorem is new, it is geometrically validated so that we are sure we will not reach an incorrect state.
        /// </summary>
        /// <param name="provedEquality">The proved equality theorem that has been established correct.</param>
        /// <param name="isNew">Indicates whether the theorem is new to the helper.</param>
        /// <param name="isValid">Indicates whether the theorem is geometrically valid.</param>
        /// <param name="equalityResult">The result containing object and theorem changes that this equality might have triggered.</param>
        public void MarkProvedEquality(Theorem provedEquality, out bool isNew, out bool isValid, out NewEqualityResult equalityResult)
        {
            #region At most one known object

            // Ensure this is an equality
            if (provedEquality.Type != EqualObjects)
                throw new TheoremProverException($"Invalid type of theorem, expected {EqualObjects}.");

            // Find the inner objects that are equal
            var object1 = provedEquality.InvolvedObjectsList[0].GetInnerConfigurationObjects().Single();
            var object2 = provedEquality.InvolvedObjectsList[1].GetInnerConfigurationObjects().Single();

            // Find out which of them are known
            var isObject1Known = _objectToItsNormalVersion.ContainsKey(object1);
            var isObject2Known = _objectToItsNormalVersion.ContainsKey(object2);

            // We will not allow both of them not being known
            if (!isObject1Known && !isObject2Known)
                throw new TheoremProverException($"New equality theorems must contain at least one already known object.");

            // Case when there is a new object
            if (!isObject1Known || !isObject2Known)
            {
                // This means we have a new theorem
                isNew = true;

                // We need to check if it really is geometrically valid
                if (!IsNewProvedTheoremGeometricallyCorrect(provedEquality))
                {
                    // It's invalid
                    isValid = false;

                    // No changes
                    equalityResult = null;

                    // We're done
                    return;
                }

                // If we got here, then the theorem is valid
                isValid = true;

                // And there is a new object, which must be constructed
                var newObject = (ConstructedConfigurationObject)(isObject1Known ? object2 : object1);

                // Make sure the new object is marked 
                MarkObjectByConstruction(newObject);

                // Get the old object
                var oldObject = isObject1Known ? object1 : object2;

                // Mark sure the new object has the normal version of the old one assigned as its normal version
                _objectToItsNormalVersion.Add(newObject, _objectToItsNormalVersion[oldObject]);

                // Set the result to a result that indicates adding this new object and no further change
                equalityResult = new NewEqualityResult(newObject);

                // We're done
                return;
            }

            #endregion

            #region Two known objects

            // If we got here, then both objects are known. Find their normal versions
            var object1NormalVersion = _objectToItsNormalVersion[object1];
            var object2NormalVersion = _objectToItsNormalVersion[object2];

            // Handle the case when the equality of these objects is known, 
            // which is indicated by the equality of their normal versions
            if (object1NormalVersion.Equals(object2NormalVersion))
            {
                // The equality is not new
                isNew = false;

                // But it is valid
                isValid = true;

                // No changes
                equalityResult = null;

                // We're done
                return;
            }

            // If we got here, the theorem is not new
            isNew = true;

            // If we got here, we have an equality of the two normalized objects
            // First we check if it is even a valid one
            if (!IsNewProvedTheoremGeometricallyCorrect(provedEquality))
            {
                // It's invalid
                isValid = false;

                // No changes
                equalityResult = null;

                // We're done
                return;
            }

            // If we got here, the theorem is valid
            isValid = true;

            // We are in a situation where we have introduced an object that turns out to be equal to an already introduced 
            // one. This equality should therefore make them have the same normal version. This change of normal versions 
            // might trigger other changes (for example, if Y --> X, and there is a normal version PointReflection(Y, Z), then 
            // PointReflection(Y, Z) --> PointReflection(X, Z). This may again trigger other changes like that...The following lines
            // might seem a bit complicated. They are like that. It is a result of a week during which I had about 6 simpler versions 
            // that had a catch. That makes be believe this really is a difficult problem.

            // Before doing anything else, copy the current normal versions. It will become useful later
            var oldObjectToItsNormalVersion = new Dictionary<ConfigurationObject, ConfigurationObject>(_objectToItsNormalVersion);

            #region Finding equalities triggered by the new one

            // Prepare the equality helper that will find equality groups from the equalities we will tell it
            var equalityHelper = new EqualityHelper();

            // Initially every object is equal to its normal version
            oldObjectToItsNormalVersion.ForEach(pair => equalityHelper.MarkEqualObjects((pair.Key, pair.Value)));

            // And the given two objects are equal as well
            equalityHelper.MarkEqualObjects((object1, object2));

            // We will keep adding equalities until they won't cause any change
            while (true)
            {
                // Prepare a variable that indicates whether any change happened
                var didAnyChangeHappen = false;

                // We will try to alter arguments of objects to get equal objects, for example Midpoint(A,B) = Midpoint(A',B)
                // if A = A'. The only objects that can appear in arguments of constructions are current normal versions, that
                // are in the list of ordered normal versions. Some of them might be already equal. We will simply go through
                // all possible pairs of these normal versions and do 1 --> 2 if they are equal according to the equality helper.
                foreach (var (normalVersion1, normalVersion2) in _orderedNormalVersions.CombinedWith(_orderedNormalVersions))
                {
                    // Skip if they are not equal
                    if (!equalityHelper.AreEqual(normalVersion1, normalVersion2))
                        continue;

                    // Skip trivial equality
                    if (normalVersion1.Equals(normalVersion2))
                        continue;

                    // We're going to do the change of normalVersion1 to normalVersion2 in arguments of objects
                    equalityHelper.AllObjects
                        // These objects must be constructed
                        .OfType<ConstructedConfigurationObject>()
                        // Enumerate as we might modify the helper
                        .ToArray()
                        // Try to change a given one
                        .ForEach(constructedObject =>
                        {
                            // Construct the change object by coping the construction
                            var changedObject = new ConstructedConfigurationObject(constructedObject.Construction,
                                    // And altering its arguments list
                                    constructedObject.PassedArguments.FlattenedList
                                        // By replacing any object equal to normalVersion1 with normalVersion2
                                        .Select(argument => argument.Equals(normalVersion1) ? normalVersion2 : argument).ToArray());

                            // The changed object is in most cases equal to the original one. In any case, the equality helper
                            // will handle it. Mark this equality to it and return if it caused any new equality
                            equalityHelper.MarkEqualObjects((constructedObject, changedObject), isThisEqualityNew: out var didAnyChangeHappenNow);

                            // If it caused a new equality, then we will set then in the whole iteration there has been a change
                            if (didAnyChangeHappenNow)
                                didAnyChangeHappen = true;
                        });
                }

                // If there has been no new equality in this iteration, we're done, other iterations would be the same
                if (!didAnyChangeHappen)
                    break;
            }

            #endregion

            #region Finding new normal versions

            // First we will take equality groups and pick a candidate for the normal version of every 
            // object within the group. This is not the end, because we might pick a candidate whose 
            // arguments are not normal (if we have two groups and pick X from one and f(Y) from the other)
            // We will fix this issue later, therefore we will call the current candidates pre-normal versions.
            // Take the equality groups 
            var prenormalVersions = equalityHelper.EqualityGroups
                // From each try to take the object of the original configuration as the pre-normal version 
                .Select(group => group.FirstOrDefault(_pictures.Configuration.AllObjects.Contains)
                    // If it's not there, then pick some old normal version (there should be at least one)
                    // Theoretically speaking, it shouldn't matter what we're going to pick, but it's better
                    // to pick an old normal version because we will not change lots of objects like this.
                    // Another reason why we will keep the old normal versions is that we already have 
                    // them ordered in a constructible way, which will be convenient in their normalization
                    ?? group.First(_orderedNormalVersions.Contains))
                // Enumerate
                .ToArray();

            // We will use the dictionary that maps object to their pre-normal version
            // We can simply build it by taking the pre-normal versions
            var objectToPrenormalVersion = prenormalVersions
                // For each we will find equal objects to it
                .SelectMany(prenormalVersion => equalityHelper.GetEqualObjects(prenormalVersion)
                    // For every equal object we will return it pairs with the pre-normal version
                    .Select(equalObject => (equalObject, prenormalVersion)))
                // Now we can make a dictionary
                .ToDictionary(pair => pair.equalObject, pair => pair.prenormalVersion);

            // Now its finally time to normalized the pre-normal versions. Prepare a dictionary with the result
            var prenormalVersionToNormalVersion = new Dictionary<ConfigurationObject, ConfigurationObject>();

            // We will also need to find the new order of normal versions. This order will correspond to the
            // order in which we will find construct them from the pre-normal versions
            var newOrderOfNormalVersions = new List<ConfigurationObject>();

            // This is not a simply problem. If our pre-normal versions are for example x, f(x), g(f(y)) and we know x = y,
            // which means f(x) = f(y), then we cannot start our pre-normalization with g(f(y)) --> f(y) needs to be
            // pre-normalized first. Therefore we need to order the pre-normal first in such a way that inner arguments
            // of every objects are pre-normalized earlier. Therefore we take the pre-normal versions
            prenormalVersions
                // And order them
                .OrderBy(prenormalVersion => (prenormalVersion switch
                {
                    // If this pre-normal version is an object of the original configuration, then we want to be
                    // at the beginning, these objects have arguments only from the original configuration and therefore
                    // no further normalization is needed in such cases
                    _ when _pictures.Configuration.AllObjects.Contains(prenormalVersion) => -1,

                    // If we have a constructed pre-normal version, then we need to have a look at the arguments
                    ConstructedConfigurationObject constructedPrenormalVersion => constructedPrenormalVersion.PassedArguments.FlattenedList
                        // The logical time when this object needs to be normalized correspond to the index of the 
                        // latest argument object (we need to have it normalized before this object). Since we picked
                        // our pre-normal versions from the original normal versions, we have them ordered in a constructible way
                        .Select(argumentObject => _orderedNormalVersions.IndexOf(objectToPrenormalVersion[argumentObject])).Max(),

                    // Unhandled cases
                    _ => throw new TheoremProverException($"Unhandled type of {nameof(ConfigurationObject)}: {prenormalVersion.GetType()}")
                }))
                // We now iterate over the pre-normal objects in the right order and therefore we can do the pre-normalization
                // Each pre-normal version will be returned together with the normal version
                .Select(prenormalVersion => (prenormalVersion switch
                {
                    // The objects from the original configuration don't need re-normalization and will keep there normal version
                    _ when _pictures.Configuration.AllObjects.Contains(prenormalVersion) => (prenormalVersion, normalVersion: prenormalVersion),

                    // If we have a constructed pre-normal version
                    ConstructedConfigurationObject constructedPrenormalVersion => (prenormalVersion,
                        // Then we need to reconstruct the object to get the normal version. We will keep the construction
                        normalVersion: new ConstructedConfigurationObject(constructedPrenormalVersion.Construction,
                            // And alter the arguments
                            constructedPrenormalVersion.PassedArguments.FlattenedList
                                // Where each will be first converted to the pre-normal version and then to already established
                                // normal version (they are already ordered in a way that this pre-normal version is already normalized)
                                .Select(argumentObject => prenormalVersionToNormalVersion[objectToPrenormalVersion[argumentObject]]).ToArray())),

                    // Unhandled cases
                    _ => throw new TheoremProverException($"Unhandled type of {nameof(ConfigurationObject)}: {prenormalVersion.GetType()}")
                }))
                // Handle each pair of pre-normal and normal version
                .ForEach(pair =>
                {
                    // Deconstruct
                    var (prenormalVersion, normalVersion) = pair;

                    // Mark this result in the dictionary mapping pre-normal versions to newly crafted normal ones
                    prenormalVersionToNormalVersion.Add(prenormalVersion, normalVersion);

                    // Add the new normal version to the list of ordered normal versions
                    newOrderOfNormalVersions.Add(normalVersion);
                });

            // Set the new ordered normal versions 
            _orderedNormalVersions.SetItems(newOrderOfNormalVersions);

            #endregion

            #region Updating normal versions of all objects

            // We finally have found new normal versions and we need to update them in the main dictionary
            // Therefore we take the new normal versions (values of the pre-normal dictionary)
            prenormalVersionToNormalVersion.Values
                // Handle each
                .ForEach(newNormalVersion =>
                {
                    // For very object that is equal to this new normal version
                    equalityHelper.GetEqualObjects(newNormalVersion).ForEach(equalObject =>
                    {
                        // Re-assign the new normal version
                        _objectToItsNormalVersion[equalObject] = newNormalVersion;

                        // If the current object is constructed
                        if (equalObject is ConstructedConfigurationObject constructedEqualObject)
                        {
                            // Then it might not its arguments normalized. We will construct its version that has the 
                            // arguments normalized and add it to the dictionary as well. Therefore we copy the construction
                            var normalizedObject = new ConstructedConfigurationObject(constructedEqualObject.Construction,
                                // And for every argument find the normal version
                                constructedEqualObject.PassedArguments.FlattenedList.Select(argumentObject => _objectToItsNormalVersion[argumentObject]).ToArray());

                            // Add the normalized version of the object to the dictionary
                            _objectToItsNormalVersion[normalizedObject] = newNormalVersion;
                        }
                    });
                });

            #endregion

            #region Updating normal versions of introduced objects

            // The changes of normal versions might have causes the dictionary mapping introduced objects
            // to their current normal versions lying now. We need to fix this as well. Take all the pairs
            _introducedObjectsNormalVersions
               // Where the normal version is no longer normal
               .Where(pair => !pair.Value.Equals(_objectToItsNormalVersion.GetValueOrDefault(pair.Value)))
               // Enumerate so we can modify the dictionary
               .ToArray()
               // Handle each change
               .ForEach(pair =>
               {
                   // Deconstruct
                   var (introducedObject, oldNormalVersion) = pair;

                   // Update the normal version with the current one
                   _introducedObjectsNormalVersions[introducedObject] = _objectToItsNormalVersion[oldNormalVersion];
               });

            #endregion

            #region Theorem normalization

            // Prepare a list of theorems that are no longer valid, i.e. contain objects that are no longer normalized
            var dismissedTheorems = new List<Theorem>();

            // Prepare a list of new normalized theorems with the original theorems and the equalities using during the normalization
            var normalizedNewTheorems = new List<(Theorem originalTheorem, Theorem[] equalities, Theorem normalizedTheorem)>();

            // Prepare a set of the current normalized theorems used to prevent duplicated of normalized theorems
            var normalizedTheoremSet = new HashSet<Theorem>();

            // Go through the currently known theorems
            foreach (var originalTheorem in _provedTheorems)
            {
                // Normalize a given one
                var normalizedTheorem = originalTheorem.Remap(_objectToItsNormalVersion);

                // If the normalization didn't change anything, then everything's 
                if (originalTheorem.Equals(normalizedTheorem))
                    continue;

                // Otherwise the original theorem is no longer valid
                dismissedTheorems.Add(originalTheorem);

                // And if the normalized theorem is not among the proved ones or locally normalized
                if (!_provedTheorems.Contains(normalizedTheorem) && !normalizedTheoremSet.Contains(normalizedTheorem))
                {
                    // Then we mark the result of this normalization
                    normalizedNewTheorems.Add((originalTheorem, FindUsedEqualities(originalTheorem), normalizedTheorem));

                    // And mark that we have this normalized theorem in the set
                    normalizedTheoremSet.Add(normalizedTheorem);
                }
            }

            // Now we can remove the theorems that are no longer valid
            dismissedTheorems.ForEach(RemoveTheoremFromCollections);

            // And add the new theorems 
            normalizedNewTheorems.ForEach(triple => AddTheoremToCollections(triple.normalizedTheorem));

            #endregion

            #region Removing incorrect objects

            // It might happen that after all the black magic we did we now have objects of type f(x) where x is not
            // in its normal version anymore. We need to remove such objects. Let's take all objects
            _objectToItsNormalVersion.Keys
                // That are constructed
                .OfType<ConstructedConfigurationObject>()
                // Enumerate them so we can modify the dictionary
                .ToArray()
                // Handle each
                .ForEach(constructedObject =>
                {
                    // Find out if we should remove the current object. This is true if 
                    var shouldBeRemoved = constructedObject.PassedArguments.FlattenedList
                        // It has an argument that is not equal to its normal version
                        .Any(argumentObject => !argumentObject.Equals(_objectToItsNormalVersion.GetValueOrDefault(argumentObject)));

                    // If we should remove this object, do it
                    if (shouldBeRemoved)
                        _objectToItsNormalVersion.Remove(constructedObject);
                });

            #endregion

            #region Rebuilding objects by construction

            // We will do it in an easy way, first we clear it
            _objectsByConstruction.Clear();

            // And now we just add every object that is currently available to it
            _objectToItsNormalVersion.Keys.OfType<ConstructedConfigurationObject>().ForEach(MarkObjectByConstruction);

            #endregion

            #region Preparing equality result

            // Find the new objects that have been created / added by taking all objects
            var newObjects = _objectToItsNormalVersion.Keys
                // That are constructed
                .OfType<ConstructedConfigurationObject>()
                // And that were not among the old objects
                .Where(constructedObject => !oldObjectToItsNormalVersion.ContainsKey(constructedObject))
                // Enumerate
                .ToArray();

            // Find the removed objects by taking the old objects
            var removedObjects = oldObjectToItsNormalVersion.Keys
                // That are constructed
                .OfType<ConstructedConfigurationObject>()
                // And are no longer among the current objects
                .Where(constructedObject => !_objectToItsNormalVersion.ContainsKey(constructedObject))
                // Enumerate
                .ToArray();

            // Find the objects that are still there, but changed the normal version, by taking the old objects
            var changedObjects = oldObjectToItsNormalVersion.Keys
                // That are constructed
                .OfType<ConstructedConfigurationObject>()
                // Which are there a
                .Where(constructedObject => _objectToItsNormalVersion.ContainsKey(constructedObject)
                    // And that no longer have the same normal version
                    && !oldObjectToItsNormalVersion[constructedObject].Equals(_objectToItsNormalVersion[constructedObject]))
                // Enumerate
                .ToArray();

            // Set the equality result with the found data
            equalityResult = new NewEqualityResult(normalizedNewTheorems, dismissedTheorems, newObjects, removedObjects, changedObjects);

            #endregion

            #endregion
        }

        /// <summary>
        /// Marks that a given theorem, whose type is not <see cref="EqualObjects"/>, is proved, and performs its normalization if needed.
        /// If the theorem is new, it is geometrically validated so that we are sure we will not reach an incorrect state.
        /// </summary>
        /// <param name="provedTheorem">The theorem that has been established correct.</param>
        /// <param name="isNew">Indicates whether the theorem is new to the helper.</param>
        /// <param name="isValid">Indicates whether the theorem is geometrically valid.</param>
        /// <param name="normalizedTheorem">The normalized version of the proved theorem.</param>
        /// <param name="normalizationEqualities">The equalities needed to be true for the normalization to happen.</param>
        public void MarkProvedNonequality(Theorem provedTheorem, out bool isNew, out bool isValid, out Theorem normalizedTheorem, out Theorem[] normalizationEqualities)
        {
            // Ensure this is a non-equality
            if (provedTheorem.Type == EqualObjects)
                throw new TheoremProverException($"Invalid type of theorem, expected anything but {EqualObjects}.");

            // Remap the theorem to its normal form
            normalizedTheorem = provedTheorem.Remap(_objectToItsNormalVersion);

            // If it turns out to be null, then it means we have the theorem is degenerated 
            if (normalizedTheorem == null)
            {
                // It's new
                isNew = true;

                // But invalid
                isValid = false;

                // No normalization
                normalizationEqualities = null;

                // We're done
                return;
            }

            // If it's already there
            if (_provedTheorems.Contains(normalizedTheorem))
            {
                // It's not new
                isNew = false;

                // And it's valid
                isValid = true;

                // No normalization
                normalizationEqualities = null;

                // We're done
                return;
            }

            // If we got here, the theorem is new
            isNew = true;

            // If it's not geometrically correct
            if (!IsNewProvedTheoremGeometricallyCorrect(normalizedTheorem))
            {
                // It's not valid
                isValid = false;

                // No normalization
                normalizationEqualities = null;

                // We're done
                return;
            }

            // If we got here, the theorem is valid
            isValid = true;

            // Add the theorem to collections
            AddTheoremToCollections(normalizedTheorem);

            // Find the used equalities during the normalization
            normalizationEqualities = FindUsedEqualities(provedTheorem);
        }

        /// <summary>
        /// Introduces a new object to the geometric situation. This object must not be here explicitly, or even implicitly as
        /// an object equal to some other object. It also must have normalized arguments.
        /// </summary>
        /// <param name="objectToIntroduce">The object to be introduced.</param>
        public void IntroduceNewObject(ConstructedConfigurationObject objectToIntroduce)
        {
            // Make it normalized
            _objectToItsNormalVersion.Add(objectToIntroduce, objectToIntroduce);

            // Add it to the dictionary of introduced objects with the normal version equal to itself
            _introducedObjectsNormalVersions.Add(objectToIntroduce, objectToIntroduce);

            // Mark it in the representants . The order should be correct even now because the object 
            // should be constructible from the objects that are already here).
            _orderedNormalVersions.Add(objectToIntroduce);

            // Add it to the dictionary mapping constructions to objects
            MarkObjectByConstruction(objectToIntroduce);
        }

        /// <summary>
        /// Removes a given introduced object. If this object has turned out to be equal to objects of the original configuration,
        /// then nothing will happen. Otherwise this entails removing every object that has been constructed from it, every object
        /// equal to it, and every theorem that states something about all these removed objects.
        /// </summary>
        /// <param name="introducedObject">The introduced object to be removed.</param>
        /// <param name="allRemovedObjects">All objects that have been removed because they are dependent on the pass introduced object.</param>
        public void RemoveIntroducedObject(ConstructedConfigurationObject introducedObject, out ConstructedConfigurationObject[] allRemovedObjects)
        {
            // If the object has already been removed, don't do anything
            if (!_introducedObjectsNormalVersions.ContainsKey(introducedObject))
            {
                // No removed objects
                allRemovedObjects = Array.Empty<ConstructedConfigurationObject>();

                // We're done
                return;
            }

            // Otherwise find the normal version of the object about to be removed
            var normalVersion = _introducedObjectsNormalVersions[introducedObject];

            // If this is an object of the original configuration, don't remove it
            if (_pictures.Configuration.AllObjects.Contains(normalVersion))
            {
                // No removed objects
                allRemovedObjects = Array.Empty<ConstructedConfigurationObject>();

                // We're done
                return;
            }

            #region Finding normal versions to be removed

            // Prepare the set of all normal versions that are no longer valid, initially with the current one
            var normalVersionsToBeRemoved = new HashSet<ConfigurationObject> { normalVersion };

            // We will to find new normal versions to be removed
            while (true)
            {
                // Prepare a variable indicating whether there's been any new normal version to be removed
                var anyNewNormalVersionToBeRemoved = false;

                // Go through the normal versions 
                _orderedNormalVersions
                    // Take those that are not in the original configuration
                    .Where(normalVersion => !_pictures.Configuration.AllObjects.Contains(normalVersion)
                        // And those that are not removed already
                        && !normalVersionsToBeRemoved.Contains(normalVersion)
                        // And those that are constructed
                        && normalVersion is ConstructedConfigurationObject constructedNormalVersion
                        // And contain an argument that is to be removed
                        && constructedNormalVersion.PassedArguments.FlattenedList.Any(normalVersionsToBeRemoved.Contains))
                    // Handle each
                    .ForEach(normalVersion =>
                    {
                        // Add that it needs to be removed
                        normalVersionsToBeRemoved.Add(normalVersion);

                        // Mark that we have found something to be removed
                        anyNewNormalVersionToBeRemoved = true;
                    });

                // If there hasn't been any new change, we're done
                if (!anyNewNormalVersionToBeRemoved)
                    break;
            }

            #endregion

            // Remove normal versions from the ordered list
            _orderedNormalVersions.RemoveAll(normalVersionsToBeRemoved.Contains);

            #region Removing introduced objects

            // Go through the normal versions to be removed
            normalVersionsToBeRemoved.ForEach(normalVersionToBeRemoved =>
                // For each look for those introduced objects
                _introducedObjectsNormalVersions
                    // That have this normal version
                    .Where(pair => pair.Value.Equals(normalVersionToBeRemoved))
                    // Take these objects
                    .Select(pair => pair.Key)
                    // Enumerate so we can remove them
                    .ToArray()
                    // Remove them
                    .ForEach(introducedObject => _introducedObjectsNormalVersions.Remove(introducedObject)));

            #endregion

            #region Removing theorems that use normal versions to be removed

            // Go through the normal versions to be removed
            normalVersionsToBeRemoved.ForEach(normalVersionToBeRemoved =>
                // Get the theorems that hold for the current one
                _provedTheoremsByObject.GetValueOrDefault(normalVersionToBeRemoved)
                    // Enumerate so that we can modify 
                    ?.ToArray()
                    // Remove each
                    .ForEach(RemoveTheoremFromCollections));

            #endregion

            #region Finding any object to be removed

            // Find the objects to be removed by taking the normal versions
            allRemovedObjects = normalVersionsToBeRemoved
                // For each we will remove any object 
                .SelectMany(normalVersionToBeRemoved => _objectToItsNormalVersion
                    // Where the normal version is our object
                    .Where(pair => pair.Value.Equals(normalVersionToBeRemoved)
                        // Or the object is constructed
                        || pair.Key is ConstructedConfigurationObject constructedObject &&
                            // And the arguments contain it 
                            constructedObject.PassedArguments.FlattenedList.Contains(normalVersionToBeRemoved)))
                // Take the objects
                .Select(pair => pair.Key)
                // These objects are certainly constructed
                .Cast<ConstructedConfigurationObject>()
                // Enumerate
                .ToArray();

            #endregion

            #region Removing all objects to be removed

            // Remove all objects
            foreach (var objectToBeRemoved in allRemovedObjects)
            {
                // Make that the object has no normal version anymore
                _objectToItsNormalVersion.Remove(objectToBeRemoved);

                // Remove it from the dictionary of all objects
                _objectsByConstruction[objectToBeRemoved.Construction].Remove(objectToBeRemoved);
            }

            #endregion

            // Update the set of all removed objects
            _removedIntroducedObjects.Add(allRemovedObjects);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Adds a given object to the <see cref="_objectsByConstruction"/> dictionary.
        /// </summary>
        /// <param name="constructedObject">The constructed object to be categorized by its construction.</param>
        private void MarkObjectByConstruction(ConstructedConfigurationObject constructedObject)
            // Get or create and return a new set for the construction and add the given object to it
            => _objectsByConstruction.GetOrAdd(constructedObject.Construction, () => new HashSet<ConstructedConfigurationObject>()).Add(constructedObject);

        /// <summary>
        /// Adds a given theorem to all the collections related to proved theorems, i.e. <see cref="_provedTheorems"/>,
        /// <see cref="_provedTheoremsByType"/>, <see cref="_provedTheoremsByObject"/>. Also ensures it is no longer
        /// among <see cref="_theoremsToProve"/>.
        /// </summary>
        /// <param name="provedTheorem">The proved theorem to be added to the appropriate collections.</param>
        private void AddTheoremToCollections(Theorem provedTheorem)
        {
            // Add the theorem to the set of proved ones
            _provedTheorems.Add(provedTheorem);

            // Make sure it's not among those to be proved
            _theoremsToProve.Remove(provedTheorem);

            // Add the theorem to the dictionary of proved ones
            _provedTheoremsByType.GetOrAdd(provedTheorem.Type, () => new HashSet<Theorem>()).Add(provedTheorem);

            // Add the theorem to the dictionary mapping objects to theorems
            // Take the inner objects
            provedTheorem.GetInnerConfigurationObjects()
                // Register the theorem for each of them
                .ForEach(innerObject => _provedTheoremsByObject.GetOrAdd(innerObject, () => new HashSet<Theorem>()).Add(provedTheorem));
        }

        /// <summary>
        /// Removes a given theorem from all the collections related to proved theorems, i.e. <see cref="_provedTheorems"/>,
        /// <see cref="_provedTheoremsByType"/> and <see cref="_provedTheoremsByObject"/>.
        /// </summary>
        /// <param name="theoremToRemove">The theorem to be removed from the appropriate collections.</param>
        private void RemoveTheoremFromCollections(Theorem theoremToRemove)
        {
            // Remove the theorem from the set of proved ones
            _provedTheorems.Remove(theoremToRemove);

            // Remove the theorem from the dictionary by object
            _provedTheoremsByType[theoremToRemove.Type].Remove(theoremToRemove);

            // Remove the theorem from the dictionary mapping objects to theorems
            // Take the inner objects
            theoremToRemove.GetInnerConfigurationObjects()
                // Remove the theorem from each corresponding set
                .ForEach(innerObject => _provedTheoremsByObject[innerObject].Remove(theoremToRemove));
        }

        /// <summary>
        /// Finds equalities that are needed to be true in order to get the normalized version of the passed theorem.
        /// </summary>
        /// <param name="theoremBeforeNormalization">The theorem whose inner object are not in the normal form.</param>
        /// <returns>The equalities needed to get the normalized theorem from the passed one.</returns>
        private Theorem[] FindUsedEqualities(Theorem theoremBeforeNormalization)
            // Take the theorem's inner objects
            => theoremBeforeNormalization.GetInnerConfigurationObjects()
                // For each find its normal version
                .Select(innerObject => (innerObject, normalizedInnerObject: _objectToItsNormalVersion[innerObject]))
                // Take those pairs where there isn't a trivial equality
                .Where(pair => !pair.innerObject.Equals(pair.normalizedInnerObject))
                // Such pairs make a theorem needed for the normalization
                .Select(pair => new Theorem(EqualObjects, pair.innerObject, pair.normalizedInnerObject))
                // Enumerate
                .ToArray();

        /// <summary>
        /// Finds out if a new proved passed theorem is geometrically correct via <see cref="_verifier"/>.
        /// </summary>
        /// <param name="provedTheorem">The proved theorem that should be checked for correctness.</param>
        /// <returns>true, if the passed theorem is geometrically correct; false otherwise.</returns>
        private bool IsNewProvedTheoremGeometricallyCorrect(Theorem provedTheorem)
        {
            // We will not call the verifier right away. Theorems that use only objects of the original configuration
            // should all be there to prove (if they are not proven yet, but in that case, this method shouldn't get called)
            // In order to find out whether the theorem uses only objects of the configuration take the objects
            var doesTheoremUsesOnlyOriginalObjects = provedTheorem.GetInnerConfigurationObjects()
                // That are constructed
                .OfType<ConstructedConfigurationObject>()
                // Check if they are among the original objects
                .All(_pictures.Configuration.ConstructedObjectsSet.Contains);

            // If it uses only objects that are part of the original configuration,
            if (doesTheoremUsesOnlyOriginalObjects)
                // Then it is correct if and only if it is among theorems to be proved
                _theoremsToProve.Contains(provedTheorem);

            // Otherwise it contains a new object and we need to verify it numerically :/
            return _verifier.IsTrueInAllPictures(_pictures, provedTheorem);
        }

        #endregion
    }
}