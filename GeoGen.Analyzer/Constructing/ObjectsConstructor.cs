using System;
using System.Collections.Generic;
using GeoGen.Analyzer.AnalyticalGeometry;
using GeoGen.Analyzer.Objects;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions;
using GeoGen.Core.Utilities;

namespace GeoGen.Analyzer.Constructing
{
    internal class ObjectsConstructor : IObjectsConstructor
    {
        private readonly IRandomObjectsProvider _provider;

        private readonly IConstructorsResolver _resolver;

        public ObjectsConstructor(IRandomObjectsProvider provider, IConstructorsResolver resolver)
        {
            _provider = provider;
            _resolver = resolver;
        }

        public ConstructorOutput Construct(List<ConstructedConfigurationObject> constructedObjects, IObjectsContainer container)
        {
            if (constructedObjects == null)
                throw new ArgumentNullException(nameof(constructedObjects));

            if (container == null)
                throw new ArgumentNullException(nameof(container));

            if (constructedObjects.Empty())
                throw new ArgumentException("Constructed objects can't be empty");

            // It's assumed that these constructed objects differs only by indices
            var construction = constructedObjects[0].Construction;

            var arguments = constructedObjects[0].PassedArguments;

            if (construction is PredefinedConstruction predefinedConstruction)
            {
                var constructor = _resolver.Resolve(predefinedConstruction);

                var result = constructor.Apply(arguments, container);

                if (result == null)
                    return null;

                var objects = result.Objects;

                if (objects.Count != constructedObjects.Count)
                    throw new AnalyzerException("Constructor output has incorrect number of objects");

                for (var i = 0; i < objects.Count; i++)
                {
                    objects[i].ConfigurationObject = constructedObjects[i];
                }

                return result;
            }

            throw new NotImplementedException();
        }

        public GeometricalObject Construct(LooseConfigurationObject looseObject)
        {
            GeometricalObject result;

            switch (looseObject.ObjectType)
            {
                case ConfigurationObjectType.Point:
                    result = _provider.NextRandomObject<Point>();
                    break;
                case ConfigurationObjectType.Line:
                    result = _provider.NextRandomObject<Line>();
                    break;
                case ConfigurationObjectType.Circle:
                    result = _provider.NextRandomObject<Circle>();
                    break;
                default:
                    throw new AnalyzerException("Unhandled case.");
            }

            result.ConfigurationObject = looseObject;

            return result;
        }
    }
}