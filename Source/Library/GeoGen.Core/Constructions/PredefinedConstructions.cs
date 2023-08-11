using static GeoGen.Core.ConfigurationObjectType;

namespace GeoGen.Core
{
    /// <summary>
    /// A static class for creating <see cref="PredefinedConstruction"/>s.
    /// </summary>
    public static class PredefinedConstructions
    {
        /// <summary>
        /// Creates a <see cref="PredefinedConstructionType.CenterOfCircle"/> construction.
        /// </summary>
        /// <returns>The construction.</returns>
        public static PredefinedConstruction CenterOfCircle
        {
            get
            {
                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                    new ObjectConstructionParameter(Circle)
                };

                // Create the actual construction
                return new PredefinedConstruction(PredefinedConstructionType.CenterOfCircle, parameters, Point);
            }
        }

        /// <summary>
        /// Creates a <see cref="PredefinedConstructionType.Circumcircle"/> construction.
        /// </summary>
        /// <returns>The construction.</returns>
        public static PredefinedConstruction Circumcircle
        {
            get
            {
                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                    new SetConstructionParameter(new ObjectConstructionParameter(Point), 3)
                };

                // Create the actual construction
                return new PredefinedConstruction(PredefinedConstructionType.Circumcircle, parameters, Circle);
            }
        }

        /// <summary>
        /// Creates a <see cref="PredefinedConstructionType.CircleWithCenterThroughPoint"/> construction.
        /// </summary>
        /// <returns>The construction.</returns>
        public static PredefinedConstruction CircleWithCenterThroughPoint
        {
            get
            {
                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                    new ObjectConstructionParameter(Point),
                    new ObjectConstructionParameter(Point),
                };

                // Create the actual construction
                return new PredefinedConstruction(PredefinedConstructionType.CircleWithCenterThroughPoint, parameters, Circle);
            }
        }

        /// <summary>
        /// Creates a <see cref="PredefinedConstructionType.InternalAngleBisector"/> construction.
        /// </summary>
        /// <returns>The construction.</returns>
        public static PredefinedConstruction InternalAngleBisector
        {
            get
            {
                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                    new ObjectConstructionParameter(Point),
                    new SetConstructionParameter(new ObjectConstructionParameter(Point), 2)
                };

                // Create the actual construction
                return new PredefinedConstruction(PredefinedConstructionType.InternalAngleBisector, parameters, Line);
            }
        }

        /// <summary>
        /// Creates a <see cref="PredefinedConstructionType.IntersectionOfLines"/> construction.
        /// </summary>
        /// <returns>The construction.</returns>
        public static PredefinedConstruction IntersectionOfLines
        {
            get
            {
                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                    new SetConstructionParameter(new ObjectConstructionParameter(Line), 2)
                };

                // Create the actual construction
                return new PredefinedConstruction(PredefinedConstructionType.IntersectionOfLines, parameters, Point);
            }
        }

        /// <summary>
        /// Creates a <see cref="PredefinedConstructionType.LineFromPoints"/> construction.
        /// </summary>
        /// <returns>The construction.</returns>
        public static PredefinedConstruction LineFromPoints
        {
            get
            {
                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                    new SetConstructionParameter(new ObjectConstructionParameter(Point), 2)
                };

                // Create the actual construction
                return new PredefinedConstruction(PredefinedConstructionType.LineFromPoints, parameters, Line);
            }
        }

        /// <summary>
        /// Creates a <see cref="PredefinedConstructionType.Midpoint"/> construction.
        /// </summary>
        /// <returns>The construction.</returns>
        public static PredefinedConstruction Midpoint
        {
            get
            {
                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                    new SetConstructionParameter(new ObjectConstructionParameter(Point), 2)
                };

                // Create the actual construction
                return new PredefinedConstruction(PredefinedConstructionType.Midpoint, parameters, Point);
            }
        }

        /// <summary>
        /// Creates a <see cref="PredefinedConstructionType.PerpendicularProjection"/> construction.
        /// </summary>
        /// <returns>The construction.</returns>
        public static PredefinedConstruction PerpendicularProjection
        {
            get
            {
                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                    new ObjectConstructionParameter(Point),
                    new ObjectConstructionParameter(Line)
                };

                // Create the actual construction
                return new PredefinedConstruction(PredefinedConstructionType.PerpendicularProjection, parameters, Point);
            }
        }

        /// <summary>
        /// Creates a <see cref="PredefinedConstructionType.PerpendicularLine"/> construction.
        /// </summary>
        /// <returns>The construction.</returns>
        public static PredefinedConstruction PerpendicularLine
        {
            get
            {
                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                    new ObjectConstructionParameter(Point),
                    new ObjectConstructionParameter(Line)
                };

                // Create the actual construction
                return new PredefinedConstruction(PredefinedConstructionType.PerpendicularLine, parameters, Line);
            }
        }

        /// <summary>
        /// Creates a <see cref="PredefinedConstructionType.ParallelLine"/> construction.
        /// </summary>
        /// <returns>The construction.</returns>
        public static PredefinedConstruction ParallelLine
        {
            get
            {
                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                    new ObjectConstructionParameter(Point),
                    new ObjectConstructionParameter(Line)
                };

                // Create the actual construction
                return new PredefinedConstruction(PredefinedConstructionType.ParallelLine, parameters, Line);
            }
        }

        /// <summary>
        /// Creates a <see cref="PredefinedConstructionType.PointReflection"/> construction.
        /// </summary>
        /// <returns>The construction.</returns>
        public static PredefinedConstruction PointReflection
        {
            get
            {
                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                    new ObjectConstructionParameter(Point),
                    new ObjectConstructionParameter(Point)
                };

                // Create the actual construction
                return new PredefinedConstruction(PredefinedConstructionType.PointReflection, parameters, Point);
            }
        }

        /// <summary>
        /// Creates a <see cref="PredefinedConstructionType.SecondIntersectionOfCircleAndLineFromPoints"/> construction.
        /// </summary>
        /// <returns>The construction.</returns>
        public static PredefinedConstruction SecondIntersectionOfCircleAndLineFromPoints
        {
            get
            {
                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                    new ObjectConstructionParameter(Point),
                    new ObjectConstructionParameter(Point),
                    new SetConstructionParameter(new ObjectConstructionParameter(Point), 2)
                };

                // Create the actual construction
                return new PredefinedConstruction(PredefinedConstructionType.SecondIntersectionOfCircleAndLineFromPoints, parameters, Point);
            }
        }

        /// <summary>
        /// Creates a <see cref="PredefinedConstructionType.SecondIntersectionOfTwoCircumcircles"/> construction.
        /// </summary>
        /// <returns>The construction.</returns>
        public static PredefinedConstruction SecondIntersectionOfTwoCircumcircles
        {
            get
            {
                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                    new ObjectConstructionParameter(Point),
                    new SetConstructionParameter(new SetConstructionParameter(new ObjectConstructionParameter(Point), 2), 2)
                };

                // Create the actual construction
                return new PredefinedConstruction(PredefinedConstructionType.SecondIntersectionOfTwoCircumcircles, parameters, Point);
            }
        }
    }
}