using System;
using System.Collections.Generic;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// A default implementation of <see cref="IConstructorsResolver"/>.
    /// </summary>
    internal sealed class ConstructorsResolver : IConstructorsResolver
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
        /// The factory for getting composed constructors for a given composed construction.
        /// </summary>
        private readonly IComposedConstructorFactory _factory;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new constructors resolver with provided predefined
        /// constructors.
        /// </summary>
        /// <param name="constructors">The predefined constructors array.</param>
        public ConstructorsResolver(IPredefinedConstructor[] constructors, IComposedConstructorFactory factory)
        {
            if (constructors == null)
                throw new ArgumentNullException(nameof(constructors));

            _factory = factory ?? throw new ArgumentNullException(nameof(factory));

            _predefinedConstructors = new Dictionary<PredefinedConstructionType, IPredefinedConstructor>();
            _composedConstructors = new Dictionary<int, IComposedConstructor>();

            foreach (var constructor in constructors)
            {
                if (constructor == null)
                    throw new ArgumentException("Constructors can't contain null objects");

                var type = constructor.PredefinedConstructionType;

                if (_predefinedConstructors.ContainsKey(type))
                    throw new ArgumentException("Duplicate constructors");

                _predefinedConstructors.Add(type, constructor);
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
            if (construction == null)
                throw new ArgumentNullException(nameof(construction));

            if (construction is PredefinedConstruction predefinedConstruction)
            {
                try
                {
                    return _predefinedConstructors[predefinedConstruction.Type];
                }
                catch (KeyNotFoundException)
                {
                    throw new AnalyzerException("Unresolvable predefined constructor.");
                }
            }

            var composedConstruction = (ComposedConstruction) construction;

            var id = composedConstruction.Id ?? throw new AnalyzerException("Id must be set");

            if (_composedConstructors.ContainsKey(id))
                return _composedConstructors[id];

            var constructor = _factory.Create(composedConstruction);

            _composedConstructors.Add(id, constructor);

            return constructor;
        }

        #endregion
    }
}