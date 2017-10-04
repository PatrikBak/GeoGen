using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoGen.Core.Configurations;

namespace GeoGen.Analyzer.Objects
{
    internal class RegistrationResult
    {
        public bool CanBeConstructed { get; set; }

        public Dictionary<ConfigurationObject, ConfigurationObject> DuplicateObjects { get; set; }
    }
}
