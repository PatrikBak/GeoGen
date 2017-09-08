using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Constructions.Parameters;

namespace GeoGen.Generator.Constructing.Arguments.SignatureMatching
{
    /// <summary>
    /// A default implementation of the <see cref="IConstructionSignatureMatcher"/> interface.
    /// </summary>
    internal class ConstructionSignatureMatcher : IConstructionSignatureMatcher
    {
        #region Private fields

        /// <summary>
        /// The available objects dictionary.
        /// </summary>
        private IReadOnlyDictionary<ConfigurationObjectType, List<ConfigurationObject>> _objectTypeToObjects;

        /// <summary>
        /// The internal dictionary mapping an object type to the index of an object that can be used
        /// in a construction argument.
        /// </summary>
        private Dictionary<ConfigurationObjectType, int> _currentIndices;

        #endregion

        #region IConstructionSignatureMatcher implementation

        /// <summary>
        /// Initializes the signature matcher with objects represented as a map between objects types
        /// and all objects of that type.
        /// </summary>
        /// <param name="objectTypeToObjects">The objects dictionary.</param>
        public void Initialize(IReadOnlyDictionary<ConfigurationObjectType, List<ConfigurationObject>> objectTypeToObjects)
        {
            _objectTypeToObjects = objectTypeToObjects ?? throw new ArgumentNullException(nameof(objectTypeToObjects));
            _currentIndices = objectTypeToObjects.ToDictionary(keyValue => keyValue.Key, keyValue => 0);
            // TODO: Debug check if nothing is null
        }

        /// <summary>
        /// Constructs construction arguments that match the given construction parameters. It must be
        /// possible to perform the construction, otherwise a <see cref="GeneratorException"/> is thrown.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The created arguments.</returns>
        public IReadOnlyList<ConstructionArgument> Match(IReadOnlyList<ConstructionParameter> parameters)
        {
            return parameters.Select(CreateArgument).ToList();
        }

        /// <summary>
        /// Creates a next argument for a given parameter. 
        /// </summary>
        /// <param name="parameter">The given parameter.</param>
        /// <returns>The next argument.</returns>
        private ConstructionArgument CreateArgument(ConstructionParameter parameter)
        {
            if (parameter is ObjectConstructionParameter objectParameter)
            {
                var nextObject = Next(objectParameter.ExpectedType);

                return new ObjectConstructionArgument(nextObject);
            }

            var setParameter = parameter as SetConstructionParameter ?? throw new GeneratorException("Unhandled case.");

            var argumentsSet = new HashSet<ConstructionArgument>();

            for (var i = 0; i < setParameter.NumberOfParameters; i++)
            {
                // recursively call this function
                var newArgument = CreateArgument(setParameter.TypeOfParameters);

                // update the resulting set
                argumentsSet.Add(newArgument);
            }

            return new SetConstructionArgument(argumentsSet);
        }

        /// <summary>
        /// Gets the next object of the type that is supposed to be used in a construction argument.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The next available object.</returns>
        private ConfigurationObject Next(ConfigurationObjectType type)
        {
            try
            {
                return _objectTypeToObjects[type][_currentIndices[type]++];
            }
            catch (Exception)
            {
                throw new GeneratorException("Cannot do the matching, there not too few objects.");
            }
        }

        #endregion
    }
}