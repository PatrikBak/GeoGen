using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString.ObjectIdResolving;

namespace GeoGen.Generator.ConfigurationHandling.ConfigurationsConstructing.IdsFixing
{
    interface IIdsFixerFactory
    {
        IIdsFixer CreateFixer(DictionaryObjectIdResolver resolver);
    }
}
