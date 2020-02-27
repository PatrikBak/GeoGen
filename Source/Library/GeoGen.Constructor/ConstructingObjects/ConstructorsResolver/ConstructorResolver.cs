using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;

namespace GeoGen.Constructor
{
    /// <summary>
    /// The default implementation of <see cref="IConstructorResolver"/> that gets <see cref="IPredefinedConstructor"/>s
    /// and <see cref="IComposedConstructorFactory"/> injected and based on those finds the right constructors.
    /// </summary>
    public class ConstructorResolver : IConstructorResolver
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
        /// The dictionary mapping names of composed constructions to their corresponding constructors.
        /// </summary>
        private readonly Dictionary<string, IComposedConstructor> _composedConstructors = new Dictionary<string, IComposedConstructor>();

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructorResolver"/> class.
        /// </summary>
        /// <param name="constructors">The array of all the predefined constructors.</param>
        /// <param name="factory">The factory for creating composed constructors for given composed constructions.</param>
        public ConstructorResolver(IPredefinedConstructor[] constructors, IComposedConstructorFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));

            // Register all the predefined constructors
            constructors.ForEach(predefinedConstrutor => _predefinedConstructors.Add(predefinedConstrutor.Type, predefinedConstrutor));
        }

        #endregion

        #region IConstructorsResolver implementation

        /// <summary>
        /// Find the corresponding object constructor for a given construction.
        /// </summary>
        /// <param name="construction">The construction for which we want a constructor.</param>
        /// <returns>A constructor that performs the given construction.</returns>
        public IObjectConstructor Resolve(Construction construction)
        {
            // Switch based on the type of construction
            switch (construction)
            {
                // If we have a predefined construction...
                case PredefinedConstruction predefinedConstruction:

                    try
                    {
                        // We simply look into the dictionary assuming it's there
                        return _predefinedConstructors[predefinedConstruction.Type];
                    }
                    catch (KeyNotFoundException)
                    {
                        // If not, we have a problem...
                        throw new ConstructorException($"The predefined construction with type {predefinedConstruction.Type} does not have a constructor.");
                    }

                // If we have a composed construction...
                case ComposedConstruction composedConstruction:

                    // Get it from the composed constructors dictionary, or create it (using the factory), add it, and return it
                    return _composedConstructors.GetValueOrCreateAddAndReturn(composedConstruction.Name, () => _factory.Create(composedConstruction));

                // Unhandled cases
                default:
                    throw new ConstructorException($"Unhandled type of {nameof(Construction)}: {construction.GetType()}");
            }
        }

        #endregion
    }
}