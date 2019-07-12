using System.Collections.Generic;
using static GeoGen.Core.PredefinedConstructionType;

namespace GeoGen.Core
{
    /// <summary>
    /// A static class containing concrete <see cref="ComposedConstruction"/>.
    /// </summary>
    public static class ComposedConstructions
    {
        /// <summary>
        /// Intersection of line l and other line AB (signature l, {A, B}).
        /// </summary>
        /// <returns>The construction.</returns>
        public static ComposedConstruction IntersectionOfLineAndLineFromPoints
        {
            get
            {
                // Create objects
                var l = new LooseConfigurationObject(ConfigurationObjectType.Line);
                var A = new LooseConfigurationObject(ConfigurationObjectType.Point);
                var B = new LooseConfigurationObject(ConfigurationObjectType.Point);
                var lineAB = new ConstructedConfigurationObject(LineFromPoints, A, B);
                var intersection = new ConstructedConfigurationObject(IntersectionOfLines, l, lineAB);

                // Create the actual configuration
                var configuration = Configuration.DeriveFromObjects(intersection);

                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                    new ObjectConstructionParameter(ConfigurationObjectType.Line),
                    new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 2)
                };

                // Create the actual construction
                return new ComposedConstruction(nameof(IntersectionOfLineAndLineFromPoints), configuration, parameters);
            }
        }

        /// <summary>
        /// Intersection of lines AB and CD (signature {{A, B}, {C, D}}).
        /// </summary>
        /// <returns>The construction.</returns>
        public static ComposedConstruction IntersectionOfLinesFromPoints
        {
            get
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
                var configuration = Configuration.DeriveFromObjects(intersection);

                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                     new SetConstructionParameter(new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 2), 2)
                };

                // Create the actual construction
                return new ComposedConstruction(nameof(IntersectionOfLinesFromPoints), configuration, parameters);
            }
        }

        /// <summary>
        /// Perpendicular projection of points A on line BC (signature A, {B, C}).
        /// </summary>
        /// <returns>The construction.</returns>
        public static ComposedConstruction PerpendicularProjectionOnLineFromPoints
        {
            get
            {
                // Create objects
                var A = new LooseConfigurationObject(ConfigurationObjectType.Point);
                var B = new LooseConfigurationObject(ConfigurationObjectType.Point);
                var C = new LooseConfigurationObject(ConfigurationObjectType.Point);
                var lineBC = new ConstructedConfigurationObject(LineFromPoints, B, C);
                var projection = new ConstructedConfigurationObject(PerpendicularProjection, A, lineBC);

                // Create the actual configuration
                var configuration = Configuration.DeriveFromObjects(projection);

                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                     new ObjectConstructionParameter(ConfigurationObjectType.Point),
                     new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 2)
                };

                // Create the actual construction
                return new ComposedConstruction(nameof(PerpendicularProjectionOnLineFromPoints), configuration, parameters);
            }
        }

        /// <summary>
        /// Perpendicular line to line AB passing through A (signature A, B).
        /// </summary>
        /// <returns>The construction.</returns>
        public static ComposedConstruction PerpendicularLineAtPointOfLine
        {
            get
            {
                // Create objects
                var A = new LooseConfigurationObject(ConfigurationObjectType.Point);
                var B = new LooseConfigurationObject(ConfigurationObjectType.Point);
                var lineAB = new ConstructedConfigurationObject(LineFromPoints, A, B);
                var perpendicularLine = new ConstructedConfigurationObject(PerpendicularLine, A, lineAB);

                // Create the actual configuration
                var configuration = Configuration.DeriveFromObjects(perpendicularLine);

                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                     new ObjectConstructionParameter(ConfigurationObjectType.Point),
                     new ObjectConstructionParameter(ConfigurationObjectType.Point)
                };

                // Create the actual construction
                return new ComposedConstruction(nameof(PerpendicularLineAtPointOfLine), configuration, parameters);
            }
        }

        /// <summary>
        /// Perpendicular line to line BC passing through A (signature A, {B, C}).
        /// </summary>
        /// <returns>The construction.</returns>
        public static ComposedConstruction PerpendicularLineToLineFromPoints
        {
            get
            {
                // Create objects
                var A = new LooseConfigurationObject(ConfigurationObjectType.Point);
                var B = new LooseConfigurationObject(ConfigurationObjectType.Point);
                var C = new LooseConfigurationObject(ConfigurationObjectType.Point);
                var lineBC = new ConstructedConfigurationObject(LineFromPoints, B, C);
                var perpendicularLine = new ConstructedConfigurationObject(PerpendicularLine, A, lineBC);

                // Create the actual configuration
                var configuration = Configuration.DeriveFromObjects(perpendicularLine);

                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                    new ObjectConstructionParameter(ConfigurationObjectType.Point),
                    new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 2)
                };

                // Create the actual construction
                return new ComposedConstruction(nameof(PerpendicularLineToLineFromPoints), configuration, parameters);
            }
        }

        /// <summary>
        /// Reflection of point A in line l (signature A, l).
        /// </summary>
        /// <returns>The construction.</returns>
        public static ComposedConstruction ReflectionInLine
        {
            get
            {
                // Create objects
                var A = new LooseConfigurationObject(ConfigurationObjectType.Point);
                var l = new LooseConfigurationObject(ConfigurationObjectType.Line);
                var projectionA = new ConstructedConfigurationObject(PerpendicularProjection, A, l);
                var reflection = new ConstructedConfigurationObject(PointReflection, A, projectionA);

                // Create the actual configuration
                var configuration = Configuration.DeriveFromObjects(reflection);

                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                    new ObjectConstructionParameter(ConfigurationObjectType.Point),
                    new ObjectConstructionParameter(ConfigurationObjectType.Line)
                };

                // Create the actual construction
                return new ComposedConstruction(nameof(ReflectionInLine), configuration, parameters);
            }
        }

        /// <summary>
        /// Perpendicular bisector of line segment AB (signature {A, B}).
        /// </summary>
        /// <returns>The construction</returns>
        public static ComposedConstruction PerpendicularBisector
        {
            get
            {
                // Create objects
                var A = new LooseConfigurationObject(ConfigurationObjectType.Point);
                var B = new LooseConfigurationObject(ConfigurationObjectType.Point);
                var midpointAB = new ConstructedConfigurationObject(Midpoint, A, B);
                var bisector = new ConstructedConfigurationObject(PerpendicularLineAtPointOfLine, midpointAB, A);

                // Create the actual configuration
                var configuration = Configuration.DeriveFromObjects(bisector);

                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                    new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 2)
                };

                // Create the actual construction
                return new ComposedConstruction(nameof(PerpendicularBisector), configuration, parameters);
            }
        }

        /// <summary>
        /// Line parallel to line BC passing through A (signature A, {B, C}).
        /// </summary>
        /// <returns>The construction.</returns>
        public static ComposedConstruction ParallelLineToLineFromPoints
        {
            get
            {
                // Create objects
                var A = new LooseConfigurationObject(ConfigurationObjectType.Point);
                var B = new LooseConfigurationObject(ConfigurationObjectType.Point);
                var C = new LooseConfigurationObject(ConfigurationObjectType.Point);
                var lineBC = new ConstructedConfigurationObject(LineFromPoints, B, C);
                var parallelLine = new ConstructedConfigurationObject(ParallelLine, A, lineBC);

                // Create the actual configuration
                var configuration = Configuration.DeriveFromObjects(parallelLine);

                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                    new ObjectConstructionParameter(ConfigurationObjectType.Point),
                    new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 2)
                };

                // Create the actual construction
                return new ComposedConstruction(nameof(ParallelLineToLineFromPoints), configuration, parameters);
            }
        }

        /// <summary>
        /// Reflection of point A in line BC (signature A, {B, C}).
        /// </summary>
        /// <returns>The construction.</returns>
        public static ComposedConstruction ReflectionInLineFromPoints
        {
            get
            {
                // Create objects
                var A = new LooseConfigurationObject(ConfigurationObjectType.Point);
                var B = new LooseConfigurationObject(ConfigurationObjectType.Point);
                var C = new LooseConfigurationObject(ConfigurationObjectType.Point);
                var lineBC = new ConstructedConfigurationObject(LineFromPoints, B, C);
                var reflection = new ConstructedConfigurationObject(ReflectionInLine, A, lineBC);

                // Create the actual configuration
                var configuration = Configuration.DeriveFromObjects(reflection);

                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                    new ObjectConstructionParameter(ConfigurationObjectType.Point),
                    new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 2)
                };

                // Create the actual construction
                return new ComposedConstruction(nameof(ReflectionInLineFromPoints), configuration, parameters);
            }
        }

        /// <summary>
        /// Point A' such that ABA'C is a parallelogram (signature A, {B, C}).
        /// </summary>
        /// <returns>The construction.</returns>
        public static ComposedConstruction Parallelogram
        {
            get
            {
                // Create objects
                var A = new LooseConfigurationObject(ConfigurationObjectType.Point);
                var B = new LooseConfigurationObject(ConfigurationObjectType.Point);
                var C = new LooseConfigurationObject(ConfigurationObjectType.Point);
                var midpointBC = new ConstructedConfigurationObject(Midpoint, B, C);
                var reflection = new ConstructedConfigurationObject(PointReflection, A, midpointBC);

                // Create the actual configuration
                var configuration = Configuration.DeriveFromObjects(reflection);

                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                    new ObjectConstructionParameter(ConfigurationObjectType.Point),
                    new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 2)
                };

                // Create the actual construction
                return new ComposedConstruction(nameof(Parallelogram), configuration, parameters);
            }
        }

        /// <summary>
        /// Orthocenter of triangle ABC (signature {A, B, C}).
        /// </summary>
        /// <returns>The construction.</returns>
        public static ComposedConstruction Orthocenter
        {
            get
            {
                // Create objects
                var A = new LooseConfigurationObject(ConfigurationObjectType.Point);
                var B = new LooseConfigurationObject(ConfigurationObjectType.Point);
                var C = new LooseConfigurationObject(ConfigurationObjectType.Point);
                var altitudeB = new ConstructedConfigurationObject(PerpendicularLineToLineFromPoints, B, A, C);
                var altitudeC = new ConstructedConfigurationObject(PerpendicularLineToLineFromPoints, C, A, B);
                var H = new ConstructedConfigurationObject(IntersectionOfLines, altitudeB, altitudeC);

                // Create the actual configuration
                var configuration = Configuration.DeriveFromObjects(H);

                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                    new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 3)
                };

                // Create the actual construction
                return new ComposedConstruction(nameof(Orthocenter), configuration, parameters);
            }
        }

        /// <summary>
        /// Centroid of triangle ABC (signature {A, B, C}).
        /// </summary>
        /// <returns>The construction.</returns>
        public static ComposedConstruction Centroid
        {
            get
            {
                // Create objects
                var A = new LooseConfigurationObject(ConfigurationObjectType.Point);
                var B = new LooseConfigurationObject(ConfigurationObjectType.Point);
                var C = new LooseConfigurationObject(ConfigurationObjectType.Point);
                var midpointAB = new ConstructedConfigurationObject(Midpoint, A, B);
                var midpointAC = new ConstructedConfigurationObject(Midpoint, A, C);
                var G = new ConstructedConfigurationObject(IntersectionOfLinesFromPoints, C, midpointAB, B, midpointAC);

                // Create the actual configuration
                var configuration = Configuration.DeriveFromObjects(G);

                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                    new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 3)
                };

                // Create the actual construction
                return new ComposedConstruction(nameof(Centroid), configuration, parameters);
            }
        }

        /// <summary>
        /// Incenter of triangle ABC (signature {A, B, C}).
        /// </summary>
        /// <returns>The construction.</returns>
        public static ComposedConstruction Incenter
        {
            get
            {
                // Create objects
                var A = new LooseConfigurationObject(ConfigurationObjectType.Point);
                var B = new LooseConfigurationObject(ConfigurationObjectType.Point);
                var C = new LooseConfigurationObject(ConfigurationObjectType.Point);
                var bisectorA = new ConstructedConfigurationObject(InternalAngleBisector, A, B, C);
                var bisectorB = new ConstructedConfigurationObject(InternalAngleBisector, B, A, C);
                var I = new ConstructedConfigurationObject(IntersectionOfLines, bisectorA, bisectorB);

                // Create the actual configuration
                var configuration = Configuration.DeriveFromObjects(I);

                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                    new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 3)
                };

                // Create the actual construction
                return new ComposedConstruction(nameof(Incenter), configuration, parameters);
            }
        }

        /// <summary>
        /// Incircle of triangle ABC (signature {A, B, C}).
        /// </summary>
        /// <returns>The construction.</returns>
        public static ComposedConstruction Incircle
        {
            get
            {
                // Create objects
                var A = new LooseConfigurationObject(ConfigurationObjectType.Point);
                var B = new LooseConfigurationObject(ConfigurationObjectType.Point);
                var C = new LooseConfigurationObject(ConfigurationObjectType.Point);
                var I = new ConstructedConfigurationObject(Incenter, A, B, C);
                var projection = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, I, B, C);
                var incircle = new ConstructedConfigurationObject(CircleWithCenterThroughPoint, I, projection);

                // Create the actual configuration
                var configuration = Configuration.DeriveFromObjects(incircle);

                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                     new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 3)
                };

                // Create the actual construction
                return new ComposedConstruction(nameof(Incircle), configuration, parameters);
            }
        }

        /// <summary>
        /// Circumcircle of triangle ABC (signature {A, B, C}).
        /// </summary>
        /// <returns>The construction.</returns>
        public static ComposedConstruction Circumcenter
        {
            get
            {
                // Create objects
                var A = new LooseConfigurationObject(ConfigurationObjectType.Point);
                var B = new LooseConfigurationObject(ConfigurationObjectType.Point);
                var C = new LooseConfigurationObject(ConfigurationObjectType.Point);
                var c = new ConstructedConfigurationObject(Circumcircle, A, B, C);
                var O = new ConstructedConfigurationObject(CenterOfCircle, c);

                // Create the actual configuration
                var configuration = Configuration.DeriveFromObjects(O);

                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                    new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 3)
                };

                // Create the actual construction
                return new ComposedConstruction(nameof(Circumcenter), configuration, parameters);
            }
        }
    }
}