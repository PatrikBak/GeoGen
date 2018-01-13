using System.Collections.Generic;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    internal interface ITheoremsContainer : IEnumerable<Theorem>
    {
        void Add(Theorem theorem);

        bool Contains(Theorem theorem);
    }
}