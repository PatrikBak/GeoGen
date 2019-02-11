using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;

namespace GeoGen.GeometryRegistrar
{
    /// <summary>
    /// The default implementation of <see cref="IConstructorsResolver"/>.
    /// </summary>
    public class ConstructorsResolver : IConstructorsResolver
    {
        #region Dependencies

        /// <summary>
        /// The factory for creating composed constructors for given composed constructions.
        /// </summary>
        private readonly IComposedConstructorFactory _factory;

        #endregion

        #region Private fields

        /// <summary>
        /// The dictionary mapping the types of predefined constructions to their 
        /// predefined constructors.
        /// </summary>
        private readonly Dictionary<PredefinedConstructionType, IPredefinedConstructor> _predefinedConstructors = new Dictionary<PredefinedConstructionType, IPredefinedConstructor>();

        /// <summary>
        /// The dictionary mapping composed constructions to their corresponding constructors.
        /// </summary>
        private readonly Dictionary<ComposedConstruction, IComposedConstructor> _composedConstructors = new Dictionary<ComposedConstruction, IComposedConstructor>();

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructorsResolver"/> class.
        /// </summary>
        /// <param name="constructors">The array of all the predefined constructors.</param>
        /// <param name="factory">The factory for creating composed constructors for given composed constructions.</param>
        public ConstructorsResolver(IPredefinedConstructor[] constructors, IComposedConstructorFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));

            // Register all the predefined constructors
            constructors.ForEach(predefinedConstrutor => _predefinedConstructors.Add(predefinedConstrutor.Type, predefinedConstrutor));
        }

        #endregion

        #region IConstructorsResolver implementation

        /// <summary>
        /// Find the corresponding objects constructor for a given construction.
        /// </summary>
        /// <param name="construction">The construction for which we want a constructor.</param>
        /// <returns>A constructor that performs the given construction.</returns>
        public IObjectsConstructor Resolve(Construction construction)
        {
            // If we have a predefined constructor...
            if (construction is PredefinedConstruction predefinedConstruction)
            {
                try
                {
                    // We simply look into the dictionary assuming it's there
                    return _predefinedConstructors[predefinedConstruction.Type];
                }
                catch (KeyNotFoundException)
                {
                    // If not, we have a problem...
                    throw new RegistrarException($"Unresolvable predefined constructor of type {predefinedConstruction.Type}.");
                }
            }

            // Otherwise we have a composed construction
            var composedConstruction = (ComposedConstruction) construction;

            // Get it from the composed constructors dictionary, or create it (using the factory), add it, and return it
            return _composedConstructors.GetOrAdd(composedConstruction, () => _factory.Create(composedConstruction));
        }

        #endregion
    }
}