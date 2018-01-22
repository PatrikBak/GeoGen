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
        /// Gets or sets the constructor function that constructs analytical objects
        /// object using an <see cref="IObjectsContainer"/> container.
        /// </summary>
        public Func<IObjectsContainer, List<AnalyticalObject>> ConstructorFunction { get; set; }

        /// <summary>
        /// Gets or sets the function that creates the list of default theorems.
        /// </summary>
        public Func<List<Theorem>> DefaultTheoremsFunction { get; set; }
    }
}