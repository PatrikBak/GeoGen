using GeoGen.AnalyticGeometry;
using GeoGen.Core;
using System.Collections.Generic;

namespace GeoGen.Constructor
{
    /// <summary>
    /// Represents a picture that holds <see cref="GeometricObject"/>s. This picture
    /// is responsible for creating them and mapping them between <see cref="IAnalyticObject"/>s
    /// with respect to <see cref="IPicture"/>s from  <see cref="IPicturesManager"/>.
    /// </summary>
    public interface IContextualPicture
    {
        /// <summary>
        /// Gets the geometric objects matching a given query and casts them to a given type.
        /// </summary>
        /// <typeparam name="T">The type of objects.</typeparam>
        /// <param name="query">The query that we want to perform.</param>
        /// <returns>The queried objects.</returns>
        IEnumerable<T> GetGeometricObjects<T>(ContextualPictureQuery query) where T : GeometricObject;

        /// <summary>
        /// Gets the geometric objects of the requested type that corresponds to a given configuration object.
        /// </summary>
        /// <typeparam name="T">The type of the geometric object.</typeparam>
        /// <param name="configurationObject">The configuration object.</param>
        /// <returns>The corresponding geometric object.</returns>
        T GetGeometricObject<T>(ConfigurationObject configurationObject) where T : GeometricObject;

        /// <summary>
        /// Gets the analytic representation of a given geometric object in a given picture.
        /// </summary>
        /// /// <typeparam name="T">The wanted type of the analytic object.</typeparam>
        /// <param name="geometricObject">The geometric object.</param>
        /// <param name="picture">The picture.</param>
        /// <returns>The analytic object represented by the given geometric object in the given picture.</returns>
        T GetAnalyticObject<T>(GeometricObject geometricObject, IPicture picture) where T : IAnalyticObject;
    }
}