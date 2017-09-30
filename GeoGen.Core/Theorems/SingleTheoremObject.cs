using System;
using System.Collections.Generic;
using GeoGen.Core.Configurations;

namespace GeoGen.Core.Theorems
{
    /// <summary>
    /// Represents a <see cref="TheoremObject"/> that directly represents an
    /// associated configuration object.
    /// </summary>
    public class SingleTheoremObject : TheoremObject
    {
        #region TeoremObjects properties

        /// <summary>
        /// Gets the type of this object.
        /// </summary>
        public override ConfigurationObjectType Type { get; }

        /// <summary>
        /// Gets the objects that this objects is made of. For example: If this is 
        /// a line, then this could for example a list of two points, or a list...
        /// </summary>
        public override List<ConfigurationObject> InternalObjects { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a single theorem object that represents a given
        /// configuration object.
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        public SingleTheoremObject(ConfigurationObject configurationObject)
        {
            if (configurationObject == null)
                throw new ArgumentNullException(nameof(configurationObject));

            Type = configurationObject.ObjectType;
            InternalObjects = new List<ConfigurationObject> { configurationObject };
        } 

        #endregion
    }
}