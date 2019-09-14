using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.DependenciesResolver;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.TheoremFinder.Tests
{
    /// <summary>
    /// A base class for tests of <see cref="ITypedTheoremFinder"/>.
    /// </summary>
    /// <typeparam name="T">The type of theorem finder being tested.</typeparam>
    public abstract class TypedTheoremFinderTestBase<T> where T : ITypedTheoremFinder
    {
        /// <summary>
        /// Runs the algorithm on the configuration to find new and all theorems. 
        /// </summary>
        /// <param name="configuration">The configuration where we're looking for theorems.</param>
        /// <param name="instanceFactory">The factory for creating an instance of the finder. If it's null, the default constructor is used.</param>
        /// <returns>The new and all theorems.</returns>
        protected (List<Theorem> newTheorems, List<Theorem> allTheorems) FindTheorems(Configuration configuration, Func<T> instanceFactory = null)
        {
            // Prepare the kernel
            var kernel = IoC.CreateKernel().AddConstructor(new PicturesSettings
            {
                NumberOfPictures = 5,
                MaximalAttemptsToReconstructAllPictures = 0,
                MaximalAttemptsToReconstructOnePicture = 0
            });

            // Create the pictures
            var pictures = kernel.Get<IGeometryConstructor>().Construct(configuration).pictures;

            // Create the contextual picture
            var contextualPicture = kernel.Get<IContextualPictureFactory>().CreateContextualPicture(pictures);

            // If the instance factory is specified
            var finder = instanceFactory != null ?
                // Invoke it
                instanceFactory() :
                // Otherwise use reflection to call the parameterless constructor
                Activator.CreateInstance<T>();

            // Run both algorithms
            return (finder.FindNewTheorems(contextualPicture).ToList(), finder.FindAllTheorems(contextualPicture).ToList());
        }
    }
}
