using System;
using System.Linq;
using GeoGen.Core.Generator;
using Ninject.Activation;
using Ninject.Extensions.Factory;
using Ninject.Extensions.NamedScope;
using Ninject.Modules;
using Ninject.Planning.Targets;

namespace GeoGen.Core.NInject
{
    public abstract class BaseModule : NinjectModule
    {
        private const string GeneratorScopeName = "Generator";

        private const string InputParameterName = "generatorInput";

        protected void BindInGeneratorScope<TDepedency, TImplementation>()
            where TImplementation : TDepedency
        {
            Bind<TDepedency>().To<TImplementation>().InNamedScope(GeneratorScopeName);
        }

        protected void BindFactoryInGeneratorScope<TFactory>()
            where TFactory : class
        {
            Bind<TFactory>().ToFactory().InNamedScope(GeneratorScopeName);
        }

        protected void BindFactoryInSingletonScope<TFactory>()
            where TFactory : class
        {
            Bind<TFactory>().ToFactory().InSingletonScope();
        }

        protected void BindInGeneratorScopeToSelf<TDependency>()
            where TDependency : class
        {
            Bind<TDependency>().ToSelf().InNamedScope(GeneratorScopeName);
        }

        protected void BindInSingletonScope<TDependency, TImplementation>()
            where TImplementation : TDependency
        {
            Bind<TDependency>().To<TImplementation>().InSingletonScope();
        }

        protected void BindInTransietScope<TDependency, TImplementation>()
            where TImplementation : TDependency
        {
            Bind<TDependency>().To<TImplementation>().InTransientScope();
        }

        protected void BindInGeneratorScope<TDependency, TImplementation>(string argumentName, Func<GeneratorInput, object> argumentValue)
            where TImplementation : TDependency
        {
            Bind<TDependency>()
                    .To<TImplementation>()
                    .InNamedScope(GeneratorScopeName)
                    .WithConstructorArgument(argumentName, ArgumentValueCallback(argumentValue, InputParameterName));
        }

        protected void BindDefiningGeneratorScope<TDependency, TImplementation>(string argumentName, Func<GeneratorInput, object> argumentValue)
            where TImplementation : TDependency
        {
            var binding = Bind<TDependency>()
                    .To<TImplementation>()
                    .WithConstructorArgument(argumentName, ArgumentValueCallback(argumentValue, InputParameterName));

            binding.DefinesNamedScope(GeneratorScopeName);
        }

        private Func<IContext, ITarget, object> ArgumentValueCallback(Func<GeneratorInput, object> argumentValue, string inputParameterName)
        {
            return (context, target) =>
            {
                var inputParameters = context
                        .Parameters
                        .Where(p => p.Name == inputParameterName)
                        .ToList();

                if (inputParameters.Count == 0)
                    throw new Exception($"No parameter with the name {inputParameterName}");

                if (inputParameters.Count != 1)
                    throw new Exception($"There is more than one parameter with the name {inputParameterName}");

                var input = inputParameters[0].GetValue(context, target) as GeneratorInput;

                if (input == null)
                    throw new Exception($"The parameter with the name {inputParameterName} should be an instance of {nameof(GeneratorInput)}.");

                return argumentValue(input);
            };
        }
    }
}