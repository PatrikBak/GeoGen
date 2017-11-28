using System.Collections.Generic;
using GeoGen.Core.Theorems;

namespace GeoGen.Analyzer.Theorems
{
    internal interface ITheoremsContainer : IEnumerable<Theorem>
    {
        void Add(Theorem theorem);

        bool Contains(Theorem theorem);
    }
}