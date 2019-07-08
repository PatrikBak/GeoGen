using System;
using System.Collections.Generic;
using System.Reflection;

namespace GeoGen.Core
{
    /// <summary>
    /// The static factory for creating <see cref="PredefinedConstruction"/>s.
    /// </summary>
    public static class PredefinedConstructionsFactory
    {
        #region Get method

        /// <summary>
        /// Gets the predefined construction from a <see cref="PredefinedConstructionType"/>.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>The construction</returns>
        public static PredefinedConstruction Get(PredefinedConstructionType type)
        {
            // Find the method info for the method handling our predefined type
            var methodInfo = typeof(PredefinedConstructionsFactory).GetMethod(type.ToString(), BindingFlags.NonPublic | BindingFlags.Static);

            // Check if it's not null
            if (methodInfo == null)
                throw new Exception($"The type {type} of constructions doesn't have the implementation in the {nameof(PredefinedConstruction)} class.");

            // Otherwise we invoke it and return the casted result
            return (PredefinedConstruction) methodInfo.Invoke(obj: null, parameters: null);
        }

        #endregion

        #region Constructions

        /// <summary>
        /// Creates a <see cref="PredefinedConstructionType.CenterOfCircle"/> construction.
        /// </summary>
        /// <returns>The construction.</returns>
        private static PredefinedConstruction CenterOfCircle()
        {
            // Create the parameters
            var parameters = new List<ConstructionParameter>
            {
                new ObjectConstructionParameter(ConfigurationObjectType.Circle)
            };

            // Create the actual construction
            return new PredefinedConstruction(PredefinedConstructionType.CenterOfCircle, parameters, ConfigurationObjectType.Point);
        }

        /// <summary>
        /// Creates a <see cref="PredefinedConstructionType.Circumcircle"/> construction.
        /// </summary>
        /// <returns>The construction.</returns>
        private static PredefinedConstruction Circumcircle()
        {
            // Create the parameters
            var parameters = new List<ConstructionParameter>
            {
                new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 3)
            };

            // Create the actual construction
            return new PredefinedConstruction(PredefinedConstructionType.Circumcircle, parameters, ConfigurationObjectType.Circle);
        }

        /// <summary>
        /// Creates a <see cref="PredefinedConstructionType.CircleWithCenterThroughPoint"/> construction.
        /// </summary>
        /// <returns>The construction.</returns>
        private static PredefinedConstruction CircleWithCenterThroughPoint()
        {
            // Create the parameters
            var parameters = new List<ConstructionParameter>
            {
                new ObjectConstructionParameter(ConfigurationObjectType.Point),
                new ObjectConstructionParameter(ConfigurationObjectType.Point),
            };

            // Create the actual construction
            return new PredefinedConstruction(PredefinedConstructionType.CircleWithCenterThroughPoint, parameters, ConfigurationObjectType.Circle);
        }

        /// <summary>
        /// Creates a <see cref="PredefinedConstructionType.InternalAngleBisector"/> construction.
        /// </summary>
        /// <returns>The construction.</returns>
        private static PredefinedConstruction InternalAngleBisector()
        {
            // Create the parameters
            var parameters = new List<ConstructionParameter>
            {
                new ObjectConstructionParameter(ConfigurationObjectType.Point),
                new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 2)
            };

