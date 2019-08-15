using GeoGen.Constructor;
using GeoGen.Core;

namespace GeoGen.TheoremsFinder.new_stuff
{
    public interface ITheoremsFinder
    {
        TheoremsMap FindAllTheorems(ContextualPicture picture);

        TheoremsMap FindNewTheorems(HierarchicalContextualPicture picture);
    }
}