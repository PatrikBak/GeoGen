using GeoGen.AnalyticGeometry;
using GeoGen.Core;
using System;
using System.Collections.Generic;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// The default implementation of <see cref="ILooseObjectsConstructor"/>. Currently there is only 
    /// one supported layout <see cref="LooseObjectsLayout.ScaleneAcuteAngledTriangled"/> constructed
    /// via <see cref="ITriangleConstructor"/>. This will be improved later.
    /// </summary>
    public class LooseObjectsConstructor : ILooseObjectsConstructor
    {
        #region Dependencies

        /// <summary>
        /// The constructor of random triangles.
        /// </summary>
        private readonly ITriangleConstructor _triangleConstructor;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="LooseObjectsConstructor"/> class.
        /// </summary>
        /// <param name="triangleConstructor">The constructor of random triangles.</param>
        public LooseObjectsConstructor(ITriangleConstructor triangleConstructor)
        {
            _triangleConstructor = triangleConstructor ?? throw new ArgumentNullException(nameof(triangleConstructor));
        }

        #endregion

        #region ILooseObjectsConstructor implementation

        /// <summary>
        /// Constructs the objects of a given loose objects holder.
        /// </summary>
        /// <param name="looseObjectsHolder">The loose objects holder whose objects should be constructed.</param>
        /// <returns>Analytic versions of particular loose objects of the holder.</returns>
        public List<AnalyticObject> Construct(LooseObjectsHolder looseObjectsHolder)
        {
            switch (looseObjectsHolder.Layout)
            {
                // Don't allow null layout in this scenario 
                case null:
                    throw new AnalyzerException("The loose objects constructor doesn't allow the loose objects layout to be 'null'.");

                // For a triangle let the helper constructor to the job and create a random triangle
                case LooseObjectsLayout.ScaleneAcuteAngledTriangled:
                    return _triangleConstructor.NextScaleneAcuteAngedTriangle();

                // If we got here, we have an unsupported layout :/
                default:
                    throw new AnalyzerException($"Unsupported loose objects layout: {looseObjectsHolder.Layout}");
            }
        }

        #endregion
    }
}