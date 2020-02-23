using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.TheoremFinder
{
    /// <summary>
    /// A default implementation of <see cref="ITheoremFinder"/> that reuses <see cref="ITypedTheoremFinder"/>s.
    /// </summary>
    public class TheoremFinder : ITheoremFinder
    {
        #region Private fields

        /// <summary>
        /// The finders of theorems of concrete types.
        /// </summary>
        private readonly ITypedTheoremFinder[] _finders;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremFinder"/> class.
        /// </summary>
        /// <param name="finders">The finders of theorems of concrete types.</param>
        public TheoremFinder(ITypedTheoremFinder[] finders)
        {
            _finders = finders ?? throw new ArgumentNullException(nameof(finders));
        }

        #endregion

        #region ITheoremFinder implementation

        /// <summary>
        /// Finds all theorems that hold true in the configuration represented by a given contextual picture.
        /// </summary>
        /// <param name="contextualPicture">The contextual picture that represents the configuration.</param>
        /// <returns>The enumerable of true theorems in the configuration.</returns>
        public TheoremMap FindAllTheorems(ContextualPicture contextualPicture)
        {
            // Simply reuse all the finders and wrap the theorems in a map (which will enumerate it)
            return new TheoremMap(_finders.SelectMany(finder => finder.FindAllTheorems(contextualPicture)));
        }

        /// <summary>
        /// Finds all theorems that hold true in the configuration represented by a given contextual picture
        /// and in their statement use the last object of the configuration.
        /// </summary>
        /// <param name="contextualPicture">The contextual picture that represents the configuration.</param>
        /// <param name="oldTheorems">The theorems that hold true in the configuration without the last object.</param>
        /// <returns>The enumerable of true theorems in the configuration that use the last object.</returns>
        public TheoremMap FindNewTheorems(ContextualPicture contextualPicture, TheoremMap oldTheorems)
        {
            // Reuse all the finders to find the theorems that are geometrically new in the configuration
            var uniqueNewTheorems = _finders.SelectMany(finder => finder.FindNewTheorems(contextualPicture));

            // We still need to find the new theorems that are not new, due to geometric properties
            // such as collinearity or concyclity, but can now be stated using the new object
            // For that we are going to take every old theorem and return possibly new versions of it
            var redefinedNewTheorems = oldTheorems.AllObjects.SelectMany(theorem =>
                // If we have an incidence, we don't want to do anything 
                // (it's not needed to state that A lies on line AB)
                theorem.Type == TheoremType.Incidence ? Enumerable.Empty<Theorem>() :
                // Otherwise for each theorem we take its objects
                theorem.InvolvedObjects
                    // Find the possible definition changes for each
                    // (this includes the option of not changing the definition at all)
                    .Select(theoremObject => FindDefinitionChangeOptions(theoremObject, contextualPicture))
                    // Combine these definitions in every possible way
                    .Combine()
                    // For every option create a new theorem
                    .Select(objects => new Theorem(theorem.Type, objects)))
                // We might have gotten even old theorems (when all the definition option changes were 
                // 'no change'. Also we might have gotten duplicates. This call will solve both problems
                .Except(oldTheorems.AllObjects);

            // Concatenate these two types of theorems and wrap the result in a map (which will enumerate it)
            return new TheoremMap(uniqueNewTheorems.Concat(redefinedNewTheorems));
        }

        /// <summary>
        /// Finds all options for changing the definition of a given old theorem object based on the new (last)
        /// configuration object, which might have geometric properties related to this old object (for example,
        /// if the new object is a point and the old one is a line/circle, then it might lie on it). This includes 
        /// even an option of not changing the definition at all, i.e. returning the original old object. 
        /// </summary>
        /// <param name="oldTheoremObject">The old theorem object for which we're looking for definition change options.</param>
        /// <param name="contextualPicture">The contextual picture that represents the configuration.</param>
        /// <returns>The enumeration of all possible definition changes, including no change.</returns>
        private IEnumerable<TheoremObject> FindDefinitionChangeOptions(TheoremObject oldTheoremObject, ContextualPicture contextualPicture)
        {
            // Find the new object of the configuration
            var newConfigurationObject = contextualPicture.Pictures.Configuration.LastConstructedObject;

            // Find its geometric version
            var newGeometricObject = contextualPicture.GetGeometricObject(newConfigurationObject);

            // Switch based on the type of old object
            switch (oldTheoremObject)
            {
                // If we have a point or line segment (internally consisting of points...)
                case PointTheoremObject _:
                case LineSegmentTheoremObject _:

                    // Then there is no way to find a new definition of the object,
                    // i.e. we return the 'no change of definition' option
                    return new[] { oldTheoremObject };

                // If we have an angle
                case AngleTheoremObject angle:

                    // Recursively get the re-definitions of both the objects
                    return new[]
                    {
                        FindDefinitionChangeOptions(angle.Object1, contextualPicture),
                        FindDefinitionChangeOptions(angle.Object2, contextualPicture),
                    }
                    // Combine them into a single one in every possible way
                    // This will include the option of not changing the definition
                    .Combine()
                    // Each combination makes a possible answer
                    .Select(lines => new AngleTheoremObject((LineTheoremObject)lines[0], (LineTheoremObject)lines[1]));

                // If we have an object with points...
                case TheoremObjectWithPoints objectWithPoints:

                    #region Find its geometric version 

                    // We're going to find the corresponding geometric version
                    DefinableByPoints geometricObjectWithPoints = default;

                    // If our object is defined explicitly
                    if (objectWithPoints.DefinedByExplicitObject)
                        // Use the internal configuration object to get the definition directly    
                        geometricObjectWithPoints = (DefinableByPoints)contextualPicture.GetGeometricObject(objectWithPoints.ConfigurationObject);

                    // Otherwise it's defined by points
                    else
                    {
                        // Get the points corresponding to its points
                        var geometricPoints = objectWithPoints.Points
                            // For each find the geometric point
                            .Select(contextualPicture.GetGeometricObject)
                            // We know they're points
                            .Cast<PointObject>()
                            // Enumerate
                            .ToArray();

                        // Find the right object based on the type of the theorem object with points
                        geometricObjectWithPoints = (objectWithPoints switch
                        {
                            // Looking for the line that contains our points
                            // It certainly passes through the first point
                            LineTheoremObject _ => geometricPoints[0].Lines.First(line => line.ContainsAll(geometricPoints)) as DefinableByPoints,

                            // Looking for the circle that contains our points
                            // It certainly passes through the first point
                            CircleTheoremObject _ => geometricPoints[0].Circles.First(circle => circle.ContainsAll(geometricPoints)),

                            // Default case
                            _ => throw new TheoremFinderException($"Unhandled type of theorem object with points: {objectWithPoints.GetType()}")
                        });
                    }

                    #endregion

                    // Switch on the type of the last configuration object
                    switch (newConfigurationObject.ObjectType)
                    {
                        // If we have a point...
                        case ConfigurationObjectType.Point:

                            // Then the point must line on our line/circle, otherwise
                            // it cannot be used to find a new definition. If it doesn't...
                            if (!geometricObjectWithPoints.Points.Contains(newGeometricObject))
                                // Return the 'no change of definition' option
                                return new[] { oldTheoremObject };

                            // If it lies on it, we're going to alter the remaining  
                            // points in every possible well. Let's take all the points
                            return geometricObjectWithPoints.Points
                                // Pull their configuration object versions
                                .Select(point => point.ConfigurationObject)
                                // Take the points distinct from the new one
                                .Where(point => !point.Equals(newConfigurationObject))
                                // Alter other points in every possible way. If this is 
                                // a line, we need 1 other point, if a circle, then 2. 
                                .Subsets(geometricObjectWithPoints.NumberOfNeededPoints - 1)
                                // Each subset makes together with the new point a possible definition
                                .Select(points =>
                                {
                                    // Find the right constructor to use
                                    return objectWithPoints switch
                                    {
                                        // If we have a line, then we have only more point left to set (besides the new one)
                                        LineTheoremObject _ => new LineTheoremObject(points[0], newConfigurationObject) as TheoremObject,

                                        // If we have a circle, then we have two more points left to set (besides the new one)
                                        CircleTheoremObject _ => new CircleTheoremObject(points[0], points[1], newConfigurationObject),

                                        // Default case
                                        _ => throw new TheoremFinderException($"Unhandled type of theorem object with points: {objectWithPoints.GetType()}")
                                    };
                                })
                                // Append the no-change definition
                                .Concat(oldTheoremObject);

                        // If we have a line or circle...
                        case ConfigurationObjectType.Line:
                        case ConfigurationObjectType.Circle:

                            // The only way an explicit line or circle would redefine the object
                            // would be if they were the same. In that case the object must have
                            // been defined implicitly (otherwise it wouldn't be new). Therefore
                            // if they aren't the same, we don't have another definition.
                            if (geometricObjectWithPoints != newGeometricObject)
                                // Return the 'no change of definition' option
                                return new[] { oldTheoremObject };

                            // If they are the same, we can create the new possible object by
                            // defining it explicitly by the new object
                            var newDefinition = objectWithPoints switch
                            {
                                // Line case
                                LineTheoremObject _ => new LineTheoremObject(newConfigurationObject) as TheoremObject,

                                // Circle case
                                CircleTheoremObject _ => new CircleTheoremObject(newConfigurationObject),

                                // Default case
                                _ => throw new TheoremFinderException($"Unhandled type of theorem object with points: {objectWithPoints.GetType()}")
                            };

                            // Finally we can return our new definition, together
                            // with  the 'no change of definition' option
                            return new[] { oldTheoremObject, newDefinition };

                        // Default case
                        default:
                            throw new TheoremFinderException($"Unhandled type of configuration object: {newConfigurationObject.ObjectType}");
                    }

                // Default case
                default:
                    throw new TheoremFinderException($"Unhandled type of theorem object: {oldTheoremObject.GetType()}");
            }
        }

        #endregion
    }
}