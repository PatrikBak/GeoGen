using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a holder of <see cref="LooseConfigurationObject"/>s of with a given
    /// <see cref="LooseObjectsLayout"/>. It takes care of identifying them.
    /// </summary>
    public class LooseObjectsHolder
    {
        #region Public properties

        /// <summary>
        /// Gets the list of loose objects.
        /// </summary>
        public IReadOnlyList<LooseConfigurationObject> LooseObjects { get; }

        /// <summary>
        /// Gets the layout in which these objects are arranged.
        /// </summary>
        public LooseObjectsLayout? Layout { get; internal set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="looseObjects">The loose objects.</param>
        /// <param name="layout">The loose objects layout.</param>
        public LooseObjectsHolder(IEnumerable<LooseConfigurationObject> looseObjects, LooseObjectsLayout? layout = null)
        {
            LooseObjects = looseObjects?.ToList() ?? throw new ArgumentNullException(nameof(looseObjects));
            Layout = layout;
        }

        #endregion
    }
}