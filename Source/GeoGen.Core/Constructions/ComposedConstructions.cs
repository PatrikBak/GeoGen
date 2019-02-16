using System.Collections.Generic;
using static GeoGen.Core.PredefinedConstructionType;

namespace GeoGen.Core
{
    /// <summary>
    /// A static class containing concrete <see cref="ComposedConstruction"/>s.
    /// </summary>
    public static class ComposedConstructions
    {
        /// <summary>
        /// Intersection of line l and other line AB (signature l, {A, B}).
        /// </summary>
        /// <returns>The construction.</returns>
        public static ComposedConstruction IntersectionOfLineAndLineFromPoints()
        {
            // Create objects
            var l = new LooseConfigurationObject(ConfigurationObjectType.Line);
            var A = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var B = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var lineAB = new ConstructedConfigurationObject(LineFromPoints, A, B);
            var intersection = new ConstructedConfigurationObject(IntersectionOfLines, l, lineAB);

            // Create the actual configuration
            var configuration = new Configuration(l, A, B, lineAB, intersection);

            // Create the parameters
            var parameters = new List<ConstructionParameter>
            {
                new ObjectConstructionParameter(ConfigurationObjectType.Line),
                new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 2)
            };

            // Create the actual construction
            return new ComposedConstruction("IntersectionOfLineAndLineFromPoints", configuration, parameters);
        }

        /// <summary>
        /// Intersection of lines AB and CD (signature {{A, B}, {C, D}}).
        /// </summary>
        /// <returns>The construction.</returns>
        public static ComposedConstruction IntersectionOfLinesFromPoints()
        {
            // Create objects
            var A = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var B = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var C = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var D = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var lineAB = new ConstructedConfigurationObject(LineFromPoints, A, B);
            var lineCD = new ConstructedConfigurationObject(LineFromPoints, C, D);
            var intersection = new ConstructedConfigurationObject(IntersectionOfLines, lineAB, lineCD);

            // Create the actual configuration
            var configuration = new Configuration(A, B, C, D, lineAB, lineCD, intersection);

            // Create the parameters
            var parameters = new List<ConstructionParameter>
            {
                 new SetConstructionParameter(new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 2), 2)
            };

