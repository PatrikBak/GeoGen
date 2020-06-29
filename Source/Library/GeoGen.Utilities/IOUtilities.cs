using System.IO;

namespace GeoGen.Utilities
{
    /// <summary>
    /// Static IO utilities.
    /// </summary>
    public static class IOUtilities
    {
        /// <summary>
        /// Empties the directory with a given path, provided it exists.
        /// </summary>
        /// <param name="directoryPath">The path to the directory to be cleared. This value may be null.</param>
        public static void ClearDirectoryIfItExists(string directoryPath)
        {
            // If the directory is null, nothing to do
            if (directoryPath == null)
                return;

            // If the directory doesn't exist, nothing to do
            if (!Directory.Exists(directoryPath))
                return;

            // Otherwise, delete all the subdirectories
            Directory.EnumerateFiles(directoryPath).ForEach(File.Delete);

            // And all the files
            Directory.EnumerateDirectories(directoryPath).ForEach(directory => Directory.Delete(directory, recursive: true));
        }
    }
}
