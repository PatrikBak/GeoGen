using GeoGen.Constructor;
using GeoGen.Core;
using System.Collections.Generic;

namespace GeoGen.TheoremsFinder.new_stuff
{
    public interface ITypedTheoremsFinder
    {
        List<Theorem> FindAllTheorems(ContextualPicture picture);

        List<Theorem> FindNewTheorems(HierarchicalContextualPicture picture);
    }
}