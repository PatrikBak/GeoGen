using System;
using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Generator.ConstructingConfigurations;

namespace GeoGen.Generator.ConstructingObjects
{
    /// <summary>
    /// An internal class represeting the output from a constructor.
    /// </summary>
    internal class ConstructorOutput : IComparable<ConstructorOutput>
    {
        /// <summary>
        /// Gets or sets the initial wrapped configuration that was extended.
        /// </summary>
        public ConfigurationWrapper InitialConfiguration { get; set; }

        /// <summary>
        /// Gets or set the list of outputed constructed configuration objects.
        /// </summary>
        public List<ConstructedConfigurationObject> ConstructedObjects { get; set; }

        // TODO: Remove this debug code
        public int Id { get; set; }
        public int CompareTo(ConstructorOutput other)
        {
            return Id - other.Id;
        }
    }
}