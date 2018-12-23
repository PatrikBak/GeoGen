using GeoGen.Core;
using System.Collections.Generic;

namespace GeoGen.Generator
{
    public interface IFullObjectToStringConverterFactory
    {
        IFullObjectToStringConverter CreateConverter(ILooseObjectsIdResolver resolver);
    }
}
