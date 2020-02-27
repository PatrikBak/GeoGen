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
        #region Public methods

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
                throw new GeoGenException($"The type {type} of a construction doesn't have the implementation in the {nameof(PredefinedConstructions)} class.");

            // Otherwise we invoke it and return the casted result
            return (PredefinedConstruction)propertyInfo.GetMethod.Invoke(obj: null, parameters: null);
        }

        /// <summary>
        /// Finds the composed construction by its name. 
        /// </summary>
        /// <param name="constructionName">The name of the construction.</param>
        /// <returns>The composed construction with the passed name; or null, if it doesn't exist.</returns>
        public static ComposedConstruction GetComposedConstruction(string constructionName)
        {
            // Use reflection to find the property with the needed name
            return (ComposedConstruction)typeof(ComposedConstructions).GetProperty(constructionName)?.GetMethod.Invoke(null, null);
        }

        /// <summary>
        /// Gets all <see cref="PredefinedConstruction"/> defined in the <see cref="PredefinedConstructions"/> class.
        /// </summary>
        /// <returns>The predefined constructions.</returns>
        public static IEnumerable<PredefinedConstruction> GetPredefinedConstructions() => ConstructionsFromClass<PredefinedConstruction>(typeof(PredefinedConstructions));

        /// <summary>
        /// Gets all <see cref="ComposedConstruction"/> defined in the <see cref="ComposedConstructions"/> class.
        /// </summary>
        /// <returns>The composed constructions.</returns>
        public static IEnumerable<ComposedConstruction> GetComposedConstructions() => ConstructionsFromClass<ComposedConstruction>(typeof(ComposedConstructions));

        /// <summary>
        /// Gets all constructions defined in <see cref="PredefinedConstructions"/> 
        /// and <see cref="ComposedConstructions"/> classes.
        /// </summary>
        /// <returns>The constructions</returns>
        public static IEnumerable<Construction> GetAllConstructions() => GetPredefinedConstructions().Cast<Construction>().Concat(GetComposedConstructions());

        #endregion

        #region Private methods

        /// <summary>
        /// Finds all constructions of given type in a given class.
        /// </summary>
        /// <typeparam name="T">The type of constructions.</typeparam>
        /// <param name="typeOfClass">The class where the constructions should be.</param>
        /// <returns>The constructions.</returns>
        private static IEnumerable<T> ConstructionsFromClass<T>(Type typeOfClass) where T : Construction
        {
            // Get all properties
            return typeOfClass.GetProperties()
                // Perform the get method of each
                .Select(property => property.GetGetMethod().Invoke(null, null))
                // Cast to the requested type
                .Cast<T>();
        }

        #endregion
    }
}
