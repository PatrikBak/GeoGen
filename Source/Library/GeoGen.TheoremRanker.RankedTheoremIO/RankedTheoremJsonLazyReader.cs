using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;

namespace GeoGen.TheoremRanker.RankedTheoremIO
{
    /// <summary>
    /// The default implementation of <see cref="IRankedTheoremJsonLazyReader"/>.
    /// </summary>
    public class RankedTheoremJsonLazyReader : IRankedTheoremJsonLazyReader
    {
        /// <inheritdoc/>
        public IEnumerable<RankedTheorem> Read(string filePath)
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
                    var intermediateObject = JObject.Load(reader).ToObject<RankedTheoremIntermediate>();

                    // Return its converted version to the real object
                    yield return RankedTheoremIntermediate.Convert(intermediateObject);
                }
            }
        }
    }
}
