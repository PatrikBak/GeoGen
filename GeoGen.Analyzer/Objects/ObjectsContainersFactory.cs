using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Analyzer.Constructing;
using GeoGen.Core.Configurations;

namespace GeoGen.Analyzer.Objects
{
    internal sealed class ObjectsContainersFactory : IObjectsContainersFactory
    {
        private readonly ILooseObjectsConstructor _constructor;

        public ObjectsContainersFactory(ILooseObjectsConstructor constructor)
        {
            _constructor = constructor ?? throw new ArgumentNullException(nameof(constructor));
        }

        public IObjectsContainer CreateContainer(IEnumerable<LooseConfigurationObject> looseObjects)
        {
            if (looseObjects == null)
                throw new ArgumentNullException(nameof(looseObjects));

            var looseObjectsList = looseObjects.ToList();

            if (looseObjectsList.Contains(null))
                throw new ArgumentException("Loose objects contain null");

            var ids = looseObjectsList
                    .Select(obj => obj.Id ?? throw new AnalyzerException("Id must be set"))
                    .Distinct()
                    .ToList();

            if (ids.Count != looseObjectsList.Count)
                throw new ArgumentException("Duplicate loose objects");

            var container = new ObjectsContainer();

            var objects = _constructor.Construct(looseObjectsList);

            for (var i = 0; i < looseObjectsList.Count; i++)
            {
                var configurationObject = looseObjectsList[i];
                var analyticalObject = objects[i];

                container.Add(analyticalObject, configurationObject);
            }

            return container;
        }
    }
}