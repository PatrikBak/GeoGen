using GeoGen.Core;
using GeoGen.Core.Constructions;

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
            BindInGeneratorScope<IGradualAnalyzer, GradualAnalyzer>();
            BindInGeneratorScope<ITheoremConstructor, TheoremConstructor>();
            BindInGeneratorScope<ITheoremsVerifier, TheoremsVerifier>();
            BindInGeneratorScope<ITheoremsContainer, TheoremsContainer>();
            BindInGeneratorScope<ILooseObjectsConstructor, LooseObjectsConstructor>();
            BindInGeneratorScope<IConstructorsResolver, ConstructorsResolver>();
            BindInGeneratorScope<IContextualContainer, ContextualContainer>();
            BindFactoryInGeneratorScope<IComposedConstructorFactory>();
            BindInGeneratorScope<IObjectsContainersManager, ObjectsContainersManager>("looseObjects", input => input.InitialConfiguration.LooseObjects);
            BindInGeneratorScope<IGeometryRegistrar, GeometryRegistrar>();
            BindInTransietScope<IObjectsContainer, ObjectsContainer>();

            BindFactoryInGeneratorScope<IObjectsContainerFactory>();

            BindInGeneratorScope<ITheoremVerifier, ConcurrencyVerifier>();
            BindInGeneratorScope<ITheoremVerifier, CollinearityVerifier>();

            BindInGeneratorScope<IPredefinedConstructor, MidpointConstructor>();
            BindInGeneratorScope<IPredefinedConstructor, InteresectionConstructor>();
            BindInGeneratorScope<IPredefinedConstructor, CircumcenterConstructor>();
        }
    }
}