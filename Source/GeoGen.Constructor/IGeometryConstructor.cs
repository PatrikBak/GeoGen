using GeoGen.AnalyticGeometry;
using GeoGen.Core;
using System.Collections.Generic;

namespace GeoGen.Constructor
{
    /// <summary>
    /// Represents a service that cares care of geometric construction of <see cref="Configuration"/>s and
    /// <see cref="ConfigurationObject"/>s.
    /// </summary>
    public interface IGeometryConstructor
    {
        /// <summary>
        /// Constructs a given <see cref="Configuration"/>. Throws a <see cref="GeometryConstructionException"/>
        /// if the construction couldn't be carried out.
        /// </summary>
        /// <param name="configuration">The configuration to be constructed.</param>
        /// <returns>The tuple consisting of the pictures and the construction data.</returns>
        (Pictures pictures, ConstructionData data) Construct(Configuration configuration);

        /// <summary>
        /// Constructs a given <see cref="Configuration"/> using an already constructed old one.
        /// It is assumed that the new configuration differs only by the last object from the already 
        /// constructed one. Thus only the last object is constructed. Throws a 
        /// <see cref="GeometryConstructionException"/> if the construction couldn't be carried out.
        /// </summary>
        /// <param name="oldConfigurationPictures">The pictures where the old configuration is drawn.</param>
        /// <param name="newConfiguration">The new configuration that should be drawn.</param>
        /// <returns>The tuple consisting of the pictures and the construction data.</returns>
        (Pictures pictures, ConstructionData data) ConstructByCloning(Pictures oldConfigurationPictures, Configuration newConfiguration);

        /// <summary>
        /// Constructs a given <see cref="ConstructedConfigurationObject"/> without adding it to the pictures.
        /// It is assumed that the constructed object can be construed in the passed pictures. Throws a 
        /// <see cref="GeometryConstructionException"/> if the construction couldn't be carried out.
        /// </summary>
        /// <param name="pictures">The pictures that should contain the input for the construction.</param>
        /// <param name="constructedObject">The object that is about to be constructed.</param>
        /// <returns>The construction data.</returns>
        ConstructionData ExamineObject(Pictures pictures, ConstructedConfigurationObject constructedObject);

        /// <summary>
        /// Constructs a given <see cref="ConstructedConfigurationObject"/> without adding it to the pictures.
        /// It is assumed that the constructed object can be construed in the passed pictures. The fact whether
        /// the object is or is not already present in individual pictures is ignored. If the object is 
        /// inconstructible, null is returned. Throws a <see cref="GeometryConstructionException"/> if the 
        /// construction couldn't be carried out.
        /// </summary>
        /// <param name="pictures">The pictures that should contain the input for the construction.</param>
        /// <param name="constructedObject">The object that is about to be constructed.</param>
        /// <returns>The dictionary mapping pictures to constructed objects, or null; if the object is inconstructible.</returns>
        IReadOnlyDictionary<Picture, IAnalyticObject> Construct(Pictures pictures, ConstructedConfigurationObject constructedObject);
    }
}