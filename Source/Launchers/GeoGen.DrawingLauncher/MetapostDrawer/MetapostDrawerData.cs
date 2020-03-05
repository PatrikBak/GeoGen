using GeoGen.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.DrawingLauncher
{
    /// <summary>
    /// The data for <see cref="MetapostDrawer"/>.
    /// </summary>
    public class MetapostDrawerData
    {
        #region Public properties

        /// <summary>
        /// The dictionary mapping construction to the rules explaining what should be drawn while performing them.
        /// </summary>
        public IReadOnlyDictionary<Construction, DrawingRule> DrawingRules { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MetapostDrawerData"/>.
        /// </summary>
        /// <param name="drawingRules">The available drawing rules.</param>
        public MetapostDrawerData(IEnumerable<DrawingRule> drawingRules)
        {
            DrawingRules = drawingRules?.ToDictionary(rule => rule.ObjectToDraw.Construction) ?? throw new ArgumentNullException(nameof(drawingRules));
        }

        #endregion
    }
}