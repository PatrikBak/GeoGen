using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// A default implementation of <see cref="IObjectsContainersManager"/>.
    /// </summary>
    internal sealed class ObjectsContainersManager : IObjectsContainersManager
    {
        #region Public constants 

        /// <summary>
        /// The default number of container that this manager manages.
        /// </summary>
        public const int DefaultNumberOfContainers = 3;

        #endregion

        #region Private fields

        /// <summary>
        /// The loose objects constructor.
        /// </summary>
        private readonly ILooseObjectsConstructor _constructor;

        /// <summary>
        /// The list of all objects containers.
        /// </summary>
        private readonly List<IObjectsContainer> _containers;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new objects containers manager that creates
        /// containers using a given factory, uses a given loose objects
        /// constructor to initialize the containers and holds a given
        /// number of containers.
        /// </summary>
        /// <param name="looseObjects"></param>
        /// <param name="factory">The objects containers factory.</param>
        /// <param name="constructor">The loose objects constructor.</param>
        public ObjectsContainersManager
        (
                IEnumerable<LooseConfigurationObject> looseObjects,
                IObjectsContainerFactory factory,
                ILooseObjectsConstructor constructor
        )
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            _constructor = constructor ?? throw new ArgumentNullException(nameof(constructor));

            _containers = Enumerable.Range(0, DefaultNumberOfContainers)
                    .Select(i => factory.CreateContainer())
                    .ToList();

            Initialize(looseObjects);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Initializes the manager with given loose objects. The manager is supposed
        /// to create containers and initialize them with the given objects.
        /// </summary>
        /// <param name="looseObjects">The loose objects.</param>
        private void Initialize(IEnumerable<LooseConfigurationObject> looseObjects)
        {
            if (looseObjects == null)
                throw new ArgumentNullException(nameof(looseObjects));

            // Enumerate the loose objects
            var looseObjectsList = looseObjects.ToList();

            // Check if they don't contain null
            if (looseObjectsList.Contains(null))
                throw new ArgumentException("Null object present");

            // Find their ids (which must exist)
            var ids = looseObjectsList.Select(obj => obj.Id ?? throw new AnalyzerException("Id must be set"))
                    .Distinct()
                    .ToList();

            // Check if there are objects with duplicate ids.
            if (ids.Count != looseObjectsList.Count)
                throw new ArgumentException("Duplicate objects");

            // For each container
            foreach (var container in _containers)
            {
                // Construct the objects
                var objects = _constructor.Construct(looseObjectsList);

                // Iterate over them
                for (var i = 0; i < looseObjectsList.Count; i++)
                {
                    // Pull the configuration object
                    var configurationObject = looseObjectsList[i];

                    // Pull the analytical version of it
                    var analyticalObject = objects[i];

                    // Add it to the container
                    container.Add(analyticalObject, configurationObject);
                }
            }
        }

        #endregion

        #region IEnumerable implementation

        public IEnumerator<IObjectsContainer> GetEnumerator()
        {
            return _containers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}