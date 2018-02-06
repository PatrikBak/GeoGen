using System.Collections.Generic;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    internal interface ITheoremsContainer : IEnumerable<Theorem>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="theorem"></param>
        void Add(Theorem theorem);

        bool Contains(Theorem theorem);
    }
}