            // Create the actual construction
            return new ComposedConstruction("IntersectionOfLinesFromPoints", configuration, parameters);
        }

        /// <summary>
        /// Perpendicular line to line AB passing through A (signature A, B).
        /// </summary>
        /// <returns>The construction.</returns>
        public static ComposedConstruction PerpendicularLineAtPointOfLine()
        {
            // Create objects
            var A = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var B = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var lineAB = new ConstructedConfigurationObject(LineFromPoints, A, B);
            var perpendicularLine = new ConstructedConfigurationObject(PerpendicularLine, A, lineAB);

            // Create the actual configuration
            var configuration = new Configuration(A, B, lineAB, perpendicularLine);

            // Create the parameters
            var parameters = new List<ConstructionParameter>
            {
                 new ObjectConstructionParameter(ConfigurationObjectType.Point),
                 new ObjectConstructionParameter(ConfigurationObjectType.Point)
            };

            // Create the actual construction
            return new ComposedConstruction("PerpendicularLineAtPointOfLine", configuration, parameters);
        }

        /// <summary>
        /// Perpendicular line to line BC passing through A (signature A, {B, C}).
        /// </summary>
        /// <returns>The construction.</returns>
        public static ComposedConstruction PerpendicularLineToLineFromPoints()
        {
            // Create objects
            var A = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var B = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var C = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var lineBC = new ConstructedConfigurationObject(LineFromPoints, B, C);
            var perpendicularLine = new ConstructedConfigurationObject(PerpendicularLine, A, lineBC);

            // Create the actual configuration
            var configuration = new Configuration(A, B, C, lineBC, perpendicularLine);

            // Create the parameters
            var parameters = new List<ConstructionParameter>
            {
                new ObjectConstructionParameter(ConfigurationObjectType.Point),
                new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 2)
            };

            // Create the actual construction
            return new ComposedConstruction("PerpendicularLineToLineFromPoints", configuration, parameters);
        }

        /// <summary>
        /// Reflection of point A in line l (signature A, l).
        /// </summary>
        /// <returns>The construction.</returns>
        public static ComposedConstruction ReflectionInLine()
        {
            // Create objects
            var A = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var l = new LooseConfigurationObject(ConfigurationObjectType.Line);
            var projectionA = new ConstructedConfigurationObject(PerpendicularProjection, A, l);
            var reflection = new ConstructedConfigurationObject(PointReflection, A, projectionA);

            // Create the actual configuration
            var configuration = new Configuration(A, l, projectionA, reflection);

            // Create the parameters
            var parameters = new List<ConstructionParameter>
            {
                new ObjectConstructionParameter(ConfigurationObjectType.Point),
                new ObjectConstructionParameter(ConfigurationObjectType.Line)
            };

            // Create the actual construction
            return new ComposedConstruction("ReflectionInLine", configuration, parameters);
        }

        /// <summary>
        /// Reflection of point A in line BC (signature A, {B, C}).
        /// </summary>
        /// <returns>The construction.</returns>
        public static ComposedConstruction ReflectionInLineFromPoints()
        {
            // Create objects
            var A = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var B = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var C = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var lineBC = new ConstructedConfigurationObject(LineFromPoints, B, C);
            var reflection = new ConstructedConfigurationObject(ReflectionInLine(), A, lineBC);

            // Create the actual configuration
            var configuration = new Configuration(A, B, C, lineBC, reflection);

            // Create the parameters
            var parameters = new List<ConstructionParameter>
            {
                new ObjectConstructionParameter(ConfigurationObjectType.Point),
                new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 2)
            };

            // Create the actual construction
            return new ComposedConstruction("ReflectionInLineFromPoints", configuration, parameters);
        }

        /// <summary>
        /// Point A' such that ABA'C is a parallelogram (signature A, {B, C}).
        /// </summary>
        /// <returns>The construction.</returns>
        public static ComposedConstruction Parallelogram()
        {
            // Create objects
            var A = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var B = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var C = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var midpointBC = new ConstructedConfigurationObject(Midpoint, B, C);
            var reflection = new ConstructedConfigurationObject(PointReflection, A, midpointBC);

            // Create the actual configuration
            var configuration = new Configuration(A, B, C, midpointBC, reflection);

            // Create the parameters
            var parameters = new List<ConstructionParameter>
            {
                new ObjectConstructionParameter(ConfigurationObjectType.Point),
                new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 2)
            };

            // Create the actual construction
            return new ComposedConstruction("Parallelogram", configuration, parameters);
        }

        /// <summary>
        /// Orthocenter of triangle ABC (signature {A, B, C}).
        /// </summary>
        /// <returns>The construction.</returns>
        public static ComposedConstruction Orthocenter()
        {
            // Create objects
            var A = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var B = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var C = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var altitudeB = new ConstructedConfigurationObject(PerpendicularLineToLineFromPoints(), B, A, C);
            var altitudeC = new ConstructedConfigurationObject(PerpendicularLineToLineFromPoints(), C, A, B);
            var H = new ConstructedConfigurationObject(IntersectionOfLines, altitudeB, altitudeC);

            // Create the actual configuration
            var configuration = new Configuration(A, B, C, altitudeB, altitudeC, H);

            // Create the parameters
            var parameters = new List<ConstructionParameter>
            {
                new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 3)
            };

            // Create the actual construction
            return new ComposedConstruction("Orthocenter", configuration, parameters);
        }

        /// <summary>
        /// Centroid of triangle ABC (signature {A, B, C}).
        /// </summary>
        /// <returns>The construction.</returns>
        public static ComposedConstruction Centroid()
        {
            // Create objects
            var A = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var B = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var C = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var midpointAB = new ConstructedConfigurationObject(Midpoint, A, B);
            var midpointAC = new ConstructedConfigurationObject(Midpoint, A, C);
            var G = new ConstructedConfigurationObject(IntersectionOfLinesFromPoints(), C, midpointAB, B, midpointAC);

            // Create the actual configuration
            var configuration = new Configuration(A, B, C, midpointAB, midpointAC, G);

            // Create the parameters
            var parameters = new List<ConstructionParameter>
            {
                new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 3)
            };

            // Create the actual construction
            return new ComposedConstruction("Centroid", configuration, parameters);
        }

        /// <summary>
        /// Incenter of triangle ABC (signature {A, B, C}).
        /// </summary>
        /// <returns>The construction.</returns>
        public static ComposedConstruction Incenter()
        {
            // Create objects
            var A = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var B = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var C = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var bisectorA = new ConstructedConfigurationObject(InternalAngleBisector, A, B, C);
            var bisectorB = new ConstructedConfigurationObject(InternalAngleBisector, B, A, C);
            var I = new ConstructedConfigurationObject(IntersectionOfLines, bisectorA, bisectorB);

            // Create the actual configuration
            var configuration = new Configuration(A, B, C, bisectorA, bisectorB, I);

            // Create the parameters
            var parameters = new List<ConstructionParameter>
            {
                new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 3)
            };

            // Create the actual construction
            return new ComposedConstruction("Incenter", configuration, parameters);
        }
    }
}
