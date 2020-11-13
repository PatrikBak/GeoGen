using GeoGen.AnalyticGeometry;
using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.TheoremRanker;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.TheoremSorter
{
    /// <summary>
    /// The default implementation of <see cref="ITheoremSorter"/> that tracks a specified number of theorems.
    /// The main feature of this implementation is the ability to recognize geometrically equal theorems. This is done
    /// numerically via <see cref="IGeometryConstructor"/>. If there are two geometrically equal theorems, then the
    /// following resolution algorithm is used:
    /// <list type="number">
    /// <item>If there is a theorem with a smaller number of objects, then this one is picked.</item>
    /// <item>If there is a theorem with a smaller number of points, then this one is picked.</item>
    /// <item>If there is a theorem with a higher ranking, then this one is picked.</item>
    /// <item>Otherwise there is a tie which is resolved by taking the older one.</item>
    /// </list>
    /// While constructing theorems configuration isomorphism is taken into account. Therefore for example the fact that
    /// the A-midline of triangle ABC is parallel to BC is the same as the B-midline being parallel to AC.
    /// </summary>
    public class TheoremSorter : ITheoremSorter
    {
        #region Dependencies

        /// <summary>
        /// The constructor used to geometrically compare theorems.
        /// </summary>
        private readonly IGeometryConstructor _constructor;

        /// <summary>
        /// The sorter of geometry failures for object and theorem construction.
        /// </summary>
        private readonly ISortingGeometryFailureTracer _tracer;

        #endregion

        #region Private fields

        /// <summary>
        /// The dictionary mapping pictures to loose object holders that are drawn in them.
        /// </summary>
        private readonly Dictionary<Picture, LooseObjectHolder> _looseObjectHolders = new Dictionary<Picture, LooseObjectHolder>();

        /// <summary>
        /// The dictionary mapping loose object layouts to pictures where a loose object holder with the given layout is drawn.
        /// </summary>
        private readonly Dictionary<LooseObjectLayout, Picture> _pictures = new Dictionary<LooseObjectLayout, Picture>();

        /// <summary>
        /// The map mapping picture-analytic theorem pairs to actual ranked theorems that are currently best.
        /// </summary>
        private readonly Map<(Picture, AnalyticTheorem), RankedTheorem> _drawnTheorems = new Map<(Picture, AnalyticTheorem), RankedTheorem>();

        /// <summary>
        /// The ladder of best theorems sorted by the ranking.
        /// </summary>
        private readonly RankingLadder<RankedTheorem, TheoremRanking> _ladder;

        #endregion

        #region ITheoremSorter properties

        /// <inheritdoc/>
        public IEnumerable<RankedTheorem> BestTheorems => _ladder.Select(pair => pair.item);

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremSorter"/> class.
        /// </summary>
        /// <param name="constructor">The constructor used to geometrically compare theorems.</param>
        /// <param name="tracer">The tracer of geometry failure.</param>
        /// <param name="numberOfTheorems">The maximal number of theorems that will be tracked.</param>
        public TheoremSorter(IGeometryConstructor constructor, ISortingGeometryFailureTracer tracer, int numberOfTheorems)
        {
            _constructor = constructor ?? throw new ArgumentNullException(nameof(constructor));
            _tracer = tracer ?? throw new ArgumentNullException(nameof(tracer));

            // Ensure the number of theorems is positive
            if (numberOfTheorems <= 0)
                throw new ArgumentOutOfRangeException(nameof(numberOfTheorems), "The maximal number of theorem to be tracked must be at least 1");

            // Initialize the ladder with the requested capacity
            _ladder = new RankingLadder<RankedTheorem, TheoremRanking>(capacity: numberOfTheorems);
        }

        #endregion

        #region ITheoremSorter methods

        /// <inheritdoc/>
        public void AddTheorems(IEnumerable<RankedTheorem> rankedTheorems, out bool bestTheoremsChanged)
        {
            // Set the that the best theorems hasn't initially changed
            bestTheoremsChanged = false;

            // Handle all the theorems
            foreach (var rankedTheorem in rankedTheorems)
            {
                // Find out if the ladder has a place for an object with this ranking. If not, we can ignore this
                if (!_ladder.IsTherePlaceFor(rankedTheorem.Ranking))
                    continue;

                // Otherwise find the picture 
                var picture = EnsurePictureIsCreated(rankedTheorem.Configuration.LooseObjectsHolder.Layout);

                // And geometrically equal theorem in it
                var (equalRankedTheorem, analyticTheorem) = FindGeometricallyEqualTheorem(picture, rankedTheorem);

                // If it didn't work out, which should be extremely rare, then we better skip this
                if (analyticTheorem == null)
                    continue;

                // Find out if there is a geometrically equal theorem or it is just the passed one
                var isThereGeometricallyEqualTheorem = equalRankedTheorem != rankedTheorem;

                // If the theorem is not new
                if (isThereGeometricallyEqualTheorem)
                {
                    // If we're not replacing, we can move on
                    if (!ShouldWeReplace(equalRankedTheorem, rankedTheorem))
                        continue;

                    // Otherwise remove the theorem
                    _drawnTheorems.RemoveLeft((picture, analyticTheorem));

                    // Add the new one
                    _drawnTheorems.Add((picture, analyticTheorem), rankedTheorem);

                    // Remove the old theorem from the ladder
                    _ladder.Remove(equalRankedTheorem, equalRankedTheorem.Ranking);
                }
                // Otherwise the theorem is new and we should add it to drawn ones
                else
                    _drawnTheorems.Add((picture, analyticTheorem), rankedTheorem);

                // Add the new theorem to the ladder. We already know there is place for it
                _ladder.Add(rankedTheorem, rankedTheorem.Ranking, out var removedTheorem);

                // If something has been removed, remove it here as well
                if (removedTheorem != null)
                    _drawnTheorems.RemoveRight(removedTheorem);

                // Finally set that the content has changed 
                bestTheoremsChanged = true;
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Finds out if we should replace the current theorem that is among <see cref="BestTheorems"/>
        /// with a geometrically equal passed theorem.
        /// </summary>
        /// <param name="currentTheorem">The theorem that is currently among <see cref="BestTheorems"/>.</param>
        /// <param name="newTheorem">A geometrically equal theorem that could be a better version of it.</param>
        /// <returns>true, if we should replace the current theorem; false otherwise.</returns>
        private static bool ShouldWeReplace(RankedTheorem currentTheorem, RankedTheorem newTheorem)
        {
            #region Number of objects

            // Find the counts
            var currentTheoremObjectCount = currentTheorem.Configuration.AllObjects.Count;
            var newTheoremObjectCount = newTheorem.Configuration.AllObjects.Count;

            // If the current one has more objects, then yes
            if (currentTheoremObjectCount > newTheoremObjectCount)
                return true;

            // If the current one has fewer objects, then no
            if (currentTheoremObjectCount < newTheoremObjectCount)
                return false;

            #endregion

            #region Number of points

            // Find the counts
            var currentTheoremPointCount = currentTheorem.Configuration.ObjectMap.GetObjectsForKeys(ConfigurationObjectType.Point).Count();
            var newTheoremPointCount = newTheorem.Configuration.ObjectMap.GetObjectsForKeys(ConfigurationObjectType.Point).Count();

            // If the current one has more points, then yes
            if (currentTheoremPointCount > newTheoremPointCount)
                return true;

            // If the current one has fewer points, then no
            if (currentTheoremPointCount < newTheoremPointCount)
                return false;

            #endregion

            #region Ranking

            // Find the rankings
            var currentTheoremRanking = currentTheorem.Ranking.TotalRanking.Rounded();
            var newTheoremRanking = newTheorem.Ranking.TotalRanking.Rounded();

            // If the current theorem has a smaller ranking, then yes
            if (currentTheoremRanking < newTheoremRanking)
                return true;

            // If the current theorem has a higher ranking, then no
            if (currentTheoremRanking > newTheoremRanking)
                return false;

            #endregion

            // Otherwise we have an unresolvable tie. It's better to say we don't need any replacement as it involves less work
            return false;
        }

        /// <summary>
        /// Ensures that a given layout has been drawn to a picture that can be retrieved from <see cref="_pictures"/>.
        /// It also makes sure the corresponding <see cref="_looseObjectHolders"/> is created and stored.
        /// </summary>
        /// <param name="layout">The loose object layout that should be drawn in the picture.</param>
        /// <returns>The picture associated with the layout.</returns>
        private Picture EnsurePictureIsCreated(LooseObjectLayout layout)
        {
            // Try to find it in the picture dictionary
            _pictures.TryGetValue(layout, out var picture);

            // If it's there, we're done
            if (picture != null)
                return picture;

            // Create a holder that has this layout by creating loose objects of the needed types
            var holder = new LooseObjectHolder(layout.ObjectTypes().Select(type => new LooseConfigurationObject(type)), layout);

            // Otherwise we will use the loose object holder from the configuration to simulate constructions of it
            var plainConfiguration = new Configuration(holder, Array.Empty<ConstructedConfigurationObject>());

            // We can construct it now in 1 picture and generation style
            picture = _constructor.ConstructWithUniformLayout(plainConfiguration, numberOfPictures: 1)
                // And take the picture
                .pictures.First();

            // Now we can associate it with the layout
            _pictures.Add(layout, picture);

            // And also remember the used loose object holder
            _looseObjectHolders.Add(picture, holder);

            // Finally we can return the result
            return picture;
        }

        /// <summary>
        /// Finds a geometrically equal theorem with respect to a given picture. This method tries to find 
        /// a different ranked theorem. If it cannot do so, then the result will be the passed theorem
        /// together with its analytic version.
        /// </summary>
        /// <param name="picture">The picture to be used for construction of inner theorem objects.</param>
        /// <param name="rankedTheorem">The ranked theorem to be examined.</param>
        /// <returns>
        /// The pair of ranked theorem and analytic theorem. If there is no distinct ranked theorem, then the theorem will
        /// be the passed one. If the examination fails, then the default value of the returning type will be returned.
        /// </returns>
        private (RankedTheorem, AnalyticTheorem) FindGeometricallyEqualTheorem(Picture picture, RankedTheorem rankedTheorem)
        {
            // Prepare the set of temporary objects
            var temporaryObjects = new HashSet<ConfigurationObject>();

            // Prepare the set of constructed analytic theorems
            var analyticTheorems = new HashSet<AnalyticTheorem>();

            // Prepare the variable that will hold the analytic theorem corresponding to the current one
            AnalyticTheorem analyticPassedTheorem = null;

            // Go through the isomorphic mappings of the current loose objects
            foreach (var looseObjectMapping in _looseObjectHolders[picture].GetIsomorphicMappings())
            {
                // Find out if this is an identity
                var isThisIdentity = looseObjectMapping.All(pair => pair.Key == pair.Value);

                // In order to remap the theorem we need to remap the loose its configuration
                var externalLooseObjectMapping = rankedTheorem.Configuration.LooseObjects
                   // With the 'internal' loose objects used in the constructed picture
                   .Zip(_looseObjectHolders[picture].LooseObjects)
                   // Use the current mapping among the 'internal' loose objects
                   .Select(pair => (pair.First, Second: looseObjectMapping[pair.Second]))
                   // To get the final result that is zipped to a dictionary
                   .ToDictionary(pair => pair.First, pair => pair.Second);

                // To map theorems we need to have all objects mapped
                var objectMapping = new Dictionary<ConfigurationObject, ConfigurationObject>();

                // Prepare the list of objects that we will find
                var remappedConstructedObjects = new List<ConstructedConfigurationObject>();

                // Add the mapped loose objects to the object mapping
                externalLooseObjectMapping.ForEach(pair => objectMapping.Add(pair.Key, pair.Value));

                // Remap all constructed objects 
                rankedTheorem.Configuration.ConstructedObjects.ForEach(constructedObject =>
                {
                    // Remap the current one
                    var remappedConstructObject = (ConstructedConfigurationObject)constructedObject.Remap(externalLooseObjectMapping);

                    // Add it to the mapping
                    objectMapping.Add(constructedObject, remappedConstructObject);

                    // Add it to the list of remapped constructed objects
                    remappedConstructedObjects.Add(remappedConstructObject);
                });

                // Mark these objects as temporary ones
                temporaryObjects.Add(remappedConstructedObjects);

                // Ensure the remapped objects are constructed
                EnsureObjectsAreConstructed(picture, remappedConstructedObjects, out var inconstructibleObject);

                // If they can't be, then it is really weird
                if (inconstructibleObject != null)
                {
                    // Trace it
                    _tracer.TraceInconstructibleObject(rankedTheorem);

                    // And return the default value indicating a failure
                    return default;
                }

                // Now we can finally remap the theorem
                var remappedTheorem = rankedTheorem.Theorem.Remap(objectMapping);

                try
                {
                    // And construct the analytic theorem
                    var remappedAnalyticTheorem = new AnalyticTheorem(remappedTheorem, picture);

                    // If we have an identity mapping, set that the current remapped analytic theorem corresponds to it
                    if (isThisIdentity)
                        analyticPassedTheorem = remappedAnalyticTheorem;

                    // Add it to the set of all analytic theorems
                    analyticTheorems.Add(remappedAnalyticTheorem);
                }
                catch (AnalyticException e)
                {
                    // If something weird happens, which really shouldn't, then trace it
                    _tracer.TraceInconstructibleTheorem(rankedTheorem, e);

                    // And return the default value indicating a failure
                    return default;
                }
            }

            // Remove the temporary objects from the picture
            temporaryObjects.ForEach(picture.Remove);

            // Find the one that is already there
            var equalAnalyticTheorem = analyticTheorems.FirstOrDefault(analyticTheorem => _drawnTheorems.ContainsLeftKey((picture, analyticTheorem)));

            // If there is some
            return equalAnalyticTheorem != null ?
                // Then return the corresponding ranked theorem together with it
                (_drawnTheorems.GetRightValue((picture, equalAnalyticTheorem)), equalAnalyticTheorem)
                // Otherwise return the passed theorem together with its analytic version
                : (rankedTheorem, analyticPassedTheorem);
        }

        /// <summary>
        /// Ensures that passed constructed objects are constructed in a given picture. The objects should be passed
        /// in a constructible order.
        /// </summary>
        /// <param name="picture">The picture where the objects should be constructed.</param>
        /// <param name="objects">The objects that needs to be reconstructed in the picture.</param>
        /// <param name="inconstructibleObject">The first object that couldn't be constructed. If all have been constructed, then null.</param>
        private void EnsureObjectsAreConstructed(Picture picture, IEnumerable<ConstructedConfigurationObject> objects, out ConstructedConfigurationObject inconstructibleObject)
        {
            // Go through the objects 
            foreach (var constructedObject in objects)
            {
                // If the current one is constructed, we can move on
                if (picture.Contains(constructedObject))
                    continue;

                // Otherwise construct it with auto-adding to the picture
                var result = _constructor.Construct(picture, constructedObject, addToPicture: true);

                // If the construction couldn't be carried out
                if (result == null)
                {
                    // Set the inconstructible object
                    inconstructibleObject = constructedObject;

                    // We can't continue because other objects might be dependent on this one
                    return;
                }
            }

            // If we got here, everything went fine
            inconstructibleObject = null;
        }

        #endregion
    }
}