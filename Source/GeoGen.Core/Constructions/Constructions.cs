using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Core
{
    /// <summary>
    /// Helpers class containing method for getting constructions predefined in the classes
    /// <see cref="PredefinedConstructions"/> and <see cref="ComposedConstructions"/>.
    /// </summary>
    public static class Constructions
    {
        /// <summary>
        /// Gets the predefined construction from a <see cref="PredefinedConstructionType"/>.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>The construction</returns>
        public static PredefinedConstruction GetPredefinedconstruction(PredefinedConstructionType type)
        {
            // Find the property info for the property handling our predefined type
            var propertyInfo = typeof(PredefinedConstructions).GetProperty(type.ToString());

            // Check if it's not null
            if (propertyInfo == null)
                throw new Exception($"The type {type} of constructions doesn't have the implementation in the {nameof(PredefinedConstruction)} class.");

            // Otherwise we invoke it and return the casted result
            return (PredefinedConstruction) propertyInfo.GetMethod.Invoke(obj: null, parameters: null);
        }

        /// <summary>
        /// Finds the composed construction by its name. 
        /// </summary>
        /// <param name="constructionName">The name of the construction.</param>
        /// <returns>The composed construction with the passed name; or null, if it doesn't exist.</returns>
        public static ComposedConstruction GetComposedConstruction(string constructionName)
        {
            // Use reflection to find the property with the needed name
            return (ComposedConstruction) typeof(ComposedConstructions).GetProperty(constructionName)?.GetMethod.Invoke(null, null);
        }

        /// <summary>
        /// Gets all constructions defined in <see cref="PredefinedConstructions"/> 
        /// and <see cref="ComposedConstructions"/> classes.
        /// </summary>
        /// <returns>The constructions</returns>
        public static List<Construction> GetAll()
        {
            // Helper function that gets all construction from a class
            IEnumerable<Construction> ConstructionFromClass(Type typeOfClass)
            {
                // Get all properties
                return typeOfClass.GetProperties()
                    // Perform the get method of each
                    .Select(p => p.GetGetMethod().Invoke(null, null))
                    // Cast to a construction
                    .Cast<Construction>();
            }

            // Merge the predefined and composed ones
            return ConstructionFromClass(typeof(PredefinedConstructions)).Concat(ConstructionFromClass(typeof(ComposedConstructions))).ToList();
        }
    }
}
