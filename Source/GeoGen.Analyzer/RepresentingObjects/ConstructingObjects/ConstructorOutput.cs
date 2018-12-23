using System;
using System.Collections.Generic;
using GeoGen.AnalyticGeometry;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents an output of a <see cref="IObjectsConstructor"/>.
    /// </summary>
    public class ConstructorOutput
    {
        /// <summary>
        /// Gets or sets the constructor function that constructs analytic objects
        /// object using an <see cref="IObjectsContainer"/> container.
        /// </summary>
        public Func<IObjectsContainer, List<AnalyticObject>> ConstructorFunction { get; set; }

        /// <summary>
        /// Gets or sets the function that creates the list of default theorems.
        /// </summary>
        public Func<List<Theorem>> DefaultTheoremsFunction { get; set; }
    }
}