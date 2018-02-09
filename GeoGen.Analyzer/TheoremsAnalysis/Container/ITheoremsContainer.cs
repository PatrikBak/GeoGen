using System.Collections.Generic;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents a simple container that is able to recognize equal theorems. It is currently used
    /// as a container of default theorems provided by object constructors.
    /// </summary>
    internal interface ITheoremsContainer : IEnumerable<Theorem>
    {
        /// <summary>
        /// Adds a given theorem to the container.
        /// </summary>
        /// <param name="theorem">The theorem.</param>
        void Add(Theorem theorem);

        /// <summary>
        /// Finds out if a given theorem is present in the container.
        /// </summary>
        /// <param name="theorem">The theorem.</param>
        /// <returns>true, if it is present, false otherwise;</returns>
        bool Contains(Theorem theorem);
    }
}