using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoGen.Analyzer
{
    public class InconsistentContainersException : AnalyzerException
    {
        public InconsistentContainersException(string message)
            : base(message)
        {
        }
    }
}