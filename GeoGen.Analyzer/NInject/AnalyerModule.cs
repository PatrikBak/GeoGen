using System;
using GeoGen.Analyzer.Constructing;
using GeoGen.Analyzer.Constructing.PredefinedConstructors;
using GeoGen.Analyzer.Objects;
using GeoGen.Analyzer.Objects.GeometricalObjects.Container;
using GeoGen.Analyzer.Theorems;
using GeoGen.Analyzer.Theorems.TheoremVerifiers;
using Ninject.Modules;

namespace GeoGen.Analyzer.NInject
{
    /// <summary>
    /// A NInject module that binds things from the Core module.
    /// </summary>
    public class AnalyerModule : NinjectModule
    {
        /// <summary>
        /// Loads all bindings.
        /// </summary>
        public override void Load()
        {
            Bind<IGradualAnalyzer>().To<GradualAnalyzer>().InSingletonScope();
            Bind<IAnalyzerInitializer>().To<AnalyzerInitializer>().InSingletonScope();
            Bind<ITheoremConstructor>().To<TheoremConstructor>().InSingletonScope();
            Bind<ITheoremsVerifier>().To<TheoremsVerifier>().InSingletonScope();
            Bind<ITheoremsContainer>().To<TheoremsContainer>().InSingletonScope();
            Bind<IGeometryRegistrar>().To<GeometryRegistrar>().InSingletonScope();
            Bind<IObjectsContainersFactory>().To<ObjectsContainersFactory>().InSingletonScope();
            Bind<IObjectsContainersManager>().To<ObjectsContainersManager>().InSingletonScope();
            Bind<IContextualContainer>().To<ContextualContainer>().InSingletonScope();
            Bind<IConstructorsResolver>().To<ConstructorsResolver>().InSingletonScope();
            Bind<ILooseObjectsConstructor>().To<LooseObjectsConstructor>().InSingletonScope();

            Bind<ITheoremVerifier>().To<ConcurrencyVerifier>().InSingletonScope();
            Bind<ITheoremVerifier>().To<CollinearityVerifier>().InSingletonScope();

            Bind<IPredefinedConstructor>().To<MidpointConstructor>().InSingletonScope();
            Bind<IPredefinedConstructor>().To<InteresectionConstructor>().InSingletonScope();
        }
    }
}