using System;
using GeoGen.Analyzer.Constructing;
using GeoGen.Analyzer.Constructing.PredefinedConstructors;
using GeoGen.Analyzer.Objects;
using GeoGen.Analyzer.Objects.GeometricalObjects.Container;
using GeoGen.Analyzer.Theorems;
using GeoGen.Analyzer.Theorems.TheoremVerifiers;
using GeoGen.Core.NInject;
using Ninject.Modules;

namespace GeoGen.Analyzer.NInject
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
            BindInGeneratorScope<IObjectsContainersManager, ObjectsContainersManager>();
            BindInTransietScope<IObjectsContainer, ObjectsContainer>();

            BindFactoryInGeneratorScope<IObjectsContainersFactory>();

            BindInGeneratorScope<ITheoremVerifier, ConcurrencyVerifier>();
            BindInGeneratorScope<ITheoremVerifier, CollinearityVerifier>();

            BindInGeneratorScope<IPredefinedConstructor, MidpointConstructor>();
            BindInGeneratorScope<IPredefinedConstructor, InteresectionConstructor>();

            BindInGeneratorScope<IGeometryRegistrar, GeometryRegistrar>("initialConfiguration", input => input.InitialConfiguration);
        }
    }
}