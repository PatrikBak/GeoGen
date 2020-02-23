using GeoGen.Core;
using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Constructor
{
    /// <summary>
    /// The extension methods for <see cref="InconsistentPicturesException"/>.
    /// </summary>
    public static class InconsistentPicturesExceptionExtensions
    {
        /// <summary>
        /// Formats the information about the exception into a human-readable string.
        /// </summary>
        /// <param name="exception">The exception to be formatted.</param>
        /// <param name="formatter">The formatter of objects to be used in formatting.</param>
        /// <returns>The formatted string.</returns>
        public static string Format(this InconsistentPicturesException exception, OutputFormatter formatter)
        {
            // Switch based on the exception type
            return exception switch
            {
                // In collinearity case we have some points
                InconsistentCollinearityException e => $"The collinearities among points " +
                    // Format the points
                    $"'{e.ProblematicPoints.Select(formatter.FormatGeometricObject).Ordered().ToJoinedString()}' " +
                    // Finish the message
                    $"couldn't be determined consistently.",

                // In concyclity case we have some points
                InconsistentConcyclityException e => $"The concyclities among points " +
                    // Format the points
                    $"'{e.ProblematicPoints.Select(formatter.FormatGeometricObject).Ordered().ToJoinedString()}' " +
                    // Finish the message
                    $"couldn't be determined consistently.",

                // In constructibility case we have just the object
                InconsistentConstructibilityException e => $"The constructibility of object " +
                    // Format the object
                    $"'{formatter.GetObjectName(e.ConstructedObject)}' " +
                    // Finish the message
                    $"couldn't be determined consistently.",

                // In equality case we have the object itself
                InconsistentEqualityException e => $"The equality of '{formatter.GetObjectName(e.ConstructedObject)}' and " +
                    // Format the equal objects
                    $"'{e.EqualObjects?.Select(formatter.GetObjectName).Ordered().ToJoinedString() ?? e.EqualGeometricObjects?.Select(formatter.FormatGeometricObject).Ordered().ToJoinedString()}' " +
                    // Finish the message
                    $"couldn't be determined consistently.",

                // In incidence case we have the inner object of the point
                InconsistentIncidenceException e => $"The incidence between point '{formatter.FormatGeometricObject(e.Point)}' and " +
                    // Format the line / circle
                    $"'{(e.LineOrCircle != null ? formatter.GetObjectName(e.LineOrCircle) : formatter.FormatGeometricObject(e.GeometricLineOrCircle))}' " +
                    // Finish the message
                    $"couldn't be determined consistently.",

                // Case when the exception doesn't bring any more information
                InconsistentPicturesException e => e.Message,

                // Unhandled cases
                _ => throw new GeoGenException($"Unhandled type of {nameof(InconsistentPicturesException)}: {exception.GetType()}")
            };
        }

        /// <summary>
        /// Returns the inner objects about which the exception holds data based on its type.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>The inner exception objects.</returns>
        public static IEnumerable<ConfigurationObject> GetInnerObjects(this InconsistentPicturesException exception)
        {
            // Local helper function to get inner objects of a geometric object
            static IEnumerable<ConfigurationObject> GetInnerObjects(GeometricObject geometricObject)
            {
                // If the physical object is present, return it
                if (geometricObject.ConfigurationObject != null)
                    yield return geometricObject.ConfigurationObject;

                // Furthermore switch based on type
                switch (geometricObject)
                {
                    // If we have an object with points...
                    case DefinableByPoints objectWithPoints:

                        // Then return all the inner objects of the points
                        foreach (var point in objectWithPoints.Points)
                            yield return point.ConfigurationObject;

                        // And break
                        yield break;

                    // If we have a point, we do nothing else
                    case PointObject _:
                        yield break;

                    // Unhandled cases
                    default:
                        throw new GeoGenException($"Unhandled type of {nameof(GeometricObject)}: {geometricObject.GetType()}");
                }
            }

            // Switch based on the exception type
            return exception switch
            {
                // In collinearity case we have some points
                InconsistentCollinearityException e => e.ProblematicPoints.SelectMany(GetInnerObjects),

                // In concyclity case we have some points
                InconsistentConcyclityException e => e.ProblematicPoints.SelectMany(GetInnerObjects),

                // In constructibility case we have just the object
                InconsistentConstructibilityException e => e.ConstructedObject.ToEnumerable(),

                // In equality case we have the object itself
                InconsistentEqualityException e => e.ConstructedObject.ToEnumerable()
                        // And possibly the potentially equal configuration objects
                        .Concat(e.EqualObjects ?? Enumerable.Empty<ConfigurationObject>())
                        // And possibly the potentially equal geometric objects (whose objects will get the helper method)
                        .Concat(e.EqualGeometricObjects?.SelectMany(GetInnerObjects) ?? Enumerable.Empty<ConfigurationObject>()),

                // In incidence case we have the point
                InconsistentIncidenceException e => e.Point.ConfigurationObject.ToEnumerable()
                        // And possibly the configuration line or circle 
                        .Concat(e.LineOrCircle?.ToEnumerable() ?? Enumerable.Empty<ConfigurationObject>())
                        // And possibly the geometric line or circle (whose objects will get the helper method)
                        .Concat(e.GeometricLineOrCircle != null ? GetInnerObjects(e.GeometricLineOrCircle) : Enumerable.Empty<ConfigurationObject>()),

                // Case when the exception doesn't bring any more objects
                InconsistentPicturesException _ => Enumerable.Empty<ConfigurationObject>(),

                // Unhandled cases
                _ => throw new GeoGenException($"Unhandled type of {nameof(InconsistentPicturesException)}: {exception.GetType()}"),
            };
        }
    }
}
