using GeoGen.Core;

namespace GeoGen.TheoremRanker
{
    /// <summary>
    /// Represents a thing that can be ranked on theorems. The higher the rank the more interesting a theorem is.
    /// </summary>
    public enum RankedAspect
    {
        /// <summary>
        /// The coefficient of theorem and configuration symmetry (<see cref="LooseObjectHolder.GetSymmetricMappings"/>.
        /// It is calculated as follows:
        /// <para>
        /// If any valid symmetry reordering of loose objects make the same configuration, then we have full symmetry and
        /// the ranking is 1. Otherwise if there is at least 1 such a reordering, then we have partial symmetry and
        /// the ranking is 0.5. Otherwise we have no symmetry and the ranking is 0.
        /// </para>
        /// <para>
        /// For example: 
        /// </para>
        /// <list type="number">
        /// <item>Triangle ABC with D = PointReflection(A, B) and theorem AB = BD has no symmetry, i.e. the ranking is 0.</item>
        /// <item>Triangle ABC with D = Midpoint(A, B) and theorem DA = DB has partial symmetry, i.e. the ranking is 0.5.</item>
        /// <item>Triangle ABC with midpoints of sides and concurrent medians has full symmetry, i.e. the ranking is 1.</item>
        /// </list>
        /// </summary>
        Symmetry,

        /// <summary>
        /// The degree to which a theorem can be stated in a simpler configuration. The coefficient is calculated as follows:
        /// <para>
        /// First we assign a 'level' to each object as follows:
        /// <list type="bullet">
        /// <item>If an object is a <see cref="LooseConfigurationObject"/>, then its level is 0.</item>
        /// <item>If an object is a <see cref="ConstructedConfigurationObject"/>, then its level is equal to the maximal
        /// level of its inner objects plus 1.</item>
        /// </list>
        /// The idea clearly is to depict how far a constructed object is from loose objects. This can then be used in a way
        /// that we take the theorem's inner <see cref="ConfigurationObject"/>s and average their levels.
        /// <para>
        /// Furthermore, the level calculated like this is always in the interval [1, NumberOfConstructedObjects]. To make 
        /// it suitable to compare configurations with different numbers of constructed objects we will normalize this rank
        /// by dividing it by the number of constructed objects.
        /// </para>
        /// <para>
        /// Finally, the ranking will be equal to 1 - this coefficient so that the higher the ranking we have the better the 
        /// theorem.
        /// </para>
        /// </para>
        /// </summary>
        Simplification,

        /// <summary>
        /// The total number of theorems of the configuration. The idea behind this metrics is that if we have more theorems in
        /// a configuration, then it usually suggests the problem is not that difficult, because we can make lots of conclusions. 
        /// This ranking might suggests a lot when there are very few theorems.
        /// </summary>
        Theorems,

        /// <summary>
        /// The number of <see cref="TheoremType.ConcyclicPoints"/> theorems. The idea behind this metrics is that in configurations 
        /// with more concyclic points it is usually easier to prove things, because of their powerful properties.
        /// </summary>
        Circles,

        /// <summary>
        /// The coefficient taking into account <see cref="TheoremType"/>. The value is taken from <see cref="TypeRankerSettings.TypeRankings"/>.
        /// </summary>
        TheoremType,

        /// <summary>
        /// The coefficient taking into account used <see cref="Construction"/>s. The values are taken from <see cref="TypeRankerSettings.TypeRankings"/>
        /// and the final ranking is the sum of these rankings over the configuration's constructed objects.
        /// </summary>
        Constructions,

        /// <summary>
        /// The coefficient taking into account used <see cref="Construction"/>s in a way that if a construction is used 'n' times,
        /// then its repetition coefficient is calculated as (n-1)^4. The idea is to punish constructions used more than 2 times.
        /// </summary>
        Repetition
    }
}