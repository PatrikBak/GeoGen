using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoGen.Core.Objects
{
    public class LineObject : GeometricalObject
    {
        public List<PointObject> Points { get; set; }
    }
}
