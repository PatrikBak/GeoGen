using GeoGen.Core;
using System.Collections.Generic;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents a factory for creating new instances of <see cref="IContextualContainer"/>, 
    /// that should contain given <see cref="ConfigurationObject"/>s and get the actual 
    /// geometric version of them from a <see cref="IObjectsContainersManager"/>.
    /// <para>The implementation is supposed to be provided by the dependency injection management system.</para>
    /// </summary>
    public interface IContextualContainerFactory
    {
        /// <summary>
        /// Creates a new contextual container that holds given objects and gets their 
        /// analytic representations from a given containers manager.
        /// </summary>
        /// <param name="objects">The objects to be present in the container.</param>
        /// <param name="manager">The manager of all the containers where our objects are supposed to be drawn.</param>
        /// <returns>A contextual container holding the given objects.</returns>
        IContextualContainer Create(IReadOnlyList<ConfigurationObject> objects, IObjectsContainersManager manager);
    }
}