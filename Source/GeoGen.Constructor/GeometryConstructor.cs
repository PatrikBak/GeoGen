using GeoGen.AnalyticGeometry;
using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Constructor
{
    /// <summary>
    /// The default implementation of <see cref="IGeometryConstructor"/>. 
    /// </summary>
    public class GeometryConstructor : IGeometryConstructor
    {
        #region Dependencies

        /// <summary>
        /// The resolver of object constructors for particular constructions.
        /// </summary>
        private readonly IConstructorsResolver _resolver;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GeometryConstructor"/> class.
        /// </summary>
        /// <param name="resolver">The resolver of object constructors for particular constructions.</param>
        public GeometryConstructor(IConstructorsResolver resolver)
        {
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
        }

        #endregion

        #region IGeometryConstructor implementation

        /// <summary>
        /// Constructs a given <see cref="Configuration"/> to a given number of pictures.
        /// Throws an <see cref="InconsistentPicturesException"/> if the construction couldn't be carried out consistently.
        /// </summary>
        /// <param name="configuration">The configuration to be constructed.</param>
        /// <param name="numberOfPictures">The number of <see cref="Picture"/>s where the configuration should be drawn.</param>
        /// <returns>The tuple consisting of the pictures and the construction data.</returns>
        public (PicturesOfConfiguration pictures, ConstructionData data) Construct(Configuration configuration, int numberOfPictures)
        {
            // Create pictures for the configuration
            var pictures = new PicturesOfConfiguration(configuration, numberOfPictures);

            // First we add loose objects to all pictures
            foreach (var picture in pictures)
            {
                // Construct the loose object for the picture
                var constructedLooseObjects = Construct(configuration.LooseObjectsHolder.Layout);

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

        /// <summary>
        /// Constructs a given <see cref="Configuration"/> using an already constructed old one.
        /// It is assumed that the new configuration differs only by the last object from the already 
        /// constructed one. Thus only the last object is constructed. Throws an
        /// <see cref="InconsistentPicturesException"/> if the construction couldn't be carried out.
        /// </summary>
        /// <param name="oldConfigurationPictures">The pictures where the old configuration is drawn.</param>
        /// <param name="newConfiguration">The new configuration that should be drawn.</param>
        /// <returns>The tuple consisting of the pictures and the construction data.</returns>
        public (PicturesOfConfiguration pictures, ConstructionData data) ConstructByCloning(PicturesOfConfiguration oldConfigurationPictures, Configuration newConfiguration)
        {
            // Clone the pictures
            var pictures = oldConfigurationPictures.Clone(newConfiguration);

            // Return them with the construction data for the new object
            return (pictures, Construct(pictures, newConfiguration.LastConstructedObject, addToPictures: true));
        }

        /// <summary>
        /// Constructs a given <see cref="ConstructedConfigurationObject"/>. It is assumed that the constructed 
        /// object can be construed in each of the passed pictures using its objects or its remembered duplicates.
        /// Throws an <see cref="InconsistentPicturesException"/> if the construction couldn't be carried out.
        /// </summary>
        /// <param name="pictures">The pictures that should contain the input for the construction.</param>
        /// <param name="constructedObject">The object that is about to be constructed.</param>
        /// <param name="addToPictures">Indicates if we should add the object to the pictures.</param>
        /// <returns>The construction data.</returns>
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
                    var equalObjects = new[] { duplicate, equalObject }.Where(o => o != null).ToArray();

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

        /// <summary>
        /// Constructs a given <see cref="ConstructedConfigurationObject"/> without adding it to the pictures.
        /// It is assumed that the constructed object can be construed in the passed pictures. The fact whether
        /// the object is or is not already present in individual pictures is ignored. If the object is 
        /// inconstructible, null is returned. Throws an <see cref="InconsistentPicturesException"/> if the 
        /// construction couldn't be carried out.
        /// </summary>
        /// <param name="pictures">The pictures that should contain the input for the construction.</param>
        /// <param name="constructedObject">The object that is about to be constructed.</param>
        /// <returns>The dictionary mapping pictures to constructed objects, or null; if the object is inconstructible.</returns>
        public IReadOnlyDictionary<Picture, IAnalyticObject> Construct(Pictures pictures, ConstructedConfigurationObject constructedObject)
        {
            // Prepare the result
            var result = new Dictionary<Picture, IAnalyticObject>();

            // Initialize a variable indicating if the construction is possible
            bool canBeConstructed = default;

            // Let the resolver find the constructor and let it create the constructor function
            var constructorFunction = _resolver.Resolve(constructedObject.Construction).Construct(constructedObject);

            // Construct it in every picture
            foreach (var picture in pictures)
            {
                // Perform the construction
                var analyticObject = constructorFunction(picture);

                // Find out if it's been constructed
                var objectConstructed = analyticObject != null;

                // We need to first check if some other picture didn't mark constructibility in the opposite way
                // If yes, we have an inconsistency
                if (picture != pictures.First() && canBeConstructed != objectConstructed)
                    throw new InconsistentConstructibilityException(constructedObject);

                // Mark the construction result
                canBeConstructed = objectConstructed;

                // If the object can be constructed, add it to the result
                result.Add(picture, analyticObject);
            }

            // If the object can be constructed, return the result, otherwise null
            return canBeConstructed ? result : null;
        }

        /// <summary>
        /// Constructs a given <see cref="ConstructedConfigurationObject"/> without adding it to the picture.
        /// It is assumed that the constructed object can be constructed in the passed picture. The fact whether
        /// the object is or is not already present in individual pictures is ignored. If the object is 
        /// inconstructible, null is returned. 
        /// </summary>
        /// <param name="picture">The picture that should contain the input for the construction.</param>
        /// <param name="constructedObject">The object that is about to be constructed.</param>
        /// <param name="addToPicture">Indicates if we should add the object to the picture.</param>
        /// <returns>The constructed object or null; if the object is inconstructible.</returns>
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
                picture.MarkDuplicate(equalObject, constructedObject);
            }

            // Return the object
            return analyticObject;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Constructs analytic objects having a given layout.
        /// </summary>
        /// <param name="layout">The layout of loose objects.</param>
        /// <returns>The constructed analytic objects.</returns>
        private IAnalyticObject[] Construct(LooseObjectsLayout layout)
        {
            switch (layout)
            {
                // With two points all options are equivalent
                case LooseObjectsLayout.LineSegment:
                    return new IAnalyticObject[] { new Point(0, 0), new Point(1, 0) };

                // With three points we'll create an acute scalene triangle
                case LooseObjectsLayout.Triangle:
                {
                    // Create the points
                    var (point1, point2, point3) = AnalyticHelpers.ConstructRandomScaleneAcuteTriangle();

                    // Return them in an array 
                    return new IAnalyticObject[] { point1, point2, point3 };
                }

                // In quadrilateral case we will a convex one
                case LooseObjectsLayout.Quadrilateral:
                {
                    // Create the points
                    var (point1, point2, point3, point4) = AnalyticHelpers.ConstructRandomConvexQuadrilateral();

                    // Return them in an array 
                    return new IAnalyticObject[] { point1, point2, point3, point4 };
                }

                // In cyclic quadrilateral case we will a convex one
                case LooseObjectsLayout.CyclicQuadrilateral:
                {
                    // Create the points
                    var (point1, point2, point3, point4) = AnalyticHelpers.ConstructRandomCyclicConvexQuadrilateral();

                    // Return them in an array 
                    return new IAnalyticObject[] { point1, point2, point3, point4 };
                }

                // In this case the line is fixed and the point is arbitrary
                case LooseObjectsLayout.ExplicitLineAndPoint:
                {
                    // Create the points
                    var (point, line) = AnalyticHelpers.ConstructLineAndRandomPointNotLyingOnIt();

                    // Return them in an array 
                    return new IAnalyticObject[] { point, line };
                }

                // In this case the line is fixed and points are arbitrary
                case LooseObjectsLayout.ExplicitLineAndTwoPoints:
                {
                    // Create the points
                    var (line, point1, point2) = AnalyticHelpers.ConstructLineAndTwoRandomPointsNotLyingOnIt();

                    // Return them in an array 
                    return new IAnalyticObject[] { line, point1, point2 };
                }

                // In this case we have three points
                case LooseObjectsLayout.RightTriangle:
                {
                    // Create the points
                    var (point1, point2, point3) = AnalyticHelpers.ConstructRandomRightTriangle();

                    // Return them in an array 
                    return new IAnalyticObject[] { point1, point2, point3 };
                }

                // Unhandled cases
                default:
                    throw new ConstructorException($"Unhandled value of {nameof(LooseObjectsLayout)}: {layout}.");
            }
        }

        #endregion
    }
}