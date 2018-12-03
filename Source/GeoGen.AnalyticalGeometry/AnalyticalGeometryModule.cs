using GeoGen.Core;

namespace GeoGen.AnalyticalGeometry
{
    /// <summary>
    /// A NInject module that binds things from the Analytical Geometry module.
    /// </summary>
    public class AnalyticalGeometryModule : BaseModule
    {
        /// <summary>
        /// Loads all bindings.
        /// </summary>
        public override void Load()
        {
            BindInGeneratorScope<IRandomObjectsProvider, RandomObjectsProvider>();
            BindInGeneratorScope<IRandomnessProvider, RandomnessProvider>();
            BindInGeneratorScope<ITriangleConstructor, TriangleConstructor>();
        }
    }
}