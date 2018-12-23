﻿using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents a factory for creating new instances of a type <see cref="IComposedConstructor"/>.
    /// NOTE: The implementation will be auto-generated by NInject. 
    /// </summary>
    public interface IComposedConstructorFactory
    {
        /// <summary>
        /// Creates a composed constructor that performs a given composed construction.
        /// </summary>
        /// <param name="construction">The composed construction.</param>
        /// <returns>The constructor.</returns>
        IComposedConstructor Create(ComposedConstruction construction);
    }
}