using System.Collections.Generic;
using GeoGen.Core.Configurations;

namespace GeoGen.Analyzer.Objects
{
    /// <summary>
    /// Represents a holder of all geometry. In fact, it's a wrapper for all
    /// interpretations of geometrical objects, i.e. geometrical objects containers.
    /// It's supposed to handle adding new objects, finding out if the object can be constructed
    /// and finding duplicate objects.
    /// </summary>
    internal interface IGeometryRegistrar
    {
        /// <summary>
        /// Initializes the holder with a given configuration.
        /// </summary>
        /// <param name="looseObjects">The loose objects enumerable.</param>
        void Initialize(Configuration looseObjects);

        /// <summary>
        /// Registers a given object to the holder.
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        /// <param name="duplicateVersion">The out parameter where the reference to duplicate object will be set.</param>
        /// <returns>true, if the content has changed, false otherwise.</returns>
        RegistrationResult Register(List<ConstructedConfigurationObject> constructedObject);
    }
}