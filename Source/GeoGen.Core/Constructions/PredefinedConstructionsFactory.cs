using System;
using System.Collections.Generic;
using System.Reflection;

namespace GeoGen.Core
{
    /// <summary>
    /// A factory for creating predefined constructions.
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
            return (PredefinedConstruction) methodInfo.Invoke(null, null);
        }

        #endregion

        #region Constructions

        /// <summary>
        /// Creates a non-identified circumcenter construction.
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
        /// Creates a non-identified circumcircle construction.
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
        /// Creates a non-identified internal angle bisector construction.
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
        /// Creates a non-identified intersection of lines construction.
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
        /// Creates a non-identified intersection of lines construction.
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
        /// Creates a non-identified intersection of lines construction.
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
        /// Creates a non-identified loose point of a line construction.
        /// </summary>
        /// <returns>The construction.</returns>
        private static PredefinedConstruction LoosePointOnLineFromPoints()
        {
            // Create the parameters
            var parameters = new List<ConstructionParameter>
            {
                new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 2)
            };

            // Create the actual construction
            return new PredefinedConstruction(PredefinedConstructionType.LoosePointOnLineFromPoints, parameters, ConfigurationObjectType.Point);
        }

        /// <summary>
        /// Creates a non-identified midpoint of a line segment construction.
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
        /// Creates a non-identified perpendicular line construction.
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

        #endregion
    }
}