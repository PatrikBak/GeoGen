using System;
using System.Collections.Generic;

namespace GeoGen.DrawingLauncher
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
        /// The minimal width that a picture must have before we decide to be clipping it because
        /// it is too large (most likely because of circles).
        /// </summary>
        public double MinimalWidthForClipping { get; }

        /// <summary>
        /// The minimal height that a picture must have before we decide to be clipping it because
        /// it is too large (most likely because of circles).
        /// </summary>
        public double MinimalHeightForClipping { get; }

        /// <summary>
        /// The minimal angle in degrees corresponding to the arc that might be clipped because the 
        /// circle makes the figure large.
        /// </summary>
        public double MinimalAngleOfClippedCircleArc { get; }

        /// <summary>
        /// The scale that is applied to the bounding box enclosing all the points of the figure.
        /// The idea is to set this value so that the scaled box includes potential point labels.
        /// </summary>
        public double PointBoundingBoxScale { get; }

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
        /// <param name="scaleVariable"><inheritdoc cref="ScaleVariable" path="/summary"/></param>
        /// <param name="shiftLength"><inheritdoc cref="ShiftLength" path="/summary"/></param>
        /// <param name="minimalWidthForClipping"><inheritdoc cref="MinimalWidthForClipping" path="/summary"/></param>
        /// <param name="minimalHeightForClipping"><inheritdoc cref="MinimalHeightForClipping" path="/summary"/></param>
        /// <param name="pointBoundingBoxScale"><inheritdoc cref="PointBoundingBoxScale" path="/summary"/></param>
        /// <param name="pointLabelMacro"><inheritdoc cref="PointLabelMacro" path="/summary"/></param>
        /// <param name="pointMarkMacros"><inheritdoc cref="PointMarkMacros" path="/summary"/></param> 
        /// <param name="lineSegmentMacros"><inheritdoc cref="LineSegmentMacros" path="/summary"/></param>
        /// <param name="circleMacros"><inheritdoc cref="CircleMacros" path="/summary"/></param>
        /// <param name="textMacro"><inheritdoc cref="TextMacro" path="/summary"/></param>
        public MetapostDrawingData(string scaleVariable,
                                   double shiftLength,
                                   double minimalWidthForClipping,
                                   double minimalHeightForClipping,
                                   double minimalAngleOfClippedCircleArc,
                                   double pointBoundingBoxScale,
                                   string pointLabelMacro,
                                   IReadOnlyDictionary<ObjectDrawingStyle, string> pointMarkMacros,
                                   IReadOnlyDictionary<ObjectDrawingStyle, string> lineSegmentMacros,
                                   IReadOnlyDictionary<ObjectDrawingStyle, string> circleMacros,
                                   string textMacro)
        {
            ScaleVariable = scaleVariable ?? throw new ArgumentNullException(nameof(scaleVariable));
            ShiftLength = shiftLength;
            MinimalWidthForClipping = minimalWidthForClipping;
            MinimalHeightForClipping = minimalHeightForClipping;
            MinimalAngleOfClippedCircleArc = minimalAngleOfClippedCircleArc;
            PointBoundingBoxScale = pointBoundingBoxScale;
            PointLabelMacro = pointLabelMacro ?? throw new ArgumentNullException(nameof(pointLabelMacro));
            PointMarkMacros = pointMarkMacros ?? throw new ArgumentNullException(nameof(pointMarkMacros));
            LineSegmentMacros = lineSegmentMacros ?? throw new ArgumentNullException(nameof(lineSegmentMacros));
            CircleMacros = circleMacros ?? throw new ArgumentNullException(nameof(circleMacros));
            TextMacro = textMacro ?? throw new ArgumentNullException(nameof(textMacro));

            // Ensure the shift length is positive
            if (shiftLength <= 0)
                throw new ArgumentOutOfRangeException(nameof(shiftLength), "The shift length must be positive.");

            // Ensure the minimal clipping width is non-negative
            if (minimalWidthForClipping < 0)
                throw new ArgumentOutOfRangeException(nameof(minimalWidthForClipping), "The minimal clipping width must be at least 0.");

            // Ensure the minimal clipping height is non-negative
            if (minimalHeightForClipping < 0)
                throw new ArgumentOutOfRangeException(nameof(minimalHeightForClipping), "The minimal clipping height must be at least 0.");

            // Ensure the bounding box cut threshold is in [0, 360)
            if (minimalAngleOfClippedCircleArc < 0 || minimalAngleOfClippedCircleArc >= 360)
                throw new ArgumentOutOfRangeException(nameof(minimalAngleOfClippedCircleArc), "The minimal angle of a clipped circle arc must be in [0, 360).");

            // Ensure the point bounding box scale is at least 1
            if (pointBoundingBoxScale < 1)
                throw new ArgumentOutOfRangeException(nameof(pointBoundingBoxScale), "The point bounding box scale must be at least 1.");
        }

        #endregion
    }
}
