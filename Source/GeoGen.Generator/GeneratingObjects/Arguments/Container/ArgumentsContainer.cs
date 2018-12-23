using GeoGen.Core;
using GeoGen.Utilities;
using System.Collections.Generic;

namespace GeoGen.Generator
{
    public class ArgumentsContainer : StringBasedContainer<Arguments>
    {
        public ArgumentsContainer(DefaultArgumentsToStringConverter converter, IEnumerable<Arguments> initialObjects = null) 
            : base(converter, initialObjects)
        {
        }
    }
}
