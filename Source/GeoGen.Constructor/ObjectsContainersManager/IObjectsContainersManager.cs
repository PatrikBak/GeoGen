using System;
using System.Collections.Generic;

namespace GeoGen.Constructor
{
    /// <summary>
    /// Represents a collection of <see cref="IObjectsContainer"/>s. It automatically attempts to take care of resolving 
    /// possible inconsistencies (see the documentation of <see cref="InconsistentContainersException"/>). If it can't 
    /// be done, it throws an <see cref="UnresolvableInconsistencyException"/>.
    /// </summary>
    public interface IObjectsContainersManager : IEnumerable<IObjectsContainer>
    {
        /// <summary>
        /// Performs a given function that might cause an <see cref="InconsistentContainersException"/> and tries 
        /// to handle it. If the exception couldn't be handled, throws an <see cref="UnresolvableInconsistencyException"/>.
        /// </summary>
        /// <param name="function">The action to be executed.</param>
        /// <param name="exceptionCallback">The action called after an <see cref="InconsistentContainersException"/> occurs.</param>
        void ExecuteAndResolvePossibleIncosistencies(Action action, Action<InconsistentContainersException> exceptionCallback);

        /// <summary>
        /// Tries to reconstruct all the containers that this manager manages. 
        /// If the exception couldn't be handled, throws an <see cref="UnresolvableInconsistencyException"/>.
        /// </summary>
        void TryReconstructContainers();
    }
}