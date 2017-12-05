using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoGen.Core.Configurations;

namespace GeoGen.Analyzer
{
    public interface IAnalyzerInitializer
    {
        void Initialize(Configuration initialConfiguration);
    }
}
