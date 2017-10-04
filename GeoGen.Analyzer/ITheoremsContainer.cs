using System.Collections.Generic;
using GeoGen.Core.Theorems;

namespace GeoGen.Analyzer
{
    internal interface ITheoremsContainer
    {
        bool Add(Theorem theorem);
    }
}