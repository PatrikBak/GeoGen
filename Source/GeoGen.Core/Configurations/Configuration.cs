using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a configuration of geometrical objects. It consists of 
    /// a list of <see cref="LooseConfigurationObject"/> and a list of 
    /// <see cref="ConstructedConfigurationObject"/>. The loose objects are 
    /// the first objects to be drawn (for example:in a triangle, they would be 
    /// 3 points). The constructed objects are supposed to be ordered so that it's
    /// possible to construct them in that order. The order of the loose objects 
    /// is important when the configuration defines a <see cref="ComposedConstruction"/>. 
    /// All objects are supposed to be mutually distinct.
    /// </summary>
    public class Configuration : IdentifiedObject
    {
        #region Public properties

        /// <summary>
        /// Gets the loose configuration objects wrapped in a holder.
        /// </summary>
        public LooseObjectsHolder LooseObjectsHolder { get; }

        /// <summary>
        /// Gets the list of loose configuration objects within this configuration.
        /// </summary>
        public IReadOnlyList<LooseConfigurationObject> LooseObjects => LooseObjectsHolder.LooseObjects;

        /// <summary>
        /// Gets or sets the layout of the loose objects of the configuration. This value doesn't have
        /// to be set, then loose objects are constructed randomly. But if it's set, it
        /// must logically correspond to the loose objects property. 
        /// </summary>
        public LooseObjectsLayout? LooseObjectsLayout
        {
            get => LooseObjectsHolder.Layout;
            set => LooseObjectsHolder.Layout = value;
        }

        /// <summary>
        /// Gets the list of constructed configuration objects within this configuration. 
        /// They're supposed be ordered so that it's possible to construct them in that order.
        /// </summary>
        public IReadOnlyList<ConstructedConfigurationObject> ConstructedObjects { get; }

        /// <summary>
        /// Gets the configuration objects map of this configuration.
        /// </summary>
        public ConfigurationObjectsMap ObjectsMap { get; }

        /// <summary>
        /// Gets the number of objects of this configuration
        /// </summary>
        public int NumberOfObjects => LooseObjects.Count + ConstructedObjects.Count;

        /// <summary>
        /// Creates a set of the ids of the objects of this configuration.
        /// </summary>
        /// <returns></returns>
        public HashSet<int> ObjectsIds()
        {
            // Cast all objects to the set of their ids
            return ObjectsMap.AllObjects.Select(obj => obj.Id).ToSet();
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="looseObjects">The list of loose configuration objects.</param>
        /// <param name="constructedObjects">The list of constructed configuration objects.</param>
        /// <param name="layout">The layout of loose objects.</param>
        public Configuration(IReadOnlyList<LooseConfigurationObject> looseObjects, IReadOnlyList<ConstructedConfigurationObject> constructedObjects, LooseObjectsLayout? layout = null)
                : this(new LooseObjectsHolder(looseObjects, layout), constructedObjects)
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="looseObjectsHolder">The loose objects holder.</param>
        /// <param name="constructedObjects">The constructed objects.</param>
        public Configuration(LooseObjectsHolder looseObjectsHolder, IReadOnlyList<ConstructedConfigurationObject> constructedObjects)
        {
            LooseObjectsHolder = looseObjectsHolder;
            ConstructedObjects = constructedObjects ?? throw new ArgumentNullException(nameof(constructedObjects));
            ObjectsMap = new ConfigurationObjectsMap(looseObjectsHolder.LooseObjects.Cast<ConfigurationObject>().Concat(constructedObjects));
        }

        #endregion

        /// <summary>
        /// This method will be removed soon.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<List<ConstructedConfigurationObject>> GroupConstructedObjects()
        {
            return ConstructedObjects.Select(o => new List<ConstructedConfigurationObject> { o });
        }
    }
}