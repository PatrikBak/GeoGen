using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// The default implementation of <see cref="ITheoremWithRankingJsonLazyReader"/>.
    /// </summary>
    public class TheoremWithRankingJsonLazyReader : ITheoremWithRankingJsonLazyReader
    {
        /// <summary>
        /// Lazily reads a given file where <see cref="TheoremWithRanking"/> objects have been written.
        /// </summary>
        /// <param name="filePath">The path to the file with theorems with ranking.</param>
        /// <returns>An enumerable of read theorems with ranking.</returns>
        public IEnumerable<TheoremWithRanking> Read(string filePath)
        {
            // Prepare the reader of the file
            using JsonTextReader reader = new JsonTextReader(new StreamReader(new FileStream(filePath, FileMode.Open, FileAccess.Read)));

            // Read until the end
            while (reader.Read())
            {
                // If we have a token that starts an object...
                if (reader.TokenType == JsonToken.StartObject)
                {
                    // Load the object from the stream and convert it to our intermediate one
                    var intermediateObject = JObject.Load(reader).ToObject<TheoremWithRankingIntermediate>();

                    // Return its converted version to the real object
                    yield return TheoremWithRankingIntermediate.Convert(intermediateObject);
                }
            }
        }
    }
}
