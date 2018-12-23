using GeoGen.Core;
using GeoGen.Generator;
using Ninject;
using System;

namespace GeoGen.Runner
{
    /// <summary>
    /// A default implementation of <see cref="IGeneratorFactory"/> that uses the NInject resolution root.
    /// </summary>
    internal class GeneratorFactory : IGeneratorFactory
    {
        #region Private fields

        /// <summary>
        /// The lock for the CreateGenerator method calls.
        /// </summary>
        private static readonly object _lock = new object();

        #endregion

        #region Dependencies

        /// <summary>
        /// The NInject kernel.
        /// </summary>
        private readonly IKernel _kernel;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneratorFactory"/> class.
        /// </summary>
        /// <param name="kernel">The NInject kernel.</param>
        public GeneratorFactory(IKernel kernel)
        {
            _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
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
            // Make sure two different threads won't manipulate the container
            lock (_lock)
            {
                // Bind the input, so it can be accessed in the run-time construction arguments resolution
                var inputBinding = _kernel.Bind<GeneratorInput>().ToConstant(generatorInput);

                // Get the generator
                var generator = _kernel.Get<IGenerator>();

                // Release the input so it can be bound again later
                _kernel.Unbind<GeneratorInput>();

                // Return the generator
                return generator;
            }
        }

        #endregion
    }
}