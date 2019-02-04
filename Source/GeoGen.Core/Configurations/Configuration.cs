using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a configuration of geometrical objects. It consists of a <see cref="Core.LooseObjectsHolder"/>
    /// and a list of <see cref="ConstructedConfigurationObject"/>. The loose objects are the first objects to be 
    /// drawn (for example: in a triangle the loose objects are its 3 vertices. The constructed objects should to 
    /// be ordered so that it's possible to construct them in this order. The configuration should contain mutually
    /// distinct objects.
    /// </summary>
    public class Configuration : IdentifiedObject
    {
        #region Public properties

        /// <summary>
        /// Gets the holder of the loose objects of this configurations.
        /// </summary>
        public LooseObjectsHolder LooseObjectsHolder { get; }

        /// <summary>
        /// Gets the list of constructed configuration objects ordered in a way that we can construct them in this order.
        /// </summary>
        public IReadOnlyList<ConstructedConfigurationObject> ConstructedObjects { get; }

        /// <summary>
        /// Gets the configuration objects map containing all the objects of the configuration.
        /// </summary>
        public ConfigurationObjectsMap ObjectsMap { get; }

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
            ObjectsMap = new ConfigurationObjectsMap(looseObjectsHolder.LooseObjects.Cast<ConfigurationObject>().Concat(constructedObjects));
        }

        /// <summary>
        /// Initializes  a new instance of the <see cref="Configuration"/> class that consists of given objects.
        /// The loose objects will be automatically detected and will have a specified layout, which is 
        /// <see cref="LooseObjectsLayout.None"/> by default.
        /// </summary>
        /// <param name="layout">The layout of the loose objects, with the default value <see cref="LooseObjectsLayout.None"/>.</param>
        /// <param name="objects">The objects of the configuration.</param>
        public Configuration(LooseObjectsLayout layout = LooseObjectsLayout.None, params ConfigurationObject[] objects)
            : this(new LooseObjectsHolder(objects.OfType<LooseConfigurationObject>().ToList(), layout), objects.OfType<ConstructedConfigurationObject>().ToList())
        {
        }

        /// <summary>
        /// Initializes  a new instance of the <see cref="Configuration"/> class that consists of given objects.
        /// The loose objects will be automatically detected and will have <see cref="LooseObjectsLayout.None"/> layout.
        /// All the objects will be automatically identified.
        /// </summary>
        /// <param name="objects">The objects of the configuration.</param>
        public Configuration(params ConfigurationObject[] objects)
            : this(layout: LooseObjectsLayout.None, objects)
        {
            Identify(objects);
        }

        #endregion

        #region To String

        /// <summary>
        /// Converts a given configuration to a string. 
        /// NOTE: This method id used only for debugging purposes.
        /// </summary>
        /// <returns>A human-readable string representation of the configuration.</returns>
        public override string ToString() => ToStringHelper.ConfigurationToString(this);

        #endregion
    }
}