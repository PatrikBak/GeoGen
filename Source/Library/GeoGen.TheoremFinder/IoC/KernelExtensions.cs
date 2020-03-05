using GeoGen.Core;
using Ninject;
using System;
using System.Linq;

namespace GeoGen.TheoremFinder
{
    /// <summary>
    /// The extension methods for <see cref="IKernel"/>.
    /// </summary>
    public static class KernelExtensions
    {
        /// <summary>
        /// Bindings for the dependencies from the Theorem Finder module.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        /// <param name="settings">The settings for the module.</param>
        /// <returns>The kernel for chaining.</returns>
        public static IKernel AddTheoremFinder(this IKernel kernel, TheoremFindingSettings settings)
        {
            // Bind the theorem finder
            kernel.Bind<ITheoremFinder>().To<TheoremFinder>();

            #region Bind theorem finders for individual types

            // Bind typed theorem finders based on the types we're seeking
            foreach (var theoremType in settings.SoughtTheoremTypes.Distinct())
            {
                // Find the expected name of the class with the corresponding namespace
                var classNameWithNamespace = $"{typeof(ITheoremFinder).Namespace}.{theoremType}TheoremFinder";

                // Find the type of the finder from the name
                var theoremFinderType = Type.GetType(classNameWithNamespace);

                // Handle if it couldn't be found
                if (theoremFinderType == null)
                    throw new TheoremFinderException($"Couldn't find an implementation of {nameof(ITypedTheoremFinder)} for type '{theoremType}', expected class name with namespace '{classNameWithNamespace}'");

                // Otherwise do the binding
                var binding = kernel.Bind(typeof(ITypedTheoremFinder)).To(theoremFinderType);

                // In some cases there is a constructor argument
                switch (theoremType)
                {
                    // Line and circle tangency 
                    case TheoremType.LineTangentToCircle:
                        binding.WithConstructorArgument(settings.LineTangentToCircleTheoremFinderSettings);
                        break;

                    // Two circles tangency
                    case TheoremType.TangentCircles:
                        binding.WithConstructorArgument(settings.TangentCirclesTheoremFinderSettings);
                        break;
                }
            }

            #endregion

            // Return the kernel for chaining
            return kernel;
        }
    }
}