using System;
using System.Collections.Generic;
using GeoGen.AnalyticalGeometry;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents an output of a <see cref="IObjectsConstructor"/>.
    /// </summary>
    internal class ConstructorOutput
    {
        /// <summary>
        /// Gets or sets the constructor function that is able to constructs
        /// an object using a <see cref="IObjectsContainer"/> container.
        /// </summary>
        public Func<IObjectsContainer, List<AnalyticalObject>> ConstructorFunction { get; set; }

        /// <summary>
        /// Gets or sets the list of default theorems that comes with
        /// a construction.
        /// </summary>
        public List<Theorem> Theorems { get; set; }
    }
}