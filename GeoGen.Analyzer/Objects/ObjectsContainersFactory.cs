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
            _constructor = constructor;
        }

        public IObjectsContainer CreateContainer(IEnumerable<LooseConfigurationObject> looseObjects)
        {
            if (looseObjects == null)
                throw new ArgumentNullException(nameof(looseObjects));

            var looseObjectsList = looseObjects.ToList();
            var container = new ObjectsContainer();

            var objects = _constructor.Construct(looseObjectsList);

            for (var i = 0; i < looseObjectsList.Count; i++)
            {
                var configurationObject = looseObjectsList[i];
                var analyticalObject = objects[i];

                var result = container.Add(analyticalObject, configurationObject);

                if (result == null)
                    throw new AnalyzerException("Duplicate loose objects.");
            }

            return container;
        }
    }
}