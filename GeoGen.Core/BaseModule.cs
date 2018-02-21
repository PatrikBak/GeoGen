using System;
using System.Linq;
using Ninject.Activation;
using Ninject.Extensions.Factory;
using Ninject.Extensions.NamedScope;
using Ninject.Modules;
using Ninject.Planning.Targets;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a base NInject module used for all generator submodules. 
    /// It defines methods that simplify common bindings in generator scope,
    /// or bindings using arguments that are pulled from a <see cref="GeneratorInput"/>.
    /// </summary>
    public abstract class BaseModule : NinjectModule
    {
        #region Protected constants

        /// <summary>
        /// The identifier name of the generator NInject scope.
        /// </summary>
        protected const string GeneratorScopeName = "Generator";

        /// <summary>
        /// The name of the parameter holding the generator input, passed to the
        /// Create method of the <see cref="IGeneratorFactory"/> interface.
        /// </summary>
        protected const string GeneratorInputParameterName = "generatorInput";

        #endregion

        #region Protected binding helpers

        /// <summary>
        /// Binds a given dependency type into its implementation type in the generator scope.
        /// </summary>
        /// <typeparam name="TDependency">The dependency type.</typeparam>
        /// <typeparam name="TImplementation">The implementation type.</typeparam>
        protected void BindInGeneratorScope<TDependency, TImplementation>()
                where TImplementation : TDependency
        {
            Bind<TDependency>().To<TImplementation>().InNamedScope(GeneratorScopeName);
        }

        /// <summary>
        /// Binds a given dependency type into its implementation type in the singleton scope.
        /// </summary>
        /// <typeparam name="TDependency">The dependency type.</typeparam>
        /// <typeparam name="TImplementation">The implementation type.</typeparam>
        protected void BindInSingletonScope<TDependency, TImplementation>()
                where TImplementation : TDependency
        {
            Bind<TDependency>().To<TImplementation>().InSingletonScope();
        }

        /// <summary>
        /// Binds a given dependency type into its implementation type in the transient scope
        /// (a new instance per each request).
        /// </summary>
        /// <typeparam name="TDependency">The dependency type.</typeparam>
        /// <typeparam name="TImplementation">The implementation type.</typeparam>
        protected void BindInTransietScope<TDependency, TImplementation>()
                where TImplementation : TDependency
        {
            Bind<TDependency>().To<TImplementation>().InTransientScope();
        }

        /// <summary>
        /// Binds a given type to factory in the generator scope.
        /// </summary>
        /// <typeparam name="TFactory">The factory type.</typeparam>
        protected void BindFactoryInGeneratorScope<TFactory>()
                where TFactory : class
        {
            Bind<TFactory>().ToFactory().InNamedScope(GeneratorScopeName);
        }

        /// <summary>
        /// Binds a given type to factory in the singleton scope.
        /// </summary>
        /// <typeparam name="TFactory">The factory type</typeparam>
        protected void BindFactoryInSingletonScope<TFactory>()
                where TFactory : class
        {
            Bind<TFactory>().ToFactory().InSingletonScope();
        }

        /// <summary>
        /// Binds a given dependency type into its implementation type in the generator scope
        /// with additional construction arguments given by a name and a callback function.
        /// </summary>
        /// <typeparam name="TDependency">The dependency type.</typeparam>
        /// <typeparam name="TImplementation">The implementation type.</typeparam>
        /// <param name="argumentName">The name of the construction argument.</param>
        /// <param name="argumentValue">The callback function that pulls the value for the argument from a generator input.</param>
        protected void BindInGeneratorScope<TDependency, TImplementation>(string argumentName, Func<GeneratorInput, object> argumentValue)
                where TImplementation : TDependency
        {
            Bind<TDependency>().To<TImplementation>()
                    .InNamedScope(GeneratorScopeName)
                    .WithConstructorArgument(argumentName, ArgumentValueCallback(argumentValue));
        }

        /// <summary>
        /// Binds a given dependency type into its implementation defining the generator scope
        /// with additional construction arguments given by a name and a callback function.
        /// This should be used exactly once since exactly one class should define the generator scope.
        /// </summary>
        /// <typeparam name="TDependency">The dependency type.</typeparam>
        /// <typeparam name="TImplementation">The implementation type</typeparam>
        /// <param name="argumentName">The name of the construction argument.</param>
        /// <param name="argumentValue">The callback function that pulls the value for the argument from a generator input.</param>
        protected void BindDefiningGeneratorScope<TDependency, TImplementation>(string argumentName, Func<GeneratorInput, object> argumentValue)
                where TImplementation : TDependency
        {
            var binding = Bind<TDependency>().To<TImplementation>()
                    .WithConstructorArgument(argumentName, ArgumentValueCallback(argumentValue));

            binding.DefinesNamedScope(GeneratorScopeName);
        }

        #endregion

        #region Passing construction arguments

        /// <summary>
        /// Creates a callback function that looks up the passed <see cref="GeneratorInput"/> 
        /// and executes the argument value function on it to obtain the actual value. This function
        /// could be used only within the generator scope, because it assumes the generator input
        /// was provided.
        /// </summary>
        /// <param name="argumentValue">The callback function that pulls the value for the argument from a generator input.</param>
        /// <returns>The argument value function.</returns>
        private Func<IContext, ITarget, object> ArgumentValueCallback(Func<GeneratorInput, object> argumentValue)
        {
            return (context, target) =>
            {
                // Find all currently requested parameters with the expected name for the generator input
                var inputParameters = context
                        .Parameters
                        .Where(p => p.Name == GeneratorInputParameterName)
                        .ToList();

                // Make sure there is any
                if (inputParameters.Count == 0)
                    throw new GeoGenException($"No parameter with the name {GeneratorInputParameterName}");

                // Check if there is exactly one
                if (inputParameters.Count != 1)
                    throw new GeoGenException($"There is more than one parameter with the name {GeneratorInputParameterName}");

                // Make sure that its value type is the type of GeneratorInput
                if (!(inputParameters[0].GetValue(context, target) is GeneratorInput input))
                    throw new GeoGenException($"The parameter with the name {GeneratorInputParameterName} should be an instance of {nameof(GeneratorInput)}.");

                // Perform the callback function to obtain the result
                return argumentValue(input);
            };
        }

        #endregion
    }
}