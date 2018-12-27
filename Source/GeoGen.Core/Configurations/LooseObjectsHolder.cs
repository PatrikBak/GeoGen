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
        /// Gets the layout in which these objects are arranged.
        /// </summary>
        public LooseObjectsLayout? Layout { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="LooseObjectsHolder"/> 
        /// instance wrapping the actual loose objects and possible their layout.
        /// </summary>
        /// <param name="looseObjects">The actual loose configurations objects.</param>
        /// <param name="layout">The layout of the objects. This parameter has the default value 'null'.</param>
        public LooseObjectsHolder(IEnumerable<LooseConfigurationObject> looseObjects, LooseObjectsLayout? layout = null)
        {
            LooseObjects = looseObjects?.ToList() ?? throw new ArgumentNullException(nameof(looseObjects));
            Layout = layout;

            // Verify the objects match the layout
            switch (layout)
            {
                case null:
                    // Specifying no layout is allowed
                    return;

                case LooseObjectsLayout.ScaleneAcuteAngledTriangled:

                    // Verify that we have exactly 3 objects and all points...
                    if (LooseObjects.Count != 3 || LooseObjects.Any(obj => obj.ObjectType != ConfigurationObjectType.Point))
                        throw new GeoGenException($"The loose objects don't match the {layout}");

                    return;
            }
        }

        #endregion
    }
}