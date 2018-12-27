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
    }
}