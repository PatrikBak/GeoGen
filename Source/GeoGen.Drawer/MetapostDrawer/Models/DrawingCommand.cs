using GeoGen.Core;
using System;
using System.Collections.Generic;

namespace GeoGen.Drawer
{
    /// <summary>
    /// Represents a command that perform the actual drawing specified by <see cref="Type"/> of an object
    /// specified inside <see cref="Arguments"/>, being drawn with a certain <see cref="Style"/>.
    /// </summary>
    public class DrawingCommand
    {
        #region Public properties

        /// <summary>
        /// The type of this command.
        /// </summary>
        public DrawingCommandType Type { get; }

        /// <summary>
        /// The style of how we want to perform this command.
        /// </summary>
        public ObjectDrawingStyle Style { get; }

        /// <summary>
        /// The actual objects to be drawn. Their number or type depends on the <see cref="Type"/>.
        /// </summary>
        public IReadOnlyList<ConfigurationObject> Arguments { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DrawingCommand"/> class.
        /// </summary>
        /// <param name="type">The type of this command.</param>
        /// <param name="style">The style of how we want to perform this command.</param>
        /// <param name="arguments">The actual objects to be drawn. Their number or type depends on the <see cref="Type"/>.</param>
        public DrawingCommand(DrawingCommandType type, ObjectDrawingStyle style, IReadOnlyList<ConfigurationObject> arguments)
        {
            Type = type;
            Style = style;
            Arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
        }

        #endregion
    }
}
