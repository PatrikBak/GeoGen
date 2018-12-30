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
        /// Creates a circumcenter construction.
        /// </summary>
        /// <returns>The construction.</returns>
        private static PredefinedConstruction CircumcenterFromPoints()
        {
            // Create the parameters
            var parameters = new List<ConstructionParameter>
            {
                new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 3)
            };
            // Create the actual construction
            return new PredefinedConstruction(PredefinedConstructionType.CircumcenterFromPoints, parameters, ConfigurationObjectType.Point);
        }

        /// <summary>
        /// Creates a circumcircle construction.
        /// </summary>
        /// <returns>The construction.</returns>
        private static PredefinedConstruction CircumcircleFromPoints()
        {
            // Create the parameters
            var parameters = new List<ConstructionParameter>
            {
                new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 3)
            };

            // Create the actual construction
            return new PredefinedConstruction(PredefinedConstructionType.CircumcircleFromPoints, parameters, ConfigurationObjectType.Circle);
        }

        /// <summary>
        /// Creates an internal angle bisector construction.
        /// </summary>
        /// <returns>The construction.</returns>
        private static PredefinedConstruction InternalAngleBisectorFromPoints()
        {
            // Create the parameters
            var parameters = new List<ConstructionParameter>
            {
                new ObjectConstructionParameter(ConfigurationObjectType.Point),
                new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 2)
            };

            // Create the actual construction
            return new PredefinedConstruction(PredefinedConstructionType.InternalAngleBisectorFromPoints, parameters, ConfigurationObjectType.Line);
        }

        /// <summary>
        /// Creates an intersection of lines construction.
        /// </summary>
        /// <returns>The construction.</returns>
        private static PredefinedConstruction IntersectionOfLinesFromPoints()
        {
            // Create the parameters
            var parameters = new List<ConstructionParameter>
            {
                new SetConstructionParameter
                (
                    new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 2), 2
                )
            };

            // Create the actual construction
            return new PredefinedConstruction(PredefinedConstructionType.IntersectionOfLinesFromPoints, parameters, ConfigurationObjectType.Point);
        }

        /// <summary>
        /// Creates an intersection of lines construction.
        /// </summary>
        /// <returns>The construction.</returns>
        private static PredefinedConstruction IntersectionOfLinesFromLineAndPoints()
        {
            // Create the parameters
            var parameters = new List<ConstructionParameter>
            {
                new ObjectConstructionParameter(ConfigurationObjectType.Line),
                new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 2)
            };

            // Create the actual construction
            return new PredefinedConstruction(PredefinedConstructionType.IntersectionOfLinesFromLineAndPoints, parameters, ConfigurationObjectType.Point);
        }

        /// <summary>
        /// Creates an intersection of lines construction.
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
        /// Creates a random point of a line construction.
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
        /// Creates a random point of a line construction.
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
        /// Creates a midpoint of a line segment construction.
        /// </summary>
        /// <returns>The construction.</returns>
        private static PredefinedConstruction MidpointFromPoints()
        {
            // Create the parameters
            var parameters = new List<ConstructionParameter>
            {
                new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 2)
            };
            
            // Create the actual construction
            return new PredefinedConstruction(PredefinedConstructionType.MidpointFromPoints, parameters, ConfigurationObjectType.Point);
        }

        /// <summary>
        /// Creates a perpendicular line construction.
        /// </summary>
        /// <returns>The construction.</returns>
        private static PredefinedConstruction PerpendicularLineFromPoints()
        {
            // Create the parameters
            var parameters = new List<ConstructionParameter>
            {
                new ObjectConstructionParameter(ConfigurationObjectType.Point),
                new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 2)
            };

            // Create the actual construction
            return new PredefinedConstruction(PredefinedConstructionType.PerpendicularLineFromPoints, parameters, ConfigurationObjectType.Line);
        }

        /// <summary>
        /// Creates a second intersection of circle from points and line from points construction.
        /// </summary>
        /// <returns>The construction.</returns>
        private static PredefinedConstruction SecondIntersectionOfCircleFromPointsAndLineFromPoints()
        {
            // Create the parameters
            var parameters = new List<ConstructionParameter>
            {
                new ObjectConstructionParameter(ConfigurationObjectType.Point),
                new ObjectConstructionParameter(ConfigurationObjectType.Point),
                new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 2)
            };

            // Create the actual construction
            return new PredefinedConstruction(PredefinedConstructionType.SecondIntersectionOfCircleFromPointsAndLineFromPoints, parameters, ConfigurationObjectType.Point);
        }

        #endregion
    }
}