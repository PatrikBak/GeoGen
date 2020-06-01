using GeoGen.Core;

namespace GeoGen.TheoremRanker
{
    /// <summary>
    /// Represents a thing that can be ranked on theorems. The higher the rank the more interesting a theorem is.
    /// </summary>
    public enum RankedAspect
    {
        /// <summary>
        /// The coefficient of theorem and configuration symmetry (<see cref="LooseObjectHolder.GetSymmetricMappings"/>).
        /// It is calculated by dividing the number of valid symmetry mappings by the total number of symmetry mappings. 
        /// </summary>
        Symmetry,

        /// <summary>
        /// The aggregated level of the objects of the configuration where the ranked theorem holds. The metrics is based
        /// on <see cref="Configuration.CalculateObjectLevels"/>. 
        /// <para>
        /// The basic idea is the following: Problems with smaller levels are easier to understand, and therefore usually prettier. 
        /// If they have enough objects, it means they are even more difficult.
        /// </para>
        /// <para>
        /// The value is calculated as follows: First we calculate the sum of squares of the object levels, call it S. 
        /// Then, let n be the number of constructed objects. It holds that n <= S <= 1^2 + ... + n^2 = n(n+1)(2n+1)/6.
        /// The final result is equal 1 for ń<=1 and otherwise 1 - (S-n)/ (n(n+1)(2n+1)/6 - n), which can be 'simplifed'
        /// to 1 - 6 (S-n) / (n(n-1)(2n+5)).
        /// </para>
        /// <para>
        /// <list type="bullet">
        /// <item>It can be seen that the ranking is always in the interval [0,1].</item>
        /// <item>The idea behind squaring levels is to punish smaller levels more.</item>
        /// <item>The value is 1 - something in order to keep the invariant that the higher the level the better the problem.</item>
        /// <item>This ranking takes into account only the configuration where the theorem holds.</item>
        /// </list>
        /// </para>
        /// </summary>
        Level,

        /// <summary>
        /// The total number of theorems of the configuration, minus the one we're ranking. The idea behind this metrics 
        /// is that if we have more theorems in a configuration, then it usually suggests the problem is not that difficult,
        /// because we can make lots of conclusions. This ranking might suggests a lot when there are very few theorems.
        /// </summary>
        NumberOfTheorems,

        /// <summary>
        /// The number of <see cref="TheoremType.ConcyclicPoints"/> theorems, potentially minus one we're ranking (if it is
        /// about proving concyclic points). The idea behind this metrics is that in configurations with more concyclic points 
        /// it is usually easier to prove things, because of their powerful properties.
        /// </summary>
        NumberOfCyclicQuadrilaterals
    }
}