using System.Collections.Generic;
using GeoGen.Core.Configurations;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a holder of the loose configuration objects of 
    /// a single generation process.  
    /// </summary>
    internal interface ILooseObjectsHolder
    {
        /// <summary>
        /// Gets the identified loose objects that are common for a single
        /// generation process. 
        /// </summary>
        IEnumerable<LooseConfigurationObject> LooseObjects { get; }
    }
}