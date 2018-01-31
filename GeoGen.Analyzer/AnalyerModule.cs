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
            BindInGeneratorScope<ITheoremsValidator, TheoremsValidator>();
            BindInGeneratorScope<IOutputValidator, OutputValidator>();
            BindInGeneratorScope<INeedlessObjectsAnalyzer, NeedlessObjectsAnalyzer>();
            BindInGeneratorScope<ITheoremConstructor, TheoremConstructor>();
            BindInGeneratorScope<ITheoremsContainer, TheoremsContainer>();
            BindInGeneratorScope<ILooseObjectsConstructor, LooseObjectsConstructor>();
            BindInGeneratorScope<IConstructorsResolver, ConstructorsResolver>();
            BindInTransietScope<IComposedConstructor, ComposedConstructor>();
            BindInGeneratorScope<IContextualContainerFactory, ContextualContainerFactory>();
            BindFactoryInGeneratorScope<IComposedConstructorFactory>();
            BindInGeneratorScope<IObjectsContainersManager, ObjectsContainersManager>("looseObjects", input => input.InitialConfiguration.LooseObjects);
            BindInGeneratorScope<IGeometryRegistrar, GeometryRegistrar>();
            BindInTransietScope<IObjectsContainer, ObjectsContainer>();

            BindFactoryInGeneratorScope<IObjectsContainerFactory>();

            BindInGeneratorScope<ITheoremVerifier, ConcurrencyVerifier>();
            BindInGeneratorScope<ITheoremVerifier, CollinearityVerifier>();

            BindInGeneratorScope<IPredefinedConstructor, CircumcenterFromPointsConstructor>();
            BindInGeneratorScope<IPredefinedConstructor, CircumcircleFromPointsConstructor>();
            BindInGeneratorScope<IPredefinedConstructor, IntersectionOfLinesConstructor>();
            BindInGeneratorScope<IPredefinedConstructor, IntersectionOfLinesFromLineAndPointsConstructor>();
            BindInGeneratorScope<IPredefinedConstructor, IntersectionOfLinesFromPointsConstructor>();
            BindInGeneratorScope<IPredefinedConstructor, InternalAngelBisectorFromPointsConstructor>();
            BindInGeneratorScope<IPredefinedConstructor, LoosePointOnLineFromPointsConstructor>();
            BindInGeneratorScope<IPredefinedConstructor, MidpointFromPointsConstructor>();
            BindInGeneratorScope<IPredefinedConstructor, PerpendicularLineFromPointsConstructor>();
        }
    }
}