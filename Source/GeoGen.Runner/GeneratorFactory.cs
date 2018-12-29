using GeoGen.Generator;
using Ninject;
using Ninject.Extensions.ContextPreservation;
using Ninject.Extensions.Factory;
using System;

namespace GeoGen.Runner
{
    /// <summary>
    /// The default implementation of <see cref="IGeneratorFactory"/> that uses NInject.
    /// </summary>
    public class GeneratorFactory : IGeneratorFactory
    {
        #region Public properties

        /// <summary>
        /// The action that gets called directly before resolving the generator. 
        /// </summary>
        private readonly Action<IKernel> _kernelConfigurer;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneratorFactory"/> class.
        /// </summary>
        /// <param name="kernelConfigurer">The action that gets called directly before resolving the generator. Its default value is null.</param>
        public GeneratorFactory(Action<IKernel> kernelConfigurer = null)
        {
            _kernelConfigurer = kernelConfigurer;
        }

        #endregion

        #region IGeneratorFactory implementation

        /// <summary>
        /// Creates a generator for a given generator input.
        /// </summary>
        /// <param name="generatorInput">The generator input.</param>
        /// <returns>The generator.</returns>
        public IGenerator CreateGenerator(GeneratorInput generatorInput)
        {
            // Create a kernel that will perform the resolution
            var kernel = new StandardKernel(new FuncModule(), new ContextPreservationModule());

            // Make sure we can bind to null 
            // This is used only for tracers, that are not compulsory
            // I like this better than the NullObject pattern, because
            // of the ? operator that can be used to prevent null-checks
            kernel.Settings.AllowNullInjection = true;

            // Add the algorithm dependencies to it
            kernel.AddAlgorithm(generatorInput);

            // Call the set configurer
            _kernelConfigurer?.Invoke(kernel);

            // Resolve the generator
            return kernel.Get<IGenerator>();
        }

        #endregion
    }
}