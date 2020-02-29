using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace GeoGen.MainLauncher
{
    /// <summary>
    /// An implementation of <see cref="IRankedTheoremJsonLazyWriter"/> that writes a long JSON array
    /// of intermediate <see cref="RankedTheoremIntermediate"/> objects.
    /// </summary>
    public class RankedTheoremJsonLazyWriter : IRankedTheoremJsonLazyWriter
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
        /// Initializes a new instance of the <see cref="RankedTheoremJsonLazyWriter"/> class.
        /// </summary>
        /// <param name="filePath">The path to the file to write to.</param>
        public RankedTheoremJsonLazyWriter(string filePath)
        {
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        }

        #endregion

        #region IRankedTheoremJsonLazyWriter implementation

        /// <summary>
        /// Begins lazy writing.
        /// </summary>
        public void BeginWriting()
        {
            // Check if we haven't begun
            if (HasWritingBegun)
                throw new MainLauncherException("Writing has already begun.");

            // Create the writer
            _writingStream = new StreamWriter(new FileStream(_filePath, FileMode.Create, FileAccess.Write));

            // Write the start of the array
            _writingStream.Write("[");
        }

        /// <summary>
        /// Writes a given ranked theorem.
        /// </summary>
        /// <param name="rankedTheorems">The ranked theorems to be written.</param>
        public void Write(IEnumerable<RankedTheorem> rankedTheorems)
        {
            // Check if we have begun
            if (!HasWritingBegun)
                throw new MainLauncherException("Writing hasn't begun yet.");

            // Go through the theorems 
            foreach (var rankedTheorem in rankedTheorems)
            {
                // If we haven written anything, write a colon to separate entries
                if (_firstEntryWritten)
                    _writingStream.Write(",");
                // Otherwise mark that we've written something (we're about to)
                else
                    _firstEntryWritten = true;

                // Serialize the theorem
                var theoremString = JsonConvert.SerializeObject(RankedTheoremIntermediate.Convert(rankedTheorem));

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
                throw new MainLauncherException("Writing hasn't begun yet.");

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
