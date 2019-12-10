using GeoGen.Core;
using System;
using System.Collections.Generic;

namespace GeoGen.Drawer
{
    /// <summary>
    /// Represents a description of how to draw a <see cref="ConstructedConfigurationObject"/>.
    /// </summary>
    public class DrawingRule
    {
        #region Public properties

        /// <summary>
        /// The template of an object to be drawn. Its argument are assumed to be <see cref="LooseConfigurationObject"/>s.
        /// </summary>
        public ConstructedConfigurationObject ObjectToDraw { get; }

        /// <summary>
        /// The list of auxiliary objects that are needed to be constructed in order to draw what's needed.
        /// </summary>
        public IReadOnlyList<ConstructedConfigurationObject> AuxiliaryObjects { get; }

        /// <summary>
        /// The commands containing the actual information about what should be drawn.
        /// </summary>
        public IReadOnlyList<DrawingCommand> DrawingCommands { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DrawingRule"/> class.
        /// </summary>
        /// <param name="objectToDraw">The template of an object to be drawn. Its argument are assumed to be <see cref="LooseConfigurationObject"/>s.</param>
        /// <param name="auxiliaryObjects">The list of auxiliary objects that are needed to be constructed in order to draw what's needed.</param>
        /// <param name="drawingCommands">The commands containing the actual information about what should be drawn.</param>
        public DrawingRule(ConstructedConfigurationObject objectToDraw,
                           IReadOnlyList<ConstructedConfigurationObject> auxiliaryObjects,
                           IReadOnlyList<DrawingCommand> drawingCommands)
        {
            ObjectToDraw = objectToDraw ?? throw new ArgumentNullException(nameof(objectToDraw));
            AuxiliaryObjects = auxiliaryObjects ?? throw new ArgumentNullException(nameof(auxiliaryObjects));
            DrawingCommands = drawingCommands ?? throw new ArgumentNullException(nameof(drawingCommands));
        }

        #endregion
    }
}
