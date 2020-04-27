using GeoGen.AnalyticGeometry;
using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Linq;

namespace GeoGen.Constructor
{
    /// <summary>
    /// The default implementation of <see cref="IGeometryConstructor"/> that uses <see cref="IConstructorResolver"/>
    /// to find the right constructors for particular constructions.
    /// </summary>
    public class GeometryConstructor : IGeometryConstructor
    {
        #region Dependencies

        /// <summary>
        /// The resolver of object constructors for particular constructions.
        /// </summary>
        private readonly IConstructorResolver _resolver;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GeometryConstructor"/> class.
        /// </summary>
        /// <param name="resolver">The resolver of object constructors for particular constructions.</param>
        public GeometryConstructor(IConstructorResolver resolver)
        {
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
        }

        #endregion

        #region IGeometryConstructor implementation

        /// <inheritdoc/>
        public (PicturesOfConfiguration pictures, ConstructionData data) Construct(Configuration configuration, int numberOfPictures, Func<IAnalyticObject[]> looseObjectConstructor)
        {
            // Create pictures for the configuration
            var pictures = new PicturesOfConfiguration(configuration, numberOfPictures);

            // First we add loose objects to all pictures
            foreach (var picture in pictures)
            {
                // Construct the loose object for the picture
                var constructedLooseObjects = looseObjectConstructor();

                // Add them one by one
                configuration.LooseObjects.Zip(constructedLooseObjects).ForEach(pair =>
                {
                    // Safely try to add them
                    picture.TryAdd(pair.First, pair.Second, out var equalObject);

                    // If there is an equal object, then we have a weird problem
                    if (equalObject != default)
                        throw new ConstructorException("Construction of loose objects yielded equal objects! This must be a bug...");
                });
            }

            // Then we add all constructed object
            foreach (var constructedObject in configuration.ConstructedObjects)
            {
                // Construct the object using another method
                var data = Construct(pictures, constructedObject, addToPictures: true);

                // Find out if the result is correct
                var correctResult = data.InconstructibleObject == default && data.Duplicates == default;

                // If it's not, we directly return the current data without dealing with the remaining objects
                if (!correctResult)
                    return (null, data);
            }

            // If we got here, then there are no inconstructible objects and no duplicates
            return (pictures, new ConstructionData(default, default));
        }

        /// <inheritdoc/>
        public (PicturesOfConfiguration pictures, ConstructionData data) ConstructByCloning(PicturesOfConfiguration oldConfigurationPictures, Configuration newConfiguration)
        {
            // Clone the pictures
            var pictures = oldConfigurationPictures.Clone(newConfiguration);

            // Return them with the construction data for the new object
            return (pictures, Construct(pictures, newConfiguration.LastConstructedObject, addToPictures: true));
        }

        /// <inheritdoc/>
        public ConstructionData Construct(Pictures pictures, ConstructedConfigurationObject constructedObject, bool addToPictures)
        {
            // Local function that restores all the pictures if there was an inconsistency
            void Restore()
            {
                // If the pictures aren't manipulated, don't do anything
                if (!addToPictures)
                    return;

                // Otherwise make sure the object is remove in all the pictures
                pictures.ForEach(picture => picture.Remove(constructedObject));
            }

            // Initialize a variable indicating if the construction is possible
            bool canBeConstructed = default;

            // Initialize a variable holding a potential duplicate version of the object
            ConfigurationObject duplicate = default;

            // Let the resolver find the constructor and let it create the constructor function
            var constructorFunction = _resolver.Resolve(constructedObject.Construction).Construct(constructedObject);

            // Construct it in every picture
            foreach (var picture in pictures)
            {
                // Prepare value indicating whether the object was constructed in the picture
                var objectConstructed = default(bool);

                // Prepare value holding a potential equal object in the picture to this object
                var equalObject = default(ConfigurationObject);

                // Construct it
                var analyticObject = constructorFunction(picture);

                // Set if it the construction went fine
                objectConstructed = analyticObject != null;

                // If the object is constructed...
                if (objectConstructed)
                {
                    // And we are supposed to add it to the pictures
                    if (addToPictures)
                    {
                        // Do it and let the picture determine an equal object
                        picture.TryAdd(constructedObject, analyticObject, out equalObject);
                    }
                    // Otherwise we find an equal object manually
                    else
                    {
                        // Set if there is an equal object
                        equalObject = picture.Contains(analyticObject) ? picture.Get(analyticObject) : null;
                    }
                }

                // We need to first check if some other picture didn't mark constructibility in the opposite way
                // If yes, we have an inconsistency
                if (picture != pictures.First() && canBeConstructed != objectConstructed)
                {
                    // Make sure the pictures are restored
                    Restore();

                    // Throw an exception
                    throw new InconsistentConstructibilityException(constructedObject);
                }

                // Now we need to check if some other picture didn't find a different duplicate 
                // If yes, we have an inconsistency
                if (picture != pictures.First() && duplicate != equalObject)
                {
                    // Make sure the pictures are restored
                    Restore();

                    // Get the not-null equal objects
                    var equalObjects = new[] { duplicate, equalObject }.Where(_equalObject => _equalObject != null).ToArray();

                    // Throw an exception
                    throw new InconsistentEqualityException(constructedObject, equalObjects);
                }

                // If there is an equal object and we could manipulate the picture, mark the equality
                if (equalObject != null && addToPictures)
                    picture.MarkDuplicate(equalObject, constructedObject);

                // Set the found values
                canBeConstructed = objectConstructed;
                duplicate = equalObject;
            }

            //  Now the object is handled with respect to all the pictures
            return new ConstructionData
            (
                // Set the inconstructible object to the given one, if it can't be constructed
                inconstructibleObject: !canBeConstructed ? constructedObject : default,

                // Set the duplicates to the pair of this object and the found duplicate, if there's any
                duplicates: duplicate != null ? (olderObject: duplicate, newerObject: constructedObject) : default
            );
        }

        /// <inheritdoc/>
        public IAnalyticObject Construct(Picture picture, ConstructedConfigurationObject constructedObject, bool addToPicture)
        {
            // Let the resolver find the constructor and let it create the constructor function
            var constructorFunction = _resolver.Resolve(constructedObject.Construction).Construct(constructedObject);

            // Perform the construction
            var analyticObject = constructorFunction(picture);

            // If the object couldn't be constructed, return null
            if (analyticObject == null)
                return null;

            // Otherwise the object is fine
            // If we are supposed to add it to the pictures...
            if (addToPicture)
            {
                // Then let's do it
                picture.TryAdd(constructedObject, analyticObject, out var equalObject);

                // If there was an equal object, then we just mark the duplicate
                if (equalObject != null)
                    picture.MarkDuplicate(equalObject, constructedObject);
            }

            // Return the object
            return analyticObject;
        }

        #endregion
    }
}