using System;
using GeoGen.Analyzer.Geometry;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions;

namespace GeoGen.Analyzer
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

        public GeometricalObject Construct(ConfigurationObject configurationObject)
        {
            if (configurationObject is LooseConfigurationObject looseObject)
                return ConstructLooseObject(looseObject);

            var constructedObject = (ConstructedConfigurationObject) configurationObject;

            var construction = constructedObject.Construction;

            var arguments = constructedObject.PassedArguments;

            if (construction is PredefinedConstruction predefinedConstruction)
            {
                var constructor = _resolver.Resolve(predefinedConstruction);

                var result = constructor.Apply(arguments);
                result.ConfigurationObject = configurationObject;

                return result;
            }

            // TODO: Composed constructions
            throw new NotImplementedException("Composed constructions are not resolved yet");
        }

        private GeometricalObject ConstructLooseObject(LooseConfigurationObject looseObject)
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