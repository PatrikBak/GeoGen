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
        public ConstructorOutput Construct(List<ConstructedConfigurationObject> constructedObjects)
        {
            // First flatten the arguments
            var flattenedObjects = ExtraxtInputObject(constructedObjects[0].PassedArguments);

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
        protected abstract List<AnalyticalObject> Construct(List<ConfigurationObject> flattenedObjects, IObjectsContainer container);

        /// <summary>
        /// Constructs a list of default theorems using a newly constructed objects and
        /// flattened objects from the passed arguments.
        /// </summary>
        /// <param name="input">The constructed objects.</param>
        /// <param name="flattenedObjects">The flattened argument objects.</param>
        /// <returns>The list of default theorems.</returns>
        protected abstract List<Theorem> FindDefaultTheorms(List<ConstructedConfigurationObject> input, List<ConfigurationObject> flattenedObjects);

        #endregion

        #region Private helpers

        /// <summary>
        /// Finds all objects in the arguments and flattens them to the list.
        /// </summary>
        /// <param name="arguments">The arguments list.</param>
        /// <returns>The objects list.</returns>
        private List<ConfigurationObject> ExtraxtInputObject(IEnumerable<ConstructionArgument> arguments)
        {
            // Prepare the result
            var result = new List<ConfigurationObject>();

            // Local function to extract object from an argument
            void Extract(ConstructionArgument argument)
            {
                // If we have an object argument
                if (argument is ObjectConstructionArgument objectArgument)
                {
                    // Then we simply add the internal object to the result
                    result.Add(objectArgument.PassedObject);

                    // And terminate
                    return;
                }

                // Otherwise we have a set argument
                var setArgument = (SetConstructionArgument) argument;

                // We recursively call this function for internal arguments
                foreach (var passedArgument in setArgument.PassedArguments)
                {
                    Extract(passedArgument);
                }
            }

            // Now we just call our local function for all passed arguments
            foreach (var argument in arguments)
            {
                Extract(argument);
            }

            // And return the result
            return result;
        }

        #endregion
    }
}