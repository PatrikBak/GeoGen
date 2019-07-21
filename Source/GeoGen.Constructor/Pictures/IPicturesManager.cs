using System;
using System.Collections.Generic;

namespace GeoGen.Constructor
{
    /// <summary>
    /// Represents a collection of <see cref="IPicture"/>s. It automatically attempts to take care of resolving 
    /// possible inconsistencies (see the documentation of <see cref="InconsistentPicturesException"/>). If it can't 
    /// be done, it throws an <see cref="UnresolvedInconsistencyException"/>.
    /// </summary>
    public interface IPicturesManager : IEnumerable<IPicture>
    {
        /// <summary>
        /// Performs a given function that might cause an <see cref="InconsistentPicturesException"/> and tries 
        /// to handle it by reconstructing all the pictures. If the exception couldn't be handled, throws an
        /// <see cref="UnresolvedInconsistencyException"/>.
        /// </summary>
        /// <param name="function">The action to be executed.</param>
        /// <param name="exceptionCallback">The action called after an <see cref="InconsistentPicturesException"/> occurs.</param>
        void ExecuteAndResolvePossibleIncosistencies(Action action, Action<InconsistentPicturesException> exceptionCallback);
    }
}