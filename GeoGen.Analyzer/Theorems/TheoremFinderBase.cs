using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoGen.Analyzer.Objects;
using GeoGen.Core.Configurations;
using GeoGen.Core.Theorems;
using GeoGen.Core.Utilities;

namespace GeoGen.Analyzer.Theorems
{
    internal abstract class TheoremFinderBase : ITheoremFinder
    {
        public abstract IEnumerable<Theorem> Find(ConfigurationObjectsMap oldObjects, ConfigurationObjectsMap newObjects);

        protected IEnumerable<List<T>> UnundorredElements<T>(List<T> objects, int count)
        {
            return null;
            
        }
    }
}