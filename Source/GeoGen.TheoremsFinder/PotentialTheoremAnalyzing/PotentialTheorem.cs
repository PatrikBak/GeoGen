using GeoGen.Constructor;
using GeoGen.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.TheoremsFinder
{
    /// <summary>
    /// Represents a currently unverified theorem of a certain type that might hold true. 
    /// This object is an output of <see cref="IPotentialTheoremsAnalyzer"/>s. 
    /// </summary>
    public class PotentialTheorem
    {
        #region Public properties

        /// <summary>
        /// Gets or sets the type of this potential theorem.
        /// </summary>
        public TheoremType TheoremType { get; set; }

        /// <summary>
        /// Gets or sets the enumerable of the geometric objects that this theorem is about. 
        /// Their actual types depend on the <see cref="Core.TheoremType"/>.
        /// </summary>
        public IEnumerable<GeometricObject> InvolvedObjects { get; set; }

        /// <summary>
        /// Gets or sets the function that is able to tell if the theorem is true in a given picture.
        /// </summary>
        public Func<IPicture, bool> VerificationFunction { get; set; }

        #endregion

        #region Public methods

        /// <summary>
        /// Converts this potential theorem to an actual theorem.
        /// </summary>
        /// <param name="configuration">The configuration where the theorem holds.</param>
        /// <returns>The theorem.</returns>
        public Theorem ToTheorem(Configuration configuration)
        {
            // Map geometric objects to theorem objects
            var allTheoremObjects = InvolvedObjects.Select(geometricObject => geometricObject switch
                {
                    // In point case we have just the object
                    PointObject point => new PointTheoremObject(geometricObject.ConfigurationObject) as TheoremObject,

                    // In line case we need to take points into account
                    LineObject line => new LineTheoremObject(geometricObject.ConfigurationObject, line.Points.Select(p => p.ConfigurationObject)),

                    // In circle case we need to take points into account
                    CircleObject circle => new CircleTheoremObject(geometricObject.ConfigurationObject, circle.Points.Select(p => p.ConfigurationObject)),

                    _ => throw new ConstructorException($"Unhandled type of geometric object: {geometricObject.GetType()}"),
                })
                // Enumerate to an array
                .ToArray();

            // Use helper method to construct the theorem
            return Theorem.DeriveFromFlattenedObjects(configuration, TheoremType, allTheoremObjects);
        }

        #endregion
    }
}