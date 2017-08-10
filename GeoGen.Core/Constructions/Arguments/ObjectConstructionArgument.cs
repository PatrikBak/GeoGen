using System;
using GeoGen.Core.Configurations;

namespace GeoGen.Core.Constructions.Arguments
{
    /// <summary>
    /// Represent a <see cref="ConfigurationObject"/> that is passed as a <see cref="ConstructionArgument"/>. 
    /// </summary>
    public class ObjectConstructionArgument : ConstructionArgument
    {
        #region Public properties

        /// <summary>
        /// Gets the object that is passed to a construction.
        /// </summary>
        public ConfigurationObject PassedObject { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constrct a new object constuction argument wrapping a given passed configuration object.
        /// </summary>
        /// <param name="passedObject">The passed configuration object.</param>
        public ObjectConstructionArgument(ConfigurationObject passedObject)
        {
            PassedObject = passedObject ?? throw new ArgumentNullException(nameof(passedObject));
        }

        #endregion
    }
}
