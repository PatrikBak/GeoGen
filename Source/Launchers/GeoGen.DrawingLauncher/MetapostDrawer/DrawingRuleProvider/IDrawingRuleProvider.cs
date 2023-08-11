namespace GeoGen.DrawingLauncher
{
    /// <summary>
    /// Represents a service that gets <see cref="DrawingRule"/>s.
    /// </summary>
    public interface IDrawingRuleProvider
    {
        /// <summary>
        /// Gets drawing rules.
        /// </summary>
        /// <returns>The loaded drawing rules.</returns>
        Task<IReadOnlyList<DrawingRule>> GetDrawingRulesAsync();
    }
}