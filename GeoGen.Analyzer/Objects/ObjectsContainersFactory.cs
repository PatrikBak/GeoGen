using System;
using System.Collections.Generic;
using GeoGen.Analyzer.Constructing;
using GeoGen.Core.Configurations;

namespace GeoGen.Analyzer.Objects
{
    internal class ObjectsContainersFactory : IObjectsContainersFactory
    {
        private readonly IObjectsConstructor _constructor;

        public ObjectsContainersFactory(IObjectsConstructor constructor)
        {
            _constructor = constructor;
        }

        public IObjectsContainer CreateContainer(IEnumerable<LooseConfigurationObject> looseObjects)
        {
            if (looseObjects == null)
                throw new ArgumentNullException(nameof(looseObjects));

            var container = new ObjectsContainer();

            foreach (var looseObject in looseObjects)
            {
                var geometricalObject = _constructor.Construct(looseObject);

                var result = container.Add(geometricalObject);

                if (result == geometricalObject)
                    throw new AnalyzerException("Duplicate loose objects.");
            }

            return container;
        }
    }
}