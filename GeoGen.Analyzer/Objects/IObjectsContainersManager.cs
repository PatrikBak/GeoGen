using System.Collections.Generic;
using GeoGen.Core.Configurations;

namespace GeoGen.Analyzer.Objects
{
    /// <summary>
    /// Represents a manager of all available <see cref="IObjectsContainer"/>s.
    /// </summary>
    internal interface IObjectsContainersManager : IEnumerable<IObjectsContainer>
    {
        /// <summary>
        /// Initializes the manager with given loose objects. The manager is supposed
        /// to create containers and initialize them with the given objects.
        /// </summary>
        /// <param name="looseObjects">The loose objects.</param>
        void Initialize(IEnumerable<LooseConfigurationObject> looseObjects);
    }
}