            // Create the actual construction
            return new PredefinedConstruction(PredefinedConstructionType.InternalAngleBisector, parameters, ConfigurationObjectType.Line);
        }

        /// <summary>
        /// Creates a <see cref="PredefinedConstructionType.IntersectionOfLines"/> construction.
        /// </summary>
        /// <returns>The construction.</returns>
        private static PredefinedConstruction IntersectionOfLines()
        {
            // Create the parameters
            var parameters = new List<ConstructionParameter>
            {
                new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Line), 2)
            };

            // Create the actual construction
            return new PredefinedConstruction(PredefinedConstructionType.IntersectionOfLines, parameters, ConfigurationObjectType.Point);
        }

        /// <summary>
        /// Creates a <see cref="PredefinedConstructionType.LineFromPoints"/> construction.
        /// </summary>
        /// <returns>The construction.</returns>
        private static PredefinedConstruction LineFromPoints()
        {
            // Create the parameters
            var parameters = new List<ConstructionParameter>
            {
                new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 2)
            };

            // Create the actual construction
            return new PredefinedConstruction(PredefinedConstructionType.LineFromPoints, parameters, ConfigurationObjectType.Line);
        }

        /// <summary>
        /// Creates a <see cref="PredefinedConstructionType.Midpoint"/> construction.
        /// </summary>
        /// <returns>The construction.</returns>
        private static PredefinedConstruction Midpoint()
        {
            // Create the parameters
            var parameters = new List<ConstructionParameter>
            {
                new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 2)
            };

            // Create the actual construction
            return new PredefinedConstruction(PredefinedConstructionType.Midpoint, parameters, ConfigurationObjectType.Point);
        }

        /// <summary>
        /// Creates a <see cref="PredefinedConstructionType.PerpendicularProjection"/> construction.
        /// </summary>
        /// <returns>The construction.</returns>
        private static PredefinedConstruction PerpendicularProjection()
        {
            // Create the parameters
            var parameters = new List<ConstructionParameter>
            {
                new ObjectConstructionParameter(ConfigurationObjectType.Point),
                new ObjectConstructionParameter(ConfigurationObjectType.Line)
            };

            // Create the actual construction
            return new PredefinedConstruction(PredefinedConstructionType.PerpendicularProjection, parameters, ConfigurationObjectType.Point);
        }

        /// <summary>
        /// Creates a <see cref="PredefinedConstructionType.PerpendicularLine"/> construction.
        /// </summary>
        /// <returns>The construction.</returns>
        private static PredefinedConstruction PerpendicularLine()
        {
            // Create the parameters
            var parameters = new List<ConstructionParameter>
            {
                new ObjectConstructionParameter(ConfigurationObjectType.Point),
                new ObjectConstructionParameter(ConfigurationObjectType.Line)
            };

            // Create the actual construction
            return new PredefinedConstruction(PredefinedConstructionType.PerpendicularLine, parameters, ConfigurationObjectType.Line);
        }

        /// <summary>
        /// Creates a <see cref="PredefinedConstructionType.ParallelLine"/> construction.
        /// </summary>
        /// <returns>The construction.</returns>
        private static PredefinedConstruction ParallelLine()
        {
            // Create the parameters
            var parameters = new List<ConstructionParameter>
            {
                new ObjectConstructionParameter(ConfigurationObjectType.Point),
                new ObjectConstructionParameter(ConfigurationObjectType.Line)
            };

            // Create the actual construction
            return new PredefinedConstruction(PredefinedConstructionType.ParallelLine, parameters, ConfigurationObjectType.Line);
        }

        /// <summary>
        /// Creates a <see cref="PredefinedConstructionType.PointReflection"/> construction.
        /// </summary>
        /// <returns>The construction.</returns>
        private static PredefinedConstruction PointReflection()
        {
            // Create the parameters
            var parameters = new List<ConstructionParameter>
            {
                new ObjectConstructionParameter(ConfigurationObjectType.Point),
                new ObjectConstructionParameter(ConfigurationObjectType.Point)
            };

            // Create the actual construction
            return new PredefinedConstruction(PredefinedConstructionType.PointReflection, parameters, ConfigurationObjectType.Point);
        }

        /// <summary>
        /// Creates a <see cref="PredefinedConstructionType.SecondIntersectionOfCircleAndLineFromPoints"/> construction.
        /// </summary>
        /// <returns>The construction.</returns>
        private static PredefinedConstruction SecondIntersectionOfCircleAndLineFromPoints()
        {
            // Create the parameters
            var parameters = new List<ConstructionParameter>
            {
                new ObjectConstructionParameter(ConfigurationObjectType.Point),
                new ObjectConstructionParameter(ConfigurationObjectType.Point),
                new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 2)
            };

            // Create the actual construction
            return new PredefinedConstruction(PredefinedConstructionType.SecondIntersectionOfCircleAndLineFromPoints, parameters, ConfigurationObjectType.Point);
        }

        /// <summary>
        /// Creates a <see cref="PredefinedConstructionType.SecondIntersectionOfCircleWithCenterAndLineFromPoints"/> construction.
        /// </summary>
        /// <returns>The construction.</returns>
        private static PredefinedConstruction SecondIntersectionOfCircleWithCenterAndLineFromPoints()
        {
            // Create the parameters
            var parameters = new List<ConstructionParameter>
            {
                new ObjectConstructionParameter(ConfigurationObjectType.Point),
                new ObjectConstructionParameter(ConfigurationObjectType.Point),
                new ObjectConstructionParameter(ConfigurationObjectType.Point),
            };

            // Create the actual construction
            return new PredefinedConstruction(PredefinedConstructionType.SecondIntersectionOfCircleWithCenterAndLineFromPoints, parameters, ConfigurationObjectType.Point);
        }

        /// <summary>
        /// Creates a <see cref="PredefinedConstructionType.SecondIntersectionOfTwoCircumcircles"/> construction.
        /// </summary>
        /// <returns>The construction.</returns>
        private static PredefinedConstruction SecondIntersectionOfTwoCircumcircles()
        {
            // Create the parameters
            var parameters = new List<ConstructionParameter>
            {
                new ObjectConstructionParameter(ConfigurationObjectType.Point),
                new SetConstructionParameter(new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 2), 2)
            };

            // Create the actual construction
            return new PredefinedConstruction(PredefinedConstructionType.SecondIntersectionOfTwoCircumcircles, parameters, ConfigurationObjectType.Point);
        }

        /// <summary>
        /// Creates a <see cref="PredefinedConstructionType.RandomPointOnLine"/> construction.
        /// </summary>
        /// <returns>The construction.</returns>
        private static PredefinedConstruction RandomPointOnLine()
        {
            // Create the parameters
            var parameters = new List<ConstructionParameter>
            {
                new ObjectConstructionParameter(ConfigurationObjectType.Line)
            };

            // Create the actual construction
            return new PredefinedConstruction(PredefinedConstructionType.RandomPointOnLine, parameters, ConfigurationObjectType.Point);
        }

        /// <summary>
        /// Creates a <see cref="PredefinedConstructionType.RandomPointOnLineFromPoints"/> construction.
        /// </summary>
        /// <returns>The construction.</returns>
        private static PredefinedConstruction RandomPointOnLineFromPoints()
        {
            // Create the parameters
            var parameters = new List<ConstructionParameter>
            {
                new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 2)
            };

            // Create the actual construction
            return new PredefinedConstruction(PredefinedConstructionType.RandomPointOnLineFromPoints, parameters, ConfigurationObjectType.Point);
        }

        /// <summary>
        /// Creates a <see cref="PredefinedConstructionType.RandomPointOnCircle"/> construction.
        /// </summary>
        /// <returns>The construction.</returns>
        private static PredefinedConstruction RandomPointOnCircle()
        {
            // Create the parameters
            var parameters = new List<ConstructionParameter>
            {
                new ObjectConstructionParameter(ConfigurationObjectType.Circle)
            };

            // Create the actual construction
            return new PredefinedConstruction(PredefinedConstructionType.RandomPointOnCircle, parameters, ConfigurationObjectType.Point);
        }

        /// <summary>
        /// Creates a <see cref="PredefinedConstructionType.RandomPoint"/> construction.
        /// </summary>
        /// <returns>The construction.</returns>
        private static PredefinedConstruction RandomPoint()
        {
            // Create the actual construction, which has no parameters
            return new PredefinedConstruction(PredefinedConstructionType.RandomPoint, new ConstructionParameter[0], ConfigurationObjectType.Point);
        }

        #endregion
    }
}