using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// A base implementation of <see cref="IPredefinedConstructor"/> that infers the
    /// type of the construction from a name that should be in the form "{type}Constructor".
    /// </summary>
    internal abstract class PredefinedConstructorBase : IPredefinedConstructor
    {
        #region IPredefinedConstructor properties
        
        /// <summary>
        /// Gets the type of predefined construction that this constructor performs.
        /// </summary>
        public PredefinedConstructionType PredefinedConstructionType { get; }

        #endregion

        #region IPredefinedConstructor methods

        /// <summary>
        /// Gets the type of predefined construction that this constructor performs.
        /// </summary>
        public abstract ConstructorOutput Construct(List<ConstructedConfigurationObject> constructedObjects);

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        protected PredefinedConstructorBase()
        {
            // Set the compulsory construction type
            PredefinedConstructionType = FindTypeFromClassName();
        }

        #endregion

        #region Finding type from class name

        /// <summary>
        /// Infers the type of the predefined constructor from the class name. 
        /// The class name should be in the form {type}Constructor.
        /// </summary>
        /// <returns>The type.</returns>
        private PredefinedConstructionType FindTypeFromClassName()
        {
            // Construct the regex with one group that catches the name
            var regex = new Regex("^(.*)Constructor$");

            // Get the class name
            var className = GetType().Name;

            // Do the matching
            var match = regex.Match(className);

            // If we failed, we want to throw an exception
            if (!match.Success)
                throw new Exception($"The class {className} doesn't match the name pattern '{{type}}Constructor'");

            // Otherwise we pull the supposed type name
            var typeName = match.Groups[1].Value;

            // Try to parse (without ignoring the cases)
            var parsingSuccessful = Enum.TryParse(typeName, false, out PredefinedConstructionType result);

            // If the parsing failed, we want to thrown an exception
            if (!parsingSuccessful)
                throw new Exception($"Unable to parse the {typeName} (inferred from the {className}) into a value of {nameof(PredefinedConstructionType)}.");

            // Otherwise we're fine
            return result;
        } 

        #endregion
    }
}