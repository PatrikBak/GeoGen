using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a <see cref="Construction"/> that is defined as an output of some configuration. 
    /// This is supposed to represent a complex constructions that can defined dynamically, unlike
    /// <see cref="PredefinedConstruction"/>. An example of such a construction could be the centroid
    /// of a triangle, which can be defined using <see cref="PredefinedConstructionType.MidpointFromPoints"/>
    /// and <see cref="PredefinedConstructionType.IntersectionOfLinesFromPoints"/>. The output of 
    /// such a construction is the last object in the list of constructed objects of the configuration
    /// that defines it. From this type the output type of the construction is inferred.
    /// </summary>
    public class ComposedConstruction : Construction
    {
        #region Public properties

        /// <summary>
        /// Gets the configuration that defines this composed construction. The output of the construction is
        /// its last constructed object. Its loose objects should match its signature. The layout of the 
        /// loose objects is ignored.
        /// </summary>
        public Configuration Configuration { get; }

        /// <summary>
        /// Gets the constructed configuration object that represents the output of this construction.
        /// </summary>
        public ConstructedConfigurationObject ConstructionOutput => Configuration.LastConstructedObject;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ComposedConstruction"/> class.
        /// </summary>
        /// <param name="name">The name of the construction.</param>
        /// <param name="configuration">The configuration that defines the construction steps. The output of the construction is its last constructed object.</param>
        /// <param name="parameters">The parameters representing the signature of the construction. They must match the loose objects of the defining configuration.</param>
        public ComposedConstruction(string name, Configuration configuration, IReadOnlyList<ConstructionParameter> parameters)
            : base(name, parameters, configuration.LastConstructedObject.ObjectType, isRandom: false)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            #region Checking if the passed parameters match the loose objects of the configuration

            // Local function that throws an exception if they don't
            static void ThrowException() => throw new GeoGenException("Incorrect composed construction: The loose objects don't correspond to the parameters");

            // Wrap the loose objects in a map and for each pair of [type, objects]....
            var looseObjectMap = new ConfigurationObjectMap(configuration.LooseObjects);

            // Make sure the number of the actual object types is the same as the number of the needed object types
            // If not, throw an exception...
            if (looseObjectMap.Count != Signature.ObjectTypesToNeededCount.Count)
                ThrowException();

            // Check if the number of objects of each type matched the needed count
            looseObjectMap.ForEach(pair =>
            {
                // Deconstruct
                var (objectType, looseObjects) = pair;

                // If not, throw an exception...
                if (looseObjects.Count != Signature.ObjectTypesToNeededCount[objectType])
                    ThrowException();
            });

            #endregion
        }

        #endregion
    }
}