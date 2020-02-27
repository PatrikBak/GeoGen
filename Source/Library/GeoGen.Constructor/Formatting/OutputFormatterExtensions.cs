using GeoGen.Core;
using GeoGen.Utilities;
using System.Linq;

namespace GeoGen.Constructor
{
    /// <summary>
    /// Extension methods for <see cref="OutputFormatter"/>
    /// </summary>
    public static class OutputFormatterExtensions
    {
        /// <summary>
        /// Creates a formatted string describing a given geometric object. 
        /// </summary>
        /// <param name="formatter">The output formatter.</param>
        /// <param name="geometricObject">The geometric object.</param>
        /// <returns>The string representing the geometric object.</returns>
        public static string FormatGeometricObject(this OutputFormatter formatter, GeometricObject geometricObject)
        {
            // Prepare the result
            var result = "";

            // If the physical object is present, append it's name
            if (geometricObject.ConfigurationObject != null)
                result += formatter.GetObjectName(geometricObject.ConfigurationObject);

            // Furthermore switch based on type
            switch (geometricObject)
            {
                // If we have an object with points...
                case DefinableByPoints objectWithPoints:

                    // If there are no points, then we're done
                    if (objectWithPoints.Points.IsEmpty())
                        return result;

                    // Otherwise we convert points
                    var pointsString = objectWithPoints.Points
                        // By taking their names
                        .Select(point => formatter.GetObjectName(point.ConfigurationObject))
                        // Ordering them
                        .Ordered()
                        // And joining with commas
                        .ToJoinedString();

                    // And return based on whether this is a line or circle
                    return objectWithPoints switch
                    {
                        // Enclose line points in []
                        LineObject _ => $"{result}[{pointsString}]",

                        // Enclose circle points in ()
                        CircleObject _ => $"{result}({pointsString})",

                        // Otherwise we have an unhandled type
                        _ => throw new GeoGenException($"Unhandled type of {nameof(DefinableByPoints)}: {objectWithPoints.GetType()}")
                    };

                // If we have a point, we do nothing else
                case PointObject _:
                    return result;

                // Unhandled cases
                default:
                    throw new GeoGenException($"Unhandled type of {nameof(GeometricObject)}: {geometricObject.GetType()}");
            }
        }
    }
}