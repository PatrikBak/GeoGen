using System.Collections.Generic;
using GeoGen.AnalyticGeometry;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// A base class for <see cref="IObjectsConstructor"/>s that handles common
    /// things for all objects constructors (like flattening the objects arguments).
    /// </summary>
    public abstract class ObjectsConstructorBase : IObjectsConstructor
    {
        #region IObjectsConstructor methods

        /// <summary>
        /// Constructs a given list of constructed configurations objects. These objects 
        /// should be the result of the same construction.
        /// </summary>
        /// <param name="constructedObjects">The constructed objects list.</param>
        /// <returns>The constructor output.</returns>
        public ConstructorOutput Construct(IReadOnlyList<ConstructedConfigurationObject> constructedObjects)
        {
            // First pull the flatened objects
            var flattenedObjects = constructedObjects[0].PassedArguments.FlattenedList;

            // And construct the output using the abstract methods
            return new ConstructorOutput
            {
                ConstructorFunction = container => Construct(flattenedObjects, container),
            };
        }

        #endregion

        #region Protected abstract methods

        /// <summary>
        /// Constructs a list of analytic objects from a given list of 
        /// flattened objects from the arguments and a container that is used to 
        /// obtain the actual analytic versions of these objects.
        /// </summary>
        /// <param name="flattenedObjects">The flattened argument objects.</param>
        /// <param name="container">The objects container.</param>
        /// <returns>The list of constructed analytic objects.</returns>
        protected abstract List<AnalyticObject> Construct(IReadOnlyList<ConfigurationObject> flattenedObjects, IObjectsContainer container);

        #endregion
    }
}