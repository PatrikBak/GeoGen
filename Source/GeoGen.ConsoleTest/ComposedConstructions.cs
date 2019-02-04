using GeoGen.Core;
using System.Collections.Generic;
using static GeoGen.Core.PredefinedConstructionType;

namespace GeoGen.ConsoleTest
{
    public static class ComposedConstructions
    {
        public static ComposedConstruction ReflectionInLineFromPoints()
        {
            var A = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var B = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var C = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var altitudeA = new ConstructedConfigurationObject(PerpendicularLineFromPoints, A, B, C);
            var feetA = new ConstructedConfigurationObject(IntersectionOfLinesFromLineAndPoints, altitudeA, B, C);
            var reflection = new ConstructedConfigurationObject(PointReflection, A, feetA);

            var configuration = new Configuration(A, B, C, altitudeA, feetA, reflection);

            var parameters = new List<ConstructionParameter>
            {
                new ObjectConstructionParameter(ConfigurationObjectType.Point),
                new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 2)
            };

            return new ComposedConstruction("ReflectionInLineFromPoints", configuration, parameters);
        }

        public static ComposedConstruction Parallelogram()
        {
            var A = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var B = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var C = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var midpointBC = new ConstructedConfigurationObject(MidpointFromPoints, B, C);
            var D = new ConstructedConfigurationObject(PointReflection, A, midpointBC);

            var configuration = new Configuration(A, B, C, midpointBC, D);

            var parameters = new List<ConstructionParameter>
            {
                new ObjectConstructionParameter(ConfigurationObjectType.Point),
                new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 2)
            };

            return new ComposedConstruction("Parallelogram", configuration, parameters);
        }

        public static ComposedConstruction OrthocenterFromPoints()
        {
            var A = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var B = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var C = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var altitudeB = new ConstructedConfigurationObject(PerpendicularLineFromPoints, B, A, C);
            var altitudeC = new ConstructedConfigurationObject(PerpendicularLineFromPoints, C, A, B);
            var H = new ConstructedConfigurationObject(IntersectionOfLines, altitudeB, altitudeC);

            var configuration = new Configuration(A, B, C, altitudeB, altitudeC, H);

            var parameters = new List<ConstructionParameter>
            {
                new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 3)
            };

            return new ComposedConstruction("Orthocenter", configuration, parameters);
        }

        public static ComposedConstruction CentroidFromPoints()
        {
            var A = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var B = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var C = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var midpointAB = new ConstructedConfigurationObject(MidpointFromPoints, A, B);
            var midpointAC = new ConstructedConfigurationObject(MidpointFromPoints, A, C);
            var G = new ConstructedConfigurationObject(IntersectionOfLinesFromPoints, C, midpointAB, B, midpointAC);

            var configuration = new Configuration(A, B, C, midpointAB, midpointAC, G);

            var parameters = new List<ConstructionParameter>
            {
                new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 3)
            };

            return new ComposedConstruction("Centroid", configuration, parameters);
        }

        public static ComposedConstruction IncenterFromPoints()
        {
            var A = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var B = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var C = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var bisectorA = new ConstructedConfigurationObject(InternalAngleBisectorFromPoints, A, B, C);
            var bisectorB = new ConstructedConfigurationObject(InternalAngleBisectorFromPoints, B, A, C);
            var I = new ConstructedConfigurationObject(IntersectionOfLines, bisectorA, bisectorB);

            var configuration = new Configuration(A, B, C, bisectorA, bisectorB, I);

            var parameters = new List<ConstructionParameter>
            {
                new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 3)
            };

            return new ComposedConstruction("Incenter", configuration, parameters);
        }
    }
}