﻿using GeoGen.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a list of <see cref="ConstructionArgument"/> that are used to create <see cref="ConstructedConfigurationObject"/>s. 
    /// </summary>
    public class Arguments : IEnumerable<ConstructionArgument>
    {
        #region Public properties

        /// <summary>
        /// Gets the arguments list wraped by this object.
        /// </summary>
        public IReadOnlyList<ConstructionArgument> ArgumentsList { get; }

        /// <summary>
        /// Gets the list of configuration objects that are obtained within the arguments
        /// in the order that we get if we recursively search through them from left to right. 
        /// For example: With { {A,B}, {C,D} } we might get A,B,C,D; or D,C,B,A. The order of objects
        /// within a set  itself is not deterministic. This list is lazily evaluated.
        /// </summary>
        public IReadOnlyList<ConfigurationObject> FlattenedList { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs arguments wrapping a given arguments list. 
        /// </summary>
        /// <param name="argumentsList">The arguments list.</param>
        public Arguments(IReadOnlyList<ConstructionArgument> argumentsList)
        {
            ArgumentsList = argumentsList ?? throw new ArgumentNullException(nameof(argumentsList));
            FlattenedList = ExtraxtInputObject();
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Finds all objects in the arguments and flattens them to a list.
        /// </summary>
        /// <returns>The objects list.</returns>
        private List<ConfigurationObject> ExtraxtInputObject()
        {
            // Prepare the result
            var result = new List<ConfigurationObject>();

            // Local function to extract objects from an argument
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
                setArgument.PassedArguments.ForEach(Extract);
            }

            // Now we just call our local function for all arguments
            ArgumentsList.ForEach(Extract);

            // And return the result
            return result;
        }

        #endregion

        #region IEnumerable implementation

        /// <summary>
        /// Gets a generic enumerator.
        /// </summary>
        /// <returns>The generic enumerator.</returns>
        public IEnumerator<ConstructionArgument> GetEnumerator()
        {
            return ArgumentsList.GetEnumerator();
        }

        /// <summary>
        /// Gets a non-generic enumerator.
        /// </summary>
        /// <returns>The non-generic enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
