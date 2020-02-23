using GeoGen.Core;
using System.Collections.Generic;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Represents a service that is able to suggests objects that are related to provided <see cref="ConstructedConfigurationObject"/>s
    /// and could be used in proofs of theorems involving these provided objects.
    /// </summary>
    public interface IObjectIntroducer
    {
        /// <summary>
        /// Enumerates available options for new objects to be introduced in relation to given available objects.
        /// </summary>
        /// <param name="availableObjects">The objects in relation to which the new objects should be introduced.</param>
        /// <returns>The enumerable of options for new objects to be introduced.</returns>
        IEnumerable<IEnumerable<ConstructedConfigurationObject>> IntroduceObjects(IReadOnlyList<ConstructedConfigurationObject> availableObjects);
    }
}