using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// A NInject module that binds things from the Core module.
    /// </summary>
    public class AnalyerModule : BaseModule
    {
        /// <summary>
        /// Loads all bindings.
        /// </summary>
        public override void Load()
        {
            BindInGeneratorScope<ITheoremsAnalyzer, TheoremsAnalyzer>();
            BindInGeneratorScope<IOutputValidator, OutputValidator>();
            BindInGeneratorScope<INeedlessObjectsAnalyzer, NeedlessObjectsAnalyzer>();
            BindInGeneratorScope<ITheoremConstructor, TheoremConstructor>();
            BindInGeneratorScope<ITheoremsContainer, TheoremsContainer>();
            BindInGeneratorScope<ILooseObjectsConstructor, LooseObjectsConstructor>();
            BindInGeneratorScope<IConstructorsResolver, ConstructorsResolver>();
            BindInTransietScope<IComposedConstructor, ComposedConstructor>();
            BindInGeneratorScope<IContextualContainerFactory, ContextualContainerFactory>();
            BindFactoryInGeneratorScope<IComposedConstructorFactory>();
            BindInTransietScope<IObjectsContainersManager, ObjectsContainersManager>();
            BindFactoryInGeneratorScope<IObjectsContainersManagerFactory>();
            BindInGeneratorScope<IGeometryRegistrar, GeometryRegistrar>();
            BindInTransietScope<IObjectsContainer, ObjectsContainer>();
            BindFactoryInGeneratorScope<IObjectsContainerFactory>();
            BindInGeneratorScope<IObjectContainersMapper, ObjectContainersMapper>();

            BindInGeneratorScope<ITheoremVerifier, CollinearPointsVerifier>();
            BindInGeneratorScope<ITheoremVerifier, ConcurrentObjectsVerifier>();
            BindInGeneratorScope<ITheoremVerifier, ConcyclicPointsVerifier>();
            //BindInGeneratorScope<ITheoremVerifier, EqualAngelsVerifier>();
            BindInGeneratorScope<ITheoremVerifier, EqualLineSegmentsVerifier>();
            //BindInGeneratorScope<ITheoremVerifier, LineTangentToCircleVerifier>();
            BindInGeneratorScope<ITheoremVerifier, ParallelLinesVerifier>();
            //BindInGeneratorScope<ITheoremVerifier, PerpendicularLinesVerifier>();
            BindInGeneratorScope<ITheoremVerifier, TangentCirclesVerifier>();

            BindInGeneratorScope<IPredefinedConstructor, CircumcenterFromPointsConstructor>();
            BindInGeneratorScope<IPredefinedConstructor, CircumcircleFromPointsConstructor>();
            BindInGeneratorScope<IPredefinedConstructor, IntersectionOfLinesConstructor>();
            BindInGeneratorScope<IPredefinedConstructor, IntersectionOfLinesFromLineAndPointsConstructor>();
            BindInGeneratorScope<IPredefinedConstructor, IntersectionOfLinesFromPointsConstructor>();
            BindInGeneratorScope<IPredefinedConstructor, InternalAngleBisectorFromPointsConstructor>();
            BindInGeneratorScope<IPredefinedConstructor, LoosePointOnLineFromPointsConstructor>();
            BindInGeneratorScope<IPredefinedConstructor, MidpointFromPointsConstructor>();
            BindInGeneratorScope<IPredefinedConstructor, PerpendicularLineFromPointsConstructor>();
        }
    }
}