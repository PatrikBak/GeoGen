using System;
using System.Collections.Generic;

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
            switch (type)
            {
                case PredefinedConstructionType.CircumcenterFromPoints:
                    return CircumcenterFromPoints();
                case PredefinedConstructionType.CircumcircleFromPoints:
                    return CircumcircleFromPoints();
                case PredefinedConstructionType.InternalAngelBisectorFromPoints:
                    return InternalAngleBisectorFromPoints();
                case PredefinedConstructionType.IntersectionOfLinesFromPoints:
                    return IntersectionOfLinesFromPoints();
                case PredefinedConstructionType.IntersectionOfLinesFromLineAndPoints:
                    return IntersectionOfLinesFromLineAndPoints();
                case PredefinedConstructionType.IntersectionOfLines:
                    return IntersectionOfLines();
                case PredefinedConstructionType.LoosePointOnLineFromPoints:
                    return LoosePointOnLineFromPoints();
                case PredefinedConstructionType.MidpointFromPoints:
                    return MidpointFromPoints();
                case PredefinedConstructionType.PerpendicularLineFromPoints:
                    return PerpendicularLineFromPoints();
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
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

            // Create the output types
            var outputTypes = new List<ConfigurationObjectType> {ConfigurationObjectType.Point};

            // Create the actual construction
            return new PredefinedConstruction(PredefinedConstructionType.CircumcenterFromPoints, parameters, outputTypes);
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

            // Create the output types
            var outputTypes = new List<ConfigurationObjectType> {ConfigurationObjectType.Circle};

            // Create the actual construction
            return new PredefinedConstruction(PredefinedConstructionType.CircumcircleFromPoints, parameters, outputTypes);
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

            // Create the output types
            var outputTypes = new List<ConfigurationObjectType> {ConfigurationObjectType.Line};

            // Create the actual construction
            return new PredefinedConstruction(PredefinedConstructionType.InternalAngelBisectorFromPoints, parameters, outputTypes);
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

            // Create the output types
            var outputTypes = new List<ConfigurationObjectType> {ConfigurationObjectType.Point};

            // Create the actual construction
            return new PredefinedConstruction(PredefinedConstructionType.IntersectionOfLinesFromPoints, parameters, outputTypes);
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
                new SetConstructionParameter
                (
                    new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 2), 2
                )
            };

            // Create the output types
            var outputTypes = new List<ConfigurationObjectType> {ConfigurationObjectType.Point};

            // Create the actual construction
            return new PredefinedConstruction(PredefinedConstructionType.IntersectionOfLinesFromLineAndPoints, parameters, outputTypes);
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

            // Create the output types
            var outputTypes = new List<ConfigurationObjectType> {ConfigurationObjectType.Point};

            // Create the actual construction
            return new PredefinedConstruction(PredefinedConstructionType.IntersectionOfLines, parameters, outputTypes);
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

            // Create the output types
            var outputTypes = new List<ConfigurationObjectType> {ConfigurationObjectType.Point};

            // Create the actual construction
            return new PredefinedConstruction(PredefinedConstructionType.LoosePointOnLineFromPoints, parameters, outputTypes);
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

            // Create the output types
            var outputTypes = new List<ConfigurationObjectType> {ConfigurationObjectType.Point};

            // Create the actual construction
            return new PredefinedConstruction(PredefinedConstructionType.MidpointFromPoints, parameters, outputTypes);
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

            // Create the output types
            var outputTypes = new List<ConfigurationObjectType> {ConfigurationObjectType.Line};

            // Create the actual construction
            return new PredefinedConstruction(PredefinedConstructionType.PerpendicularLineFromPoints, parameters, outputTypes);
        }

        #endregion
    }
}