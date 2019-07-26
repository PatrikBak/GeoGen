using GeoGen.AnalyticGeometry;
using GeoGen.Core;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Constructor
{
    /// <summary>
    /// The default implementation of <see cref="ILooseObjectsConstructor"/>. Currently there is only 
    /// one supported layout <see cref="LooseObjectsLayout.ScaleneAcuteTriangle"/>.
    /// </summary>
    public class LooseObjectsConstructor : ILooseObjectsConstructor
    {
        /// <summary>
        /// Constructs the objects of a given loose objects holder.
        /// </summary>
        /// <param name="looseObjectsHolder">The loose objects holder whose objects should be constructed.</param>
        /// <returns>Analytic versions of particular loose objects of the holder.</returns>
        public List<IAnalyticObject> Construct(LooseObjectsHolder looseObjectsHolder)
        {
            switch (looseObjectsHolder.Layout)
            {
                // Don't allow none layout in this scenario 
                case LooseObjectsLayout.NoLayout:
                    throw new ConstructorException("The loose objects constructor doesn't allow the loose objects layout to be 'null'.");

                // For a triangle let the helper constructor to the job and create a random triangle
                case LooseObjectsLayout.ScaleneAcuteTriangle:
                    return AnalyticHelpers.ConstructRandomScaleneAcuteTriangle().Cast<IAnalyticObject>().ToList();

                // If we got here, we have an unsupported layout :/
                default:
                    throw new ConstructorException($"Unsupported loose objects layout: {looseObjectsHolder.Layout}");
            }
        }
    }
}