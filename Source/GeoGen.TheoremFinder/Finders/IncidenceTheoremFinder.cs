using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;
using static GeoGen.Core.ConfigurationObjectType;

namespace GeoGen.TheoremFinder
{
    /// <summary>
    /// A <see cref="ITypedTheoremFinder"/> for <see cref="TheoremType.Incidence"/>.
    /// </summary>
    public class IncidenceTheoremFinder : TheoremFinderBase
    {
        /// <summary>
        /// Finds all theorems of the sought type that hold true in the configuration 
        /// represented by a given contextual picture.
        /// </summary>
        /// <param name="contextualPicture">The contextual picture that represents the configuration.</param>
        /// <returns>The enumerable of true theorems of the sought type in the configuration.</returns>       
        public override IEnumerable<Theorem> FindAllTheorems(ContextualPicture contextualPicture)
        {
            // Take all objects
            return contextualPicture.Pictures.Configuration.AllObjects
                // That are explicit lines / circles
                .Where(configurationObject => configurationObject.ObjectType == Line || configurationObject.ObjectType == Circle)
                // For each found the corresponding geometric object
                .Select(contextualPicture.GetGeometricObject)
                // We know they have points
                .Cast<DefinableByPoints>()
                // We can now access its points
                .SelectMany(geometricObject => geometricObject.Points
                    // Every one of them makes an incidence
                    .Select(point => new Theorem(Type, geometricObject.ConfigurationObject, point.ConfigurationObject)));
        }

        /// <summary>
        /// Finds all theorems of the sought type that hold true in the configuration 
        /// represented by a given contextual picture and in their statement use the
        /// last object of the configuration, while there is no geometrically distinct
        /// way to state them without this last object.
        /// </summary>
        /// <param name="contextualPicture">The contextual picture that represents the configuration.</param>
        /// <returns>The enumerable of true theorems of the sought type in the configuration that need the last object.</returns>   
        public override IEnumerable<Theorem> FindNewTheorems(ContextualPicture contextualPicture)
        {
            // Get the last object of the configuration
            var lastConfigurationObject = contextualPicture.Pictures.Configuration.LastConstructedObject;

            // Distinguish cases bases on its type
            switch (lastConfigurationObject.ObjectType)
            {
                // If we have a point
                case Point:

                    // We find its geometric version
                    var geometricPoint = (PointObject)contextualPicture.GetGeometricObject(lastConfigurationObject);

                    // Take all circles and lines
                    return geometricPoint.Lines.Cast<DefinableByPoints>().Concat(geometricPoint.Circles)
                        // That are defined explicitly
                        .Where(lineCircle => lineCircle.ConfigurationObject != null)
                        // Each makes a valid incidence
                        .Select(lineCircle => new Theorem(Type, lineCircle.ConfigurationObject, lastConfigurationObject));

                // If we have a line or circle
                case Line:
                case Circle:

                    // We find its geometric version
                    var geometricLineCircle = (DefinableByPoints)contextualPicture.GetGeometricObject(lastConfigurationObject);

                    // Take its points
                    return geometricLineCircle.Points
                        // Each makes a valid incidence theorem
                        .Select(point => new Theorem(Type, point.ConfigurationObject, lastConfigurationObject));

                // Unhandled cases
                default:
                    throw new TheoremFinderException($"Unhandled value of {nameof(ConfigurationObjectType)}: {lastConfigurationObject.ObjectType}");
            }
        }
    }
}