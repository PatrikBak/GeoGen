using System;
using System.Collections.Generic;
using GeoGen.Analyzer.Constructing;
using GeoGen.Analyzer.Constructing.Constructors;
using GeoGen.Core.Configurations;

namespace GeoGen.Analyzer.Objects
{
    internal class ObjectsContainersFactory : IObjectsContainersFactory
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

            var container = new ObjectsContainer();
            var objects = _constructor.Construct(looseObjects);

            foreach (var looseObject in objects)
            {
                var result = container.Add(looseObject);

                if (result == looseObject)
                    throw new AnalyzerException("Duplicate loose objects.");
            }

            return container;
        }
    }
}