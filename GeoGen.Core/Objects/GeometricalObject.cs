using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoGen.Core.Configurations;

namespace GeoGen.Core.Objects
{
    public abstract class GeometricalObject
    {
        public int Id { get; set; }

        public ConfigurationObject Object { get; set; }
    }
}
