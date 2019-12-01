using System.Collections.Generic;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// Represents a writer of <see cref="TheoremData"/>.
    /// </summary>
    public interface ITheoremDataWriter
    {
        /// <summary>
        /// Writes given theorems in this order.
        /// </summary>
        /// <param name="theorems">The enumerable of theorem data.</param>
        void WriteTheorems(IEnumerable<TheoremData> theorems);
    }
}