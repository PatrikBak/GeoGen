using System.Collections.Generic;
using static GeoGen.Core.ConfigurationObjectType;
using static GeoGen.Core.LooseObjectsLayout;
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
        public static ComposedConstruction IntersectionOfLineAndLineFromPoints
        {
            get
            {
                // Create objects
                var l = new LooseConfigurationObject(Line);
                var A = new LooseConfigurationObject(Point);
                var B = new LooseConfigurationObject(Point);
                var lineAB = new ConstructedConfigurationObject(LineFromPoints, A, B);
                var intersection = new ConstructedConfigurationObject(IntersectionOfLines, l, lineAB);

                // Create the actual configuration
                var configuration = Configuration.DeriveFromObjects(LineAndTwoPoints, intersection);

                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                    new ObjectConstructionParameter(Line),
                    new SetConstructionParameter(new ObjectConstructionParameter(Point), 2)
                };

                // Create the actual construction
                return new ComposedConstruction(nameof(IntersectionOfLineAndLineFromPoints), configuration, parameters);
            }
        }

        /// <summary>
        /// Intersection of lines AB and CD (signature {{A, B}, {C, D}}).
        /// </summary>
        public static ComposedConstruction IntersectionOfLinesFromPoints
        {
            get
            {
                // Create objects
                var A = new LooseConfigurationObject(Point);
                var B = new LooseConfigurationObject(Point);
                var C = new LooseConfigurationObject(Point);
                var D = new LooseConfigurationObject(Point);
                var lineAB = new ConstructedConfigurationObject(LineFromPoints, A, B);
                var lineCD = new ConstructedConfigurationObject(LineFromPoints, C, D);
                var intersection = new ConstructedConfigurationObject(IntersectionOfLines, lineAB, lineCD);

                // Create the actual configuration
                var configuration = Configuration.DeriveFromObjects(FourPoints, A, B, C, D, intersection);

                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                     new SetConstructionParameter(new SetConstructionParameter(new ObjectConstructionParameter(Point), 2), 2)
                };

                // Create the actual construction
                return new ComposedConstruction(nameof(IntersectionOfLinesFromPoints), configuration, parameters);
            }
        }

        /// <summary>
        /// Perpendicular projection of points A on line BC (signature A, {B, C}).
        /// </summary>
        public static ComposedConstruction PerpendicularProjectionOnLineFromPoints
        {
            get
            {
                // Create objects
                var A = new LooseConfigurationObject(Point);
                var B = new LooseConfigurationObject(Point);
                var C = new LooseConfigurationObject(Point);
                var lineBC = new ConstructedConfigurationObject(LineFromPoints, B, C);
                var projection = new ConstructedConfigurationObject(PerpendicularProjection, A, lineBC);

                // Create the actual configuration
                var configuration = Configuration.DeriveFromObjects(ThreePoints, A, B, C, projection);

                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                     new ObjectConstructionParameter(Point),
                     new SetConstructionParameter(new ObjectConstructionParameter(Point), 2)
                };

                // Create the actual construction
                return new ComposedConstruction(nameof(PerpendicularProjectionOnLineFromPoints), configuration, parameters);
            }
        }

        /// <summary>
        /// Perpendicular line to line AB passing through A (signature A, B).
        /// </summary>
        public static ComposedConstruction PerpendicularLineAtPointOfLine
        {
            get
            {
                // Create objects
                var A = new LooseConfigurationObject(Point);
                var B = new LooseConfigurationObject(Point);
                var lineAB = new ConstructedConfigurationObject(LineFromPoints, A, B);
                var perpendicularLine = new ConstructedConfigurationObject(PerpendicularLine, A, lineAB);

                // Create the actual configuration
                var configuration = Configuration.DeriveFromObjects(TwoPoints, A, B, perpendicularLine);

                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                     new ObjectConstructionParameter(Point),
                     new ObjectConstructionParameter(Point)
                };

                // Create the actual construction
                return new ComposedConstruction(nameof(PerpendicularLineAtPointOfLine), configuration, parameters);
            }
        }

        /// <summary>
        /// Perpendicular line to line BC passing through A (signature A, {B, C}).
        /// </summary>
        public static ComposedConstruction PerpendicularLineToLineFromPoints
        {
            get
            {
                // Create objects
                var A = new LooseConfigurationObject(Point);
                var B = new LooseConfigurationObject(Point);
                var C = new LooseConfigurationObject(Point);
                var lineBC = new ConstructedConfigurationObject(LineFromPoints, B, C);
                var perpendicularLine = new ConstructedConfigurationObject(PerpendicularLine, A, lineBC);

                // Create the actual configuration
                var configuration = Configuration.DeriveFromObjects(ThreePoints, A, B, C, perpendicularLine);

                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                    new ObjectConstructionParameter(Point),
                    new SetConstructionParameter(new ObjectConstructionParameter(Point), 2)
                };

                // Create the actual construction
                return new ComposedConstruction(nameof(PerpendicularLineToLineFromPoints), configuration, parameters);
            }
        }

        /// <summary>
        /// Reflection of point A in line l (signature l, A).
        /// </summary>
        public static ComposedConstruction ReflectionInLine
        {
            get
            {
                // Create objects
                var l = new LooseConfigurationObject(Line);
                var A = new LooseConfigurationObject(Point);
                var projectionA = new ConstructedConfigurationObject(PerpendicularProjection, A, l);
                var reflection = new ConstructedConfigurationObject(PointReflection, A, projectionA);

                // Create the actual configuration
                var configuration = Configuration.DeriveFromObjects(LineAndPoint, l, A, reflection);

                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                    new ObjectConstructionParameter(Line),
                    new ObjectConstructionParameter(Point)
                };

                // Create the actual construction
                return new ComposedConstruction(nameof(ReflectionInLine), configuration, parameters);
            }
        }

        /// <summary>
        /// Reflection of point A in line BC (signature A, {B, C}).
        /// </summary>
        public static ComposedConstruction ReflectionInLineFromPoints
        {
            get
            {
                // Create objects
                var A = new LooseConfigurationObject(Point);
                var B = new LooseConfigurationObject(Point);
                var C = new LooseConfigurationObject(Point);
                var lineBC = new ConstructedConfigurationObject(LineFromPoints, B, C);
                var reflection = new ConstructedConfigurationObject(ReflectionInLine, lineBC, A);

                // Create the actual configuration
                var configuration = Configuration.DeriveFromObjects(ThreePoints, A, B, C, reflection);

                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                    new ObjectConstructionParameter(Point),
                    new SetConstructionParameter(new ObjectConstructionParameter(Point), 2)
                };

                // Create the actual construction
                return new ComposedConstruction(nameof(ReflectionInLineFromPoints), configuration, parameters);
            }
        }

        /// <summary>
        /// Perpendicular bisector of line segment AB (signature {A, B}).
        /// </summary>
        public static ComposedConstruction PerpendicularBisector
        {
            get
            {
                // Create objects
                var A = new LooseConfigurationObject(Point);
                var B = new LooseConfigurationObject(Point);
                var midpointAB = new ConstructedConfigurationObject(Midpoint, A, B);
                var bisector = new ConstructedConfigurationObject(PerpendicularLineAtPointOfLine, midpointAB, A);

                // Create the actual configuration
                var configuration = Configuration.DeriveFromObjects(TwoPoints, bisector);

                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                    new SetConstructionParameter(new ObjectConstructionParameter(Point), 2)
                };

                // Create the actual construction
                return new ComposedConstruction(nameof(PerpendicularBisector), configuration, parameters);
            }
        }

        /// <summary>
        /// Line parallel to line BC passing through A (signature A, {B, C}).
        /// </summary>
        public static ComposedConstruction ParallelLineToLineFromPoints
        {
            get
            {
                // Create objects
                var A = new LooseConfigurationObject(Point);
                var B = new LooseConfigurationObject(Point);
                var C = new LooseConfigurationObject(Point);
                var lineBC = new ConstructedConfigurationObject(LineFromPoints, B, C);
                var parallelLine = new ConstructedConfigurationObject(ParallelLine, A, lineBC);

                // Create the actual configuration
                var configuration = Configuration.DeriveFromObjects(ThreePoints, A, B, C, parallelLine);

                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                    new ObjectConstructionParameter(Point),
                    new SetConstructionParameter(new ObjectConstructionParameter(Point), 2)
                };

                // Create the actual construction
                return new ComposedConstruction(nameof(ParallelLineToLineFromPoints), configuration, parameters);
            }
        }

        /// <summary>
        /// Reflection of point A in the midpoint of BC (signature A, {B, C}).
        /// </summary>
        public static ComposedConstruction ReflectionInMidpointOfSegment
        {
            get
            {
                // Create objects
                var A = new LooseConfigurationObject(Point);
                var B = new LooseConfigurationObject(Point);
                var C = new LooseConfigurationObject(Point);
                var midpointBC = new ConstructedConfigurationObject(Midpoint, B, C);
                var reflection = new ConstructedConfigurationObject(PointReflection, A, midpointBC);

                // Create the actual configuration
                var configuration = Configuration.DeriveFromObjects(ThreeArbitraryPoints, A, B, C, reflection);

                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                    new ObjectConstructionParameter(Point),
                    new SetConstructionParameter(new ObjectConstructionParameter(Point), 2)
                };

                // Create the actual construction
                return new ComposedConstruction(nameof(ReflectionInMidpointOfSegment), configuration, parameters);
            }
        }

        /// <summary>
        /// Orthocenter of triangle ABC (signature {A, B, C}).
        /// </summary>
        public static ComposedConstruction Orthocenter
        {
            get
            {
                // Create objects
                var A = new LooseConfigurationObject(Point);
                var B = new LooseConfigurationObject(Point);
                var C = new LooseConfigurationObject(Point);
                var altitudeB = new ConstructedConfigurationObject(PerpendicularLineToLineFromPoints, B, A, C);
                var altitudeC = new ConstructedConfigurationObject(PerpendicularLineToLineFromPoints, C, A, B);
                var H = new ConstructedConfigurationObject(IntersectionOfLines, altitudeB, altitudeC);

                // Create the actual configuration
                var configuration = Configuration.DeriveFromObjects(ThreePoints, H);

                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                    new SetConstructionParameter(new ObjectConstructionParameter(Point), 3)
                };

                // Create the actual construction
                return new ComposedConstruction(nameof(Orthocenter), configuration, parameters);
            }
        }

        /// <summary>
        /// Centroid of triangle ABC (signature {A, B, C}).
        /// </summary>
        public static ComposedConstruction Centroid
        {
            get
            {
                // Create objects
                var A = new LooseConfigurationObject(Point);
                var B = new LooseConfigurationObject(Point);
                var C = new LooseConfigurationObject(Point);
                var midpointAB = new ConstructedConfigurationObject(Midpoint, A, B);
                var midpointAC = new ConstructedConfigurationObject(Midpoint, A, C);
                var G = new ConstructedConfigurationObject(IntersectionOfLinesFromPoints, C, midpointAB, B, midpointAC);

                // Create the actual configuration
                var configuration = Configuration.DeriveFromObjects(ThreePoints, G);

                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                    new SetConstructionParameter(new ObjectConstructionParameter(Point), 3)
                };

                // Create the actual construction
                return new ComposedConstruction(nameof(Centroid), configuration, parameters);
            }
        }

        /// <summary>
        /// Incenter of triangle ABC (signature {A, B, C}).
        /// </summary>
        public static ComposedConstruction Incenter
        {
            get
            {
                // Create objects
                var A = new LooseConfigurationObject(Point);
                var B = new LooseConfigurationObject(Point);
                var C = new LooseConfigurationObject(Point);
                var bisectorA = new ConstructedConfigurationObject(InternalAngleBisector, A, B, C);
                var bisectorB = new ConstructedConfigurationObject(InternalAngleBisector, B, A, C);
                var I = new ConstructedConfigurationObject(IntersectionOfLines, bisectorA, bisectorB);

                // Create the actual configuration
                var configuration = Configuration.DeriveFromObjects(ThreePoints, I);

                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                    new SetConstructionParameter(new ObjectConstructionParameter(Point), 3)
                };

                // Create the actual construction
                return new ComposedConstruction(nameof(Incenter), configuration, parameters);
            }
        }

        /// <summary>
        /// Incircle of triangle ABC (signature {A, B, C}).
        /// </summary>
        public static ComposedConstruction Incircle
        {
            get
            {
                // Create objects
                var A = new LooseConfigurationObject(Point);
                var B = new LooseConfigurationObject(Point);
                var C = new LooseConfigurationObject(Point);
                var I = new ConstructedConfigurationObject(Incenter, A, B, C);
                var projection = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, I, B, C);
                var incircle = new ConstructedConfigurationObject(CircleWithCenterThroughPoint, I, projection);

                // Create the actual configuration
                var configuration = Configuration.DeriveFromObjects(ThreePoints, incircle);

                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                     new SetConstructionParameter(new ObjectConstructionParameter(Point), 3)
                };

                // Create the actual construction
                return new ComposedConstruction(nameof(Incircle), configuration, parameters);
            }
        }

        /// <summary>
        /// Circumcircle of triangle ABC (signature {A, B, C}).
        /// </summary>
        public static ComposedConstruction Circumcenter
        {
            get
            {
                // Create objects
                var A = new LooseConfigurationObject(Point);
                var B = new LooseConfigurationObject(Point);
                var C = new LooseConfigurationObject(Point);
                var c = new ConstructedConfigurationObject(Circumcircle, A, B, C);
                var O = new ConstructedConfigurationObject(CenterOfCircle, c);

                // Create the actual configuration
                var configuration = Configuration.DeriveFromObjects(ThreePoints, O);

                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                    new SetConstructionParameter(new ObjectConstructionParameter(Point), 3)
                };

                // Create the actual construction
                return new ComposedConstruction(nameof(Circumcenter), configuration, parameters);
            }
        }

        /// <summary>
        /// The midpoint of arc opposite to BAC (signature A, {B, C}).
        /// </summary>
        public static ComposedConstruction MidpointOfOppositeArc
        {
            get
            {
                // Create objects
                var A = new LooseConfigurationObject(Point);
                var B = new LooseConfigurationObject(Point);
                var C = new LooseConfigurationObject(Point);
                var I = new ConstructedConfigurationObject(Incenter, A, B, C);
                var M = new ConstructedConfigurationObject(Circumcenter, B, C, I);

                // Create the actual configuration
                var configuration = Configuration.DeriveFromObjects(ThreePoints, A, B, C, M);

                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                    new ObjectConstructionParameter(Point),
                    new SetConstructionParameter(new ObjectConstructionParameter(Point), 2)
                };

                // Create the actual construction
                return new ComposedConstruction(nameof(MidpointOfOppositeArc), configuration, parameters);
            }
        }

        /// <summary>
        /// The A-excenter of triangle ABC (signature A, {B, C}).
        /// </summary>
        public static ComposedConstruction Excenter
        {
            get
            {
                // Create objects
                var A = new LooseConfigurationObject(Point);
                var B = new LooseConfigurationObject(Point);
                var C = new LooseConfigurationObject(Point);
                var I = new ConstructedConfigurationObject(Incenter, A, B, C);
                var M = new ConstructedConfigurationObject(MidpointOfOppositeArc, A, B, C);
                var J = new ConstructedConfigurationObject(PointReflection, I, M);

                // Create the actual configuration
                var configuration = Configuration.DeriveFromObjects(ThreePoints, A, B, C, J);

                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                    new ObjectConstructionParameter(Point),
                    new SetConstructionParameter(new ObjectConstructionParameter(Point), 2)
                };

                // Create the actual construction
                return new ComposedConstruction(nameof(Excenter), configuration, parameters);
            }
        }

        /// <summary>
        /// The A-excircle of triangle ABC (signature A, {B, C}).
        /// </summary>
        public static ComposedConstruction Excircle
        {
            get
            {
                // Create objects
                var A = new LooseConfigurationObject(Point);
                var B = new LooseConfigurationObject(Point);
                var C = new LooseConfigurationObject(Point);
                var J = new ConstructedConfigurationObject(Excenter, A, B, C);
                var D = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, J, B, C);
                var c = new ConstructedConfigurationObject(CircleWithCenterThroughPoint, J, D);

                // Create the actual configuration
                var configuration = Configuration.DeriveFromObjects(ThreePoints, A, B, C, c);

                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                    new ObjectConstructionParameter(Point),
                    new SetConstructionParameter(new ObjectConstructionParameter(Point), 2)
                };

                // Create the actual construction
                return new ComposedConstruction(nameof(Excircle), configuration, parameters);
            }
        }

        /// <summary>
        /// The circle with a given diameter (signature {A, B}).
        /// </summary>
        public static ComposedConstruction CircleWithDiameter
        {
            get
            {
                // Create objects
                var A = new LooseConfigurationObject(Point);
                var B = new LooseConfigurationObject(Point);
                var M = new ConstructedConfigurationObject(Midpoint, A, B);
                var c = new ConstructedConfigurationObject(CircleWithCenterThroughPoint, M, A);

                // Create the actual configuration
                var configuration = Configuration.DeriveFromObjects(TwoPoints, A, B, c);

                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                    new SetConstructionParameter(new ObjectConstructionParameter(Point), 2)
                };

                // Create the actual construction
                return new ComposedConstruction(nameof(CircleWithDiameter), configuration, parameters);
            }
        }

        /// <summary>
        /// The A-midline of triangle ABC (signature A, {B, C}).
        /// </summary>
        public static ComposedConstruction Midline
        {
            get
            {
                // Create objects
                var A = new LooseConfigurationObject(Point);
                var B = new LooseConfigurationObject(Point);
                var C = new LooseConfigurationObject(Point);
                var D = new ConstructedConfigurationObject(Midpoint, A, B);
                var E = new ConstructedConfigurationObject(Midpoint, A, C);
                var l = new ConstructedConfigurationObject(LineFromPoints, D, E);

                // Create the actual configuration
                var configuration = Configuration.DeriveFromObjects(ThreePoints, A, B, C, l);

                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                    new ObjectConstructionParameter(Point),
                    new SetConstructionParameter(new ObjectConstructionParameter(Point), 2)
                };

                // Create the actual construction
                return new ComposedConstruction(nameof(Midline), configuration, parameters);
            }
        }

        /// <summary>
        /// The A-median of triangle ABC (signature A, {B, C}).
        /// </summary>
        public static ComposedConstruction Median
        {
            get
            {
                // Create objects
                var A = new LooseConfigurationObject(Point);
                var B = new LooseConfigurationObject(Point);
                var C = new LooseConfigurationObject(Point);
                var D = new ConstructedConfigurationObject(Midpoint, B, C);
                var l = new ConstructedConfigurationObject(LineFromPoints, A, D);

                // Create the actual configuration
                var configuration = Configuration.DeriveFromObjects(ThreePoints, A, B, C, l);

                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                    new ObjectConstructionParameter(Point),
                    new SetConstructionParameter(new ObjectConstructionParameter(Point), 2)
                };

                // Create the actual construction
                return new ComposedConstruction(nameof(Median), configuration, parameters);
            }
        }
    }
}