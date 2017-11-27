using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Utilities;

namespace GeoGen.Analyzer.Objects
{
    internal sealed class ObjectsContainersHolder : IObjectsContainersHolder
    {
        public const int NumberOfContainers = 5;

        private readonly List<IObjectsContainer> _containers;

        private readonly IObjectsContainersFactory _factory;

        public ObjectsContainersHolder(IObjectsContainersFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _containers = new List<IObjectsContainer>();
        }

        public void Initialize(IEnumerable<LooseConfigurationObject> looseObjects)
        {
            if (looseObjects == null)
                throw new ArgumentNullException(nameof(looseObjects));

            var list = looseObjects.ToList();

            if (list.Contains(null))
                throw new ArgumentException("Null object present");

            var ids = list.Select(obj => obj.Id ?? throw new AnalyzerException("Id must be set"))
                    .Distinct()
                    .ToList();

            if (ids.Count != list.Count)
                throw new ArgumentException("Duplicate objects");

            var containers = Enumerable.Range(0, NumberOfContainers)
                    .Select(i => _factory.CreateContainer(list));

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