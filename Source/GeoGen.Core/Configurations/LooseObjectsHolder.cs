using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a wrapper of <see cref="LooseConfigurationObject"/>s together with their <see cref="LooseObjectsLayout"/>.
    /// </summary>
    public class LooseObjectsHolder
    {
        #region Public properties

        /// <summary>
        /// Gets the list of actual loose configurations objects held by this instance.
        /// </summary>
        public IReadOnlyList<LooseConfigurationObject> LooseObjects { get; }

        /// <summary>
        /// Gets the loose objects wrapped in an objects map.
        /// </summary>
        public ConfigurationObjectsMap ObjectsMap { get; }

        /// <summary>
        /// Gets the layout in which these objects are arranged.
        /// </summary>
        public LooseObjectsLayout Layout { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="LooseObjectsHolder"/> 
        /// instance wrapping the actual loose objects and possible their layout.
        /// </summary>
        /// <param name="looseObjects">The actual loose configurations objects.</param>
        /// <param name="layout">The layout of these loose objects.</param>
        public LooseObjectsHolder(IEnumerable<LooseConfigurationObject> looseObjects, LooseObjectsLayout layout = default)
        {
            LooseObjects = looseObjects?.ToList() ?? throw new ArgumentNullException(nameof(looseObjects));
            ObjectsMap = new ConfigurationObjectsMap(looseObjects);
            Layout = layout;

            // If the layout is specified make sure the objects match it 
            if (layout != LooseObjectsLayout.NoLayout && !LooseObjects.Select(o => o.ObjectType).SequenceEqual(layout.ObjectTypes()))
                throw new GeoGenException($"The loose objects don't match the specified layout.");
        }

        #endregion
    }
}