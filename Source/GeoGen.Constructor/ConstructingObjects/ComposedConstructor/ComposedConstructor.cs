using GeoGen.AnalyticGeometry;
using GeoGen.Core;
using System;
using System.Linq;

namespace GeoGen.Constructor
{
    /// <summary>
    /// The default implementation of <see cref="IComposedConstructor"/> that 
    /// receives a <see cref="ComposedConstruction"/> as a constructor parameter. 
    /// </summary>
    public class ComposedConstructor : ObjectsConstructorBase, IComposedConstructor
    {
        #region Dependencies

        /// <summary>
        /// The resolver of constructors used while constructing the internal configuration of the composed construction.
        /// </summary>
        private readonly IConstructorsResolver _constructionResolver;

        /// <summary>
        /// The factory for creating objects containers in which we're constructing the internal configuration of the composed construction.
        /// </summary>
        private readonly IObjectsContainerFactory _containersFactory;

        #endregion

        #region Private fields

        /// <summary>
        /// The composed construction performed by the constructor.
        /// </summary>
        private readonly ComposedConstruction _construction;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ComposedConstructor"/> class.
        /// </summary>
        /// <param name="construction">The composed construction performed by the constructor.</param>
        /// <param name="constructionResolver">The resolver of constructors used while constructing the internal configuration of the composed construction.</param>
        /// <param name="containersFactory">The factory for creating objects containers in which we're constructing the internal configuration of the composed construction.</param>
        public ComposedConstructor(ComposedConstruction construction, IConstructorsResolver constructionResolver, IObjectsContainerFactory containersFactory)
        {
            _construction = construction ?? throw new ArgumentNullException(nameof(construction));
            _constructionResolver = constructionResolver ?? throw new ArgumentNullException(nameof(constructionResolver));
            _containersFactory = containersFactory ?? throw new ArgumentNullException(nameof(containersFactory));
        }

        #endregion

        #region ObjectsConstructorBase implementation

        /// <summary>
        /// Performs the actual construction of an analytic object based on the analytic objects given as an input.
        /// The order of the objects of the input is based on the <see cref="Arguments.FlattenedList"/>.
        /// </summary>
        /// <param name="input">The analytic objects to be used as an input.</param>
        /// <returns>The constructed analytic object, if the construction was successful; or null otherwise.</returns>
        protected override IAnalyticObject Construct(IAnalyticObject[] input)
        {
            // Initialize an internal container in which we're going to construct
            // the configuration that defines our composed construction
            var internalContainer = _containersFactory.CreateContainer();

            // Pull the loose objects of this configuration
            var looseObjects = _construction.Configuration.LooseObjectsHolder.LooseObjects;

            // Add these objects to the internal container
            // Their analytic versions should correspond to the passed input
            internalContainer.Add(looseObjects, () => input.ToList());

            // Add the constructed objects as well
            foreach (var constructedObject in _construction.Configuration.ConstructedObjects)
            {
                // For each one create the construction function
                var constructorFunction = _constructionResolver.Resolve(constructedObject.Construction).Construct(constructedObject);

                // Add the object to the container using this function that gets passed the internal container
                internalContainer.TryAdd(constructedObject, () => constructorFunction(internalContainer), out var objectConstructed, out var equalObject);

                // Find out if we have a correct result
                var correctResult = objectConstructed && equalObject == null;

                // If not, the construction failed
                if (!correctResult)
                    return null;
            }

            // If we are here, then the construction should be fine and the result
            // will be in the internal container corresponding to the last object 
            // of the configuration that defines our composed construction
            return internalContainer.Get(_construction.Configuration.ConstructedObjects.Last());
        }

        #endregion
    }
}