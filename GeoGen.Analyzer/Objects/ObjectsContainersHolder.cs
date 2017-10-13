using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Utilities;

namespace GeoGen.Analyzer.Objects
{
    internal sealed class ObjectsContainersHolder : IObjectsContainersHolder
    {
        private const int NumberOfContainers = 5;

        private readonly List<IObjectsContainer> _containers;

        private readonly IObjectsContainersFactory _factory;

        public ObjectsContainersHolder(IObjectsContainersFactory factory)
        {
            _factory = factory;
            _containers = new List<IObjectsContainer>();
        }

        public void Initialize(IEnumerable<LooseConfigurationObject> looseObjects)
        {
            var containers = Enumerable.Range(0, NumberOfContainers)
                    .Select(i => _factory.CreateContainer(looseObjects));

            _containers.SetItems(containers);
        }

        public IEnumerator<IObjectsContainer> GetEnumerator()
        {
            return _containers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}