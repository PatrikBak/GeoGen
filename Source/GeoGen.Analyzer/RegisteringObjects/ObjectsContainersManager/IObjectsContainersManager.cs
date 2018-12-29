using System;
using System.Collections.Generic;

namespace GeoGen.Analyzer
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
        /// <typeparam name="T">The return type of the function to be executed.</typeparam>
        /// <param name="function">The function to be executed.</param>
        /// <returns>The returned result of the executed function.</returns>
        T ExecuteAndResolvePossibleIncosistencies<T>(Func<T> function);
    }
}