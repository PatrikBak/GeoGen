using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoGen.Core;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents an <see cref="ILooseObjectsIdResolver"/> that always returns the id of the object itself.
    /// </summary>
    public class IdentityLooseObjectIdResolver : ILooseObjectsIdResolver
    {
        public int ResolveId(LooseConfigurationObject looseObject)
        {
            return looseObject.Id;
        }
    }
}
