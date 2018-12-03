using System.Collections.Generic;
using GeoGen.AnalyticalGeometry;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// A base class for <see cref="IObjectsConstructor"/>s that handles common
    /// things for all objects constructors (like flattening the objects arguments).
    /// </summary>
    internal abstract class ObjectsConstructorBase : IObjectsConstructor
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
                DefaultTheoremsFunction = () => FindDefaultTheorms(constructedObjects, flattenedObjects)
            };
        }

        #endregion

        #region Protected abstract methods

        /// <summary>
        /// Constructs a list of analytical objects from a given list of 
        /// flattened objects from the arguments and a container that is used to 
        /// obtain the actual analytical versions of these objects.
        /// </summary>
        /// <param name="flattenedObjects">The flattened argument objects.</param>
        /// <param name="container">The objects container.</param>
        /// <returns>The list of constructed analytical objects.</returns>
        protected abstract List<AnalyticalObject> Construct(IReadOnlyList<ConfigurationObject> flattenedObjects, IObjectsContainer container);

        /// <summary>
        /// Constructs a list of default theorems using a newly constructed objects and
        /// flattened objects from the passed arguments.
        /// </summary>
        /// <param name="input">The constructed objects.</param>
        /// <param name="flattenedObjects">The flattened argument objects.</param>
        /// <returns>The list of default theorems.</returns>
        protected abstract List<Theorem> FindDefaultTheorms(IReadOnlyList<ConstructedConfigurationObject> input, IReadOnlyList<ConfigurationObject> flattenedObjects);

        #endregion
    }
}