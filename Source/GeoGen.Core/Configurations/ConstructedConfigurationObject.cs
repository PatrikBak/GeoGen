using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Core
{
    /// <summary>
    /// Represent a <see cref="ConfigurationObject"/> that is composed of a <see cref="Core.Construction"/>, 
    /// and <see cref="Arguments"/> that hold actual configuration objects from which this one is constructed.
    /// </summary>
    public class ConstructedConfigurationObject : ConfigurationObject
    {
        #region Public properties

        /// <summary>
        /// Gets the construction that creates this object.
        /// </summary>
        public Construction Construction { get; }

        /// <summary>
        /// Gets the arguments that have been passed to the construction. 
        /// </summary>
        public Arguments PassedArguments { get; }

        #endregion

        #region Configuration Object properties

        /// <summary>
        /// Gets the actual geometrical type of this object (such as Point, Line...)
        /// </summary>
        public override ConfigurationObjectType ObjectType { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor that uses arguments.
        /// </summary>
        /// <param name="construction">The construction.</param>
        /// <param name="arguments">The passed arguments to the construction.</param>
        public ConstructedConfigurationObject(Construction construction, Arguments arguments)
        {
            Construction = construction ?? throw new ArgumentNullException(nameof(construction));
            PassedArguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
            ObjectType = Construction.OutputType;
        }

        public ConstructedConfigurationObject(Construction construction, List<ConstructionArgument> argumentsList)
            : this(construction, new Arguments(argumentsList))
        {
        }

        public ConstructedConfigurationObject(Construction construction, Arguments arguments, int id) 
            : this(construction, arguments)
        {
            Id = id;
        }

        #endregion

        #region Public abstract methods overrides

        /// <summary>
        /// Enumerates the objects that are internally used to create this object. The order of this objects
        /// should match the order in which we can gradually construct them.
        /// </summary>
        /// <returns>A lazy enumerable of the internal objects.</returns>
        public override IEnumerable<ConfigurationObject> InternalObjects() => PassedArguments.FlattenedList.SelectMany(obj => obj.AsEnumerable().Concat(obj.InternalObjects())).Distinct().Reverse();

        #endregion

        #region Protected abstract methods overrides

        /// <summary>
        /// Converts the object to a string using already set names of the objects.
        /// </summary>
        /// <param name="objectToStringMap"></param>
        /// <returns>A human-readable string representation of the object.</returns>
        protected override string ToString(IReadOnlyDictionary<ConfigurationObject, string> objectToStringMap)
        {
            // Compose the final string
            return $"{Construction.Name}({string.Join(",", PassedArguments.FlattenedList.Select(obj => objectToStringMap[obj]))})";
        }

        #endregion
    }
}