using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoGen.Analyzer.Objects;
using GeoGen.Core.Configurations;

namespace GeoGen.Analyzer.Constructing.Constructors
{
    internal interface ILooseObjectsConstructor
    {
        List<GeometricalObject> Construct(IEnumerable<LooseConfigurationObject> looseObjects);
    }
}
