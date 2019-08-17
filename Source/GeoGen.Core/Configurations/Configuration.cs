using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a configuration of geometric objects. It consists of a <see cref="Core.LooseObjectsHolder"/>
    /// and a list of <see cref="ConstructedConfigurationObject"/>. The loose objects are the first objects to be 
    /// drawn (for example: in a triangle the loose objects are its 3 vertices. The constructed objects should to 
    /// be ordered so that it's possible to construct them in this order. The configuration should contain mutually
    /// distinct objects.
    /// </summary>
    public class Configuration
    {
        #region Public properties

        /// <summary>
        /// Gets the holder of the loose objects of this configurations.
        /// </summary>
        public LooseObjectsHolder LooseObjectsHolder { get; }

        /// <summary>
        /// Gets the loose object of the configuration.
        /// </summary>
        public IReadOnlyList<LooseConfigurationObject> LooseObjects => LooseObjectsHolder.LooseObjects;

        /// <summary>
        /// Gets the list of constructed configuration objects ordered in a way that we can construct them in this order.
        /// </summary>
        public IReadOnlyList<ConstructedConfigurationObject> ConstructedObjects { get; }

        /// <summary>
        /// Gets the last constructed object of the configuration.
        /// </summary>
        public ConstructedConfigurationObject LastConstructedObject => ConstructedObjects.LastOrDefault() ?? throw new GeoGenException("No constructed objects");

        /// <summary>
        /// Gets the configuration objects map containing all the objects of the configuration.
        /// </summary>
        public ConfigurationObjectsMap ObjectsMap { get; }

        /// <summary>
        /// Gets all configuration objects of the configuration.
        /// </summary>
        public IReadOnlyList<ConfigurationObject> AllObjects => ObjectsMap.AllObjects;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Configuration"/> class.
        /// </summary>
        /// <param name="looseObjectsHolder">The holder of the loose objects of this configurations.</param>
        /// <param name="constructedObjects">The list of constructed configuration objects ordered in a way that we can construct them in this order.</param>
        public Configuration(LooseObjectsHolder looseObjectsHolder, IReadOnlyList<ConstructedConfigurationObject> constructedObjects)
        {
            LooseObjectsHolder = looseObjectsHolder;
            ConstructedObjects = constructedObjects ?? throw new ArgumentNullException(nameof(constructedObjects));
            ObjectsMap = new ConfigurationObjectsMap(LooseObjects.Cast<ConfigurationObject>().Concat(constructedObjects));
        }

        /// <summary>
        /// Initializes  a new instance of the <see cref="Configuration"/> class that consists of given objects.
        /// The loose objects will be automatically detected and will have a specified layout.
        /// </summary>
        /// <param name="layout">The layout of the loose objects./>.</param>
        /// <param name="objects">The objects of the configuration.</param>
        private Configuration(LooseObjectsLayout layout, params ConfigurationObject[] objects)
            : this(new LooseObjectsHolder(objects.OfType<LooseConfigurationObject>().ToList(), layout), objects.OfType<ConstructedConfigurationObject>().ToList())
        {
        }

        #endregion

        #region Public static methods

        /// <summary>
        /// Creates a configuration that simulates the construction of given constructed objects.
        /// The loose objects will be automatically detected and will have the specified layout.
        /// </summary>
        /// <param name="layout">The layout for the automatically detected loose objects.</param>
        /// <param name="objects">The objects whose construction defines the configuration.</param>
        /// <returns>The configuration derived from the objects.</returns>
        public static Configuration DeriveFromObjects(LooseObjectsLayout layout, params ConfigurationObject[] objects) => new Configuration(layout, objects.GetDefiningObjects().ToArray());

        #endregion

        #region To String

        /// <summary>
        /// Converts the configuration to a string. 
        /// NOTE: This method is used only for debugging purposes.
        /// </summary>
        /// <returns>A human-readable string representation of the configuration.</returns>
        public override string ToString() => AllObjects.Select(configurationObject => configurationObject.ToString()).ToJoinedString("; ");

        #endregion
    }
}