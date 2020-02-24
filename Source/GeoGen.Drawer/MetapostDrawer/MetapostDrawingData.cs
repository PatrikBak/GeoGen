using System;
using System.Collections.Generic;

namespace GeoGen.Drawer
{
    /// <summary>
    /// The class containing information such as names of macros that are used by <see cref="MetapostFigure"/>
    /// to generate the actual code of figures.
    /// </summary>
    public class MetapostDrawingData
    {
        #region Public properties

        /// <summary>
        /// The name of the variable that represents how much we should scale every point.
        /// </summary>
        public string ScaleVariable { get; }

        /// <summary>
        /// The length by which shifted segments or lines should be shifted beyond a point. 
        /// This value is before scaling, it will be multiplied by <see cref="ScaleVariable"/>.
        /// </summary>
        public double ShiftLength { get; }

        /// <summary>
        /// The margin by which the calculated bounding box of all the points is shifted.
        /// The use-case for this is to accommodate labels which are drawn after the objects.
        /// </summary>
        public double BoundingBoxOffset { get; }

        /// <summary>
        /// The name of the macro that draws the label for a point and accepts two arguments,
        /// the label picture and the point where it should draw it.
        /// </summary>
        public string PointLabelMacro { get; }

        /// <summary>
        /// The dictionary mapping drawing styles to names of point mark macros handling this style.
        /// These macros should accept one argument, the point to be marked.
        /// </summary>
        public IReadOnlyDictionary<ObjectDrawingStyle, string> PointMarkMacros { get; }

        /// <summary>
        /// The dictionary mapping drawing styles to names of segment mark macros handling this style.
        /// These macros should accept two arguments, the points of the segment.
        /// </summary>
        public IReadOnlyDictionary<ObjectDrawingStyle, string> LineSegmentMacros { get; }

        /// <summary>
        /// The dictionary mapping drawing styles to names of circle macros handling this style.
        /// These macros should accept two arguments, the center and the radius.
        /// </summary>
        public IReadOnlyDictionary<ObjectDrawingStyle, string> CircleMacros { get; }

        /// <summary>
        /// The name of the macro that accepts one text parameter and draws it.
        /// </summary>
        public string TextMacro { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MetapostDrawingData"/> class.
        /// </summary>
        /// <param name="scaleVariable">The name of the variable that represents how much we should scale every point.</param>
        /// <param name="shiftLength">The length by which shifted segments or lines should be shifted beyond a point. </param>
        /// <param name="boundingBoxOffset">The margin by which the calculated bounding box of all the points is shifted.</param>
        /// <param name="pointLabelMacro">The name of the macro that draws the label for a point and accepts two arguments, the label picture and the point where it should draw it.</param>
        /// <param name="pointMarkMacros">The dictionary mapping drawing styles to names of point mark macros handling this style.</param>
        /// <param name="lineSegmentMacros">The dictionary mapping drawing styles to names of segment mark macros handling this style.</param>
        /// <param name="circleMacros">The dictionary mapping drawing styles to names of circle macros handling this style.</param>
        /// <param name="textMacro">The name of the macro that accepts one text parameter and draws it.</param>
        public MetapostDrawingData(string scaleVariable,
                                   double shiftLength,
                                   double boundingBoxOffset,
                                   string pointLabelMacro,
                                   IReadOnlyDictionary<ObjectDrawingStyle, string> pointMarkMacros,
                                   IReadOnlyDictionary<ObjectDrawingStyle, string> lineSegmentMacros,
                                   IReadOnlyDictionary<ObjectDrawingStyle, string> circleMacros,
                                   string textMacro)
        {
            ScaleVariable = scaleVariable ?? throw new ArgumentNullException(nameof(scaleVariable));
            ShiftLength = shiftLength;
            BoundingBoxOffset = boundingBoxOffset;
            PointLabelMacro = pointLabelMacro ?? throw new ArgumentNullException(nameof(pointLabelMacro));
            PointMarkMacros = pointMarkMacros ?? throw new ArgumentNullException(nameof(pointMarkMacros));
            LineSegmentMacros = lineSegmentMacros ?? throw new ArgumentNullException(nameof(lineSegmentMacros));
            CircleMacros = circleMacros ?? throw new ArgumentNullException(nameof(circleMacros));
            TextMacro = textMacro ?? throw new ArgumentNullException(nameof(textMacro));

            // Ensure the shift length is positive
            if (shiftLength <= 0)
                throw new ArgumentOutOfRangeException(nameof(shiftLength), "The shift length must be positive.");

            // Ensure the bounding box offset is positive
            if (boundingBoxOffset <= 0)
                throw new ArgumentOutOfRangeException(nameof(boundingBoxOffset), "The bounding box offset must be positive.");
        }

        #endregion
    }
}
