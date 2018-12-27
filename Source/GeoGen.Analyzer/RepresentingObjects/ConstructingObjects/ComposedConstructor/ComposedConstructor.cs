using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.AnalyticGeometry;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// A default implementation of <see cref="IComposedConstructor"/> that uses 
    /// <see cref="ObjectsConstructorBase"/>.
    /// </summary>
    public class ComposedConstructor : ObjectsConstructorBase, IComposedConstructor
    {
        #region Private fields

        /// <summary>
        /// The actual construction performed by the constructor.
        /// </summary>
        private readonly ComposedConstruction _construction;

        /// <summary>
        /// The resolver of constructions used while constructing the configuration
        /// from the composed construction.
        /// </summary>
        private readonly IConstructorsResolver _resolver;

        /// <summary>
        /// The factory for creating objects container used while constructing 
        /// the configuration from the composed construction.
        /// </summary>
        private readonly IObjectsContainerFactory _factory;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="construction">The composed construction that is actually handled.</param>
        /// <param name="resolver">The resolver of constructions for internal objects of the configuration.</param>
        /// <param name="factory">The objects container factory for making containers for the internal construction.</param>
        public ComposedConstructor(ComposedConstruction construction, IConstructorsResolver resolver, IObjectsContainerFactory factory)
        {
            _construction = construction ?? throw new ArgumentNullException(nameof(construction));
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        #endregion

        #region ObjectsConstructorBase methods

        /// <summary>
        /// Constructs a list of analytic objects from a given list of 
        /// flattened objects from the arguments and a container that is used to 
        /// obtain the actual analytic versions of these objects.
        /// </summary>
        /// <param name="flattenedObjects">The flattened argument objects.</param>
        /// <param name="container">The objects container.</param>
        /// <returns>The list of constructed analytic objects.</returns>
        protected override List<AnalyticObject> Construct(IReadOnlyList<ConfigurationObject> flattenedObjects, IObjectsContainer container)
        {
            // Pull the loose objects (that should correspond to the flatten ones)
            var looseObjects = _construction.Configuration.LooseObjectsHolder.LooseObjects;

            // Initialize the internal container for sub-results
            var internalContainer = _factory.CreateContainer();

            // Create constructor function
            List<AnalyticObject> InternalConstructorFunction(IObjectsContainer c)
            {
                // Prepare result
                var result = new List<AnalyticObject>();

                // Map loose objects to the flattened objects
                for (var i = 0; i < looseObjects.Count; i++)
                {
                    // Take input object
                    var inputObject = flattenedObjects[i];

                    // Take real object
                    var realObjectAnalytic = container.Get(inputObject);

                    // Add this object to the results list
                    result.Add(realObjectAnalytic);
                }

                return result;
            }

            // Add loose objects to the internal container
            internalContainer.Add(looseObjects, InternalConstructorFunction);

            // Initialize the constructed objects
            foreach (var internalConstructedObjects in _construction.Configuration.GroupConstructedObjects())
            {
                // Find the constructor
                var constructor = _resolver.Resolve(internalConstructedObjects[0].Construction);

                // Find the constructor function
                var constructorFunction = constructor.Construct(internalConstructedObjects).ConstructorFunction;

                // Add objects to the container
                var result = internalContainer.Add(internalConstructedObjects, constructorFunction);

                // Find out if we have correct result
                var correctResult = result != null && result.SequenceEqual(internalConstructedObjects);

                // If not, the whole construction is not possible
                if (!correctResult)
                    return null;
            }

            // Return the result
            return new List<ConstructedConfigurationObject> { _construction.ConstructionOutput }.Select(internalContainer.Get).ToList();
        }

        /// <summary>
        /// Constructs a list of default theorems using a newly constructed objects and
        /// flattened objects from the passed arguments.
        /// </summary>
        /// <param name="input">The constructed objects.</param>
        /// <param name="flattenedObjects">The flattened argument objects.</param>
        /// <returns>The list of default theorems.</returns>
        protected override List<Theorem> FindDefaultTheorms(IReadOnlyList<ConstructedConfigurationObject> input, IReadOnlyList<ConfigurationObject> flattenedObjects)
        {
            // Invoke the function from the constructor, or return an empty list, if it's not set
            return new List<Theorem>();
        }

        #endregion
    }
}