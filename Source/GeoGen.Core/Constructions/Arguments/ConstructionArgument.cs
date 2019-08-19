using GeoGen.Utilities;
using System.Collections.Generic;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a holder of <see cref="ConfigurationObject"/>(s) that together with 
    /// a <see cref="Construction"/> represent <see cref="ConstructedConfigurationObject"/>s.
    /// </summary>
    public abstract class ConstructionArgument
    {
        #region Public static properties

        /// <summary>
        /// Gets the single instance of the equality comparer of two construction arguments that uses the 
        /// <see cref="IsEquivalentTo(ConstructionArgument)"/> method and a constant hash code function
        /// (i.e. using it together with a hash map / hash set would make all the operations O(n)).
        /// </summary>
        public static readonly IEqualityComparer<ConstructionArgument> EquivalencyComparer = new SimpleEqualityComparer<ConstructionArgument>(
            // Reuse the abstract method on the object
            (t1, t2) => t1.IsEquivalentTo(t2),
            // And the constant hash-code function
            t => 0);

        #endregion

        #region Public abstract methods

        /// <summary>
        /// Finds out if this argument is equivalent to another given one.
        /// Constructed objects created from equivalent arguments are equivalent.
        /// </summary>
        /// <param name="otherArgument">The other argument.</param>
        /// <returns>true, if they are equivalent; false otherwise.</returns>
        public abstract bool IsEquivalentTo(ConstructionArgument otherArgument);

        #endregion
    }
}