using System;
using System.Collections.Generic;
using GeoGen.AnalyticalGeometry;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// A default implementation of <see cref="IConstructorsResolver"/>.
    /// </summary>
    internal class ConstructorsResolver : IConstructorsResolver
    {
        #region Private fields

        /// <summary>
        /// The dictionary mapping the types of predefined constructions to their 
        /// predefined constructors.
        /// </summary>
        private readonly Dictionary<PredefinedConstructionType, IPredefinedConstructor> _predefinedConstructors;

        /// <summary>
        /// The dictionary mapping ids of composed constructions to their corresponding
        /// constructors.
        /// </summary>
        private readonly Dictionary<int, IComposedConstructor> _composedConstructors;

        /// <summary>
        /// The factory for creating composed constructors for a given composed construction.
        /// </summary>
        private readonly IComposedConstructorFactory _factory;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="constructors">The predefined constructors array.</param>
        /// <param name="factory">The factory for creating composed constructors.</param>
        public ConstructorsResolver(IEnumerable<IPredefinedConstructor> constructors, IComposedConstructorFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _predefinedConstructors = new Dictionary<PredefinedConstructionType, IPredefinedConstructor>();
            _composedConstructors = new Dictionary<int, IComposedConstructor>();

            // Register all predefined constructors
            foreach (var constructor in constructors)
            {
                _predefinedConstructors.Add(constructor.Type, constructor);
            }
        }

        #endregion

        #region IConstructorsResolver implementation

        /// <summary>
        /// Find the corresponding <see cref="IObjectsConstructor"/> for 
        /// a given <see cref="Construction"/>.
        /// </summary>
        /// <param name="construction">The construction.</param>
        /// <returns>The constructor.</returns>
        public IObjectsConstructor Resolve(Construction construction)
        {
            // For predefined construction
            if (construction is PredefinedConstruction predefinedConstruction)
            {
                try
                {
                    // We simply look into the dictionary
                    return _predefinedConstructors[predefinedConstruction.Type];
                }
                catch (KeyNotFoundException)
                {
                    throw new AnalyzerException($"Unresolvable predefined constructor of type {predefinedConstruction.Type}.");
                }
            }

            // Otherwise we have a composed construction
            var composedConstruction = (ComposedConstruction) construction;

            // We pull its id
            var id = composedConstruction.Id ?? throw new AnalyzerException("Id must be set");

            // Look for it in the dictionary
            if (_composedConstructors.ContainsKey(id))
                return _composedConstructors[id];

            // If it's not there, we let the factory create the constructor
            var constructor = _factory.Create(composedConstruction);

            // Update the dictionary
            _composedConstructors.Add(id, constructor);

            // And return it
            return constructor;
        }

        #endregion
    }
}