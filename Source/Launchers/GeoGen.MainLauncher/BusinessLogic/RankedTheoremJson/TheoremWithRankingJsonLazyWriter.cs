using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace GeoGen.MainLauncher
{
    /// <summary>
    /// An implementation of <see cref="ITheoremWithRankingJsonLazyWriter"/> that writes a long JSON array
    /// of intermediate <see cref="TheoremWithRankingIntermediate"/> objects.
    /// </summary>
    public class TheoremWithRankingJsonLazyWriter : ITheoremWithRankingJsonLazyWriter
    {
        #region Private fields

        /// <summary>
        /// The path to the file to write to.
        /// </summary>
        private readonly string _filePath;

        /// <summary>
        /// The stream that is used for writing the content of the file.
        /// </summary>
        private StreamWriter _writingStream;

        /// <summary>
        /// Indicates whether we have written the first entry already.
        /// </summary>
        private bool _firstEntryWritten;

        #endregion

        #region Private properties

        /// <summary>
        /// Indicates whether writing of the file has already begun.
        /// </summary>
        private bool HasWritingBegun => _writingStream != null;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremWithRankingJsonLazyWriter"/> class.
        /// </summary>
        /// <param name="filePath">The path to the file to write to.</param>
        public TheoremWithRankingJsonLazyWriter(string filePath)
        {
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        }

        #endregion

        #region ITheoremsWithRankingLazyWriter implementation

        /// <summary>
        /// Begins lazy writing.
        /// </summary>
        public void BeginWriting()
        {
            // Check if we haven't begun
            if (HasWritingBegun)
                throw new InvalidOperationException("Writing has already begun.");

            // Create the writer
            _writingStream = new StreamWriter(new FileStream(_filePath, FileMode.Create, FileAccess.Write));

            // Write the start of the array
            _writingStream.Write("[");
        }

        /// <summary>
        /// Writes a given theorem with ranking.
        /// </summary>
        /// <param name="theoremsWithRanking">The theorems ranking to be written.</param>
        public void Write(IEnumerable<TheoremWithRanking> theoremsWithRanking)
        {
            // Check if we have begun
            if (!HasWritingBegun)
                throw new InvalidOperationException("Writing hasn't begun yet.");

            // Go through the theorems 
            foreach (var theoremWithRanking in theoremsWithRanking)
            {
                // If we haven written anything, write a colon to separate entries
                if (_firstEntryWritten)
                    _writingStream.Write(",");
                // Otherwise mark that we've written something (we're about to)
                else
                    _firstEntryWritten = true;

                // Serialize the theorem
                var theoremString = JsonConvert.SerializeObject(TheoremWithRankingIntermediate.Convert(theoremWithRanking));

                // Write it
                _writingStream.Write(theoremString);
            }

            // Flush
            _writingStream.Flush();
        }

        /// <summary>
        /// Finishes lazy writing.
        /// </summary>
        public void EndWriting()
        {
            // Check if we have begun
            if (!HasWritingBegun)
                throw new InvalidOperationException("Writing hasn't begun yet.");

            // Write the end of the array
            _writingStream.Write("]");

            // Flush
            _writingStream.Flush();

            // Close the writer
            _writingStream.Dispose();

            // Make sure we're ready to write again by reseting to the default state
            _firstEntryWritten = false;
            _writingStream = null;
        }

        #endregion
    }
}
