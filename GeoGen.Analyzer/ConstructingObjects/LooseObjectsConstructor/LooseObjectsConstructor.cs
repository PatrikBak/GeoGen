using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.AnalyticalGeometry;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// A default implementation of <see cref="ILooseObjectsConstructor"/> that uses 
    /// <see cref="IRandomObjectsProvider"/> to construct mutually distinct objects
    /// with no other conditions (this should be improved later).
    /// </summary>
    internal class LooseObjectsConstructor : ILooseObjectsConstructor
    {
        #region Private fields

        /// <summary>
        /// The provided of random distinct objects.
        /// </summary>
        private readonly IRandomObjectsProvider _provider;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="provider">The provider of random distinct objects.</param>
        public LooseObjectsConstructor(IRandomObjectsProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        #endregion

        #region ILooseObjectsConstructor implementation

        /// <summary>
        /// Constructs given loose objects.
        /// </summary>
        /// <param name="looseObjects">The loose objects.</param>
        /// <returns>The list of analytical objects.</returns>
        public List<AnalyticalObject> Construct(IEnumerable<LooseConfigurationObject> looseObjects)
        {
            // Construct all objects one by one
            return looseObjects.Select(looseObject =>
            {
                // According to the type find the right random object
                switch (looseObject.ObjectType)
                {
                    case ConfigurationObjectType.Point:
                        return _provider.NextRandomObject<Point>();
                    case ConfigurationObjectType.Line:
                        return _provider.NextRandomObject<Line>();
                    case ConfigurationObjectType.Circle:
                        return _provider.NextRandomObject<Circle>();
                    default:
                        throw new AnalyzerException("Unhandled type of object.");
                }
            }).ToList();
        }

        #endregion
    }
}