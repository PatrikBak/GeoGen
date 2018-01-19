using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GeoGen.AnalyticalGeometry;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// A default implementation of <see cref="IObjectsContainersManager"/>.
    /// </summary>
    internal  class ObjectsContainersManager : IObjectsContainersManager
    {
        #region Public constants 

        /// <summary>
        /// The default number of container that this manager manages.
        /// </summary>
        public const int DefaultNumberOfContainers = 5;

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

            // Initialize the constructor function
            List<AnalyticalObject> ConstructorFunction(IObjectsContainer c)
            {
                return _constructor.Construct(looseObjectsList);
            }

            // Add loose objects to all containers
            foreach (var container in _containers)
            {
                container.Add(looseObjectsList, ConstructorFunction);
            }
        }

        #endregion

        #region IEnumerable implementation

        public IEnumerator<IObjectsContainer> GetEnumerator()
        {
            return _containers.GetEnumerator();
        }

        public T ExecuteAndResolvePossibleIncosistencies<T>(Func<T> function)
        {
            var a = 0;
            
            while (true)
            {
                try
                {
                    a++;

                    
                    var result =  function();

                    Wtf.MaximalNeededAttemps = Math.Max(Wtf.MaximalNeededAttemps, a);

                    return result;
                }
                catch (InconsistentContainersException)
                {
                    Wtf.Inconsistencies++;

                    var sw = new Stopwatch();
                    sw.Start();

                    foreach (var container in _containers)
                    {
                        container.Reconstruct();
                    }

                    sw.Stop();
                    Wtf.Count++;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}