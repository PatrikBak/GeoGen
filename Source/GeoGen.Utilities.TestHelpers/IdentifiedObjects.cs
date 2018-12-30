using GeoGen.Core;

namespace GeoGen.Utilities.TestHelpers
{
    /// <summary>
    /// Static helpers for dealing with <see cref="IdentifiedObject"/>s. These methods
    /// are intended to be used for debugging purposes and testing.
    /// </summary>
    public static class IdentifiedObjects
    {
        /// <summary>
        /// Identifies the given objects, starting from 0.
        /// </summary>
        /// <param name="objects">The objects to be identified</param>
        public static void Identify(params IdentifiedObject[] objects) => objects.ForEach((obj, index) => obj.Id = index);
    }
}
