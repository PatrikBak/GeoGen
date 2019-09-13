using GeoGen.Core;
using GeoGen.TheoremProver;
using System;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// Represents a template <see cref="Theorem"/> used by the <see cref="ITheoremProver"/>
    /// loaded from a theorems file.
    /// </summary>
    public class TemplateTheorem : Theorem
    {
        #region Public properties

        /// <summary>
        /// Gets the file name from which the theorem was loaded.
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// Gets the number of the theorem in the file.
        /// </summary>
        public int Number { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateTheorem"/> theorem.
        /// </summary>
        /// <param name="theorem">The theorem that is wrapped.</param>
        /// <param name="fileName">The file name from which the theorem was loaded.</param>
        /// <param name="number">The number of the theorem in the file.</param>
        public TemplateTheorem(Theorem theorem, string fileName, int number)
            : base(theorem.Configuration, theorem.Type, theorem.InvolvedObjects)
        {
            FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
            Number = number;
        }

        #endregion
    }
}
