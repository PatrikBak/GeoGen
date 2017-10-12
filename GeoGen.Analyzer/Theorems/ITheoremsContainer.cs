using GeoGen.Core.Theorems;

namespace GeoGen.Analyzer.Theorems
{
    internal interface ITheoremsContainer
    {
        void Add(Theorem theorem);

        bool Contains(Theorem theorem);
    }
}