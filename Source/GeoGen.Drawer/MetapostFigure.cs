using GeoGen.AnalyticGeometry;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace GeoGen.Drawer
{
    /// <summary>
    /// Represents a MetaPost figure accepts things to be drawn via Add methods and then is able
    /// to generate the actual MetaPost code that would perform the drawing. 
    /// </summary>
    public class MetapostFigure
    {
        #region Private fields

        /// <summary>
        /// The dictionary mapping objects to the labels that have been assigned.
        /// </summary>
        private readonly Dictionary<IAnalyticObject, string> _labels = new Dictionary<IAnalyticObject, string>();

        /// <summary>
        /// The dictionary mapping points to be marked to the styles that are supposed to be used.
        /// </summary>
        private readonly Dictionary<Point, ObjectDrawingStyle> _pointStyles = new Dictionary<Point, ObjectDrawingStyle>();

        /// <summary>
        /// The dictionary mapping circles to be drawn to the styles that are supposed to be used.
        /// </summary>
        private readonly Dictionary<Circle, ObjectDrawingStyle> _circleStyles = new Dictionary<Circle, ObjectDrawingStyle>();

        /// <summary>
        /// The dictionary mapping lines to be drawn to the styles that are supposed to be used.
        /// </summary>
        private readonly Dictionary<Line, ObjectDrawingStyle> _lineStyles = new Dictionary<Line, ObjectDrawingStyle>();

        /// <summary>
        /// The dictionary mapping lines to the points that are supposed to be visually lie on them.
        /// </summary>
        private readonly Dictionary<Line, List<Point>> _linePoints = new Dictionary<Line, List<Point>>();

        /// <summary>
        /// The dictionary mapping lines to the points that are supposed to be visually lie on them and be shifted at them a bit.
        /// </summary>
        private readonly Dictionary<Line, List<Point>> _shiftLinePoints = new Dictionary<Line, List<Point>>();

        /// <summary>
        /// The dictionary lines to segments with their styles that are part of them.
        /// </summary>
        private readonly Dictionary<Line, List<(Point, Point, ObjectDrawingStyle)>> _lineSegments = new Dictionary<Line, List<(Point, Point, ObjectDrawingStyle)>>();

        /// <summary>
        /// The dictionary lines to shifted segments with their styles that are part of them.
        /// </summary>
        private readonly Dictionary<Line, List<(Point, Point, ObjectDrawingStyle)>> _lineShiftedSegments = new Dictionary<Line, List<(Point, Point, ObjectDrawingStyle)>>();

        /// <summary>
        /// The text to be added to the right of the picture.
        /// </summary>
        private string _text;

        #endregion

        #region Public methods

        /// <summary>
        /// Adds a given point with a given style to the figure.
        /// </summary>
        /// <param name="point">The point to be drawn.</param>
        /// <param name="style">The style to be used while drawing.</param>
        public void AddPoint(Point point, ObjectDrawingStyle style)
        {
            // If it's not there
            if (!_pointStyles.ContainsKey(point))
            {
                // Then simply add it
                _pointStyles.Add(point, style);

                // We're done
                return;
            }

            // Otherwise it's there. If we got a higher style, then override it
            if (style > _pointStyles[point])
            {
                // Remove it
                _pointStyles.Remove(point);

                // Add it with this higher style
                _pointStyles.Add(point, style);
            }
        }

        /// <summary>
        /// Adds a given line to be drawn with a given style. It can be specified which points are supposed
        /// to visually on the line when it's drawn. The line can be also drawn to be shifted at them.
        /// </summary>
        /// <param name="line">The line to be drawn.</param>
        /// <param name="points">The points that are supposed to visually lie on the line.</param>
        /// <param name="style">The style to be used while drawing.</param>
        /// <param name="shifted">Indicates whether the line should be shifted around the <paramref name="points"/>.</param>
        public void AddLine(Line line, Point[] points, ObjectDrawingStyle style, bool shifted)
        {
            // Make sure the points lie on the line
            if (points.Any(point => !AnalyticHelpers.LiesOn(line, point)))
                throw new DrawerException("There is a line that should have contained the passed point, but it did not.");

            // Handle the points
            points.ForEach(point =>
                // Get the right dictionary according to the fact whether the line should be shifted around points
                (shifted ? _shiftLinePoints : _linePoints)
                // Make sure it contains the line and the corresponding list the point
                .GetOrAdd(line, () => new List<Point>()).Add(point));

            // Make sure the line's style is added to the lines dictionary
            // If it's not there...
            if (!_lineStyles.ContainsKey(line))
            {
                // Then simply add it
                _lineStyles.Add(line, style);

                // We're done
                return;
            }

            // Otherwise it's there. If we got a higher style, then override it
            if (style > _lineStyles[line])
            {
                // Remove it
                _lineStyles.Remove(line);

                // Add it with this higher style
                _lineStyles.Add(line, style);
            }
        }

        /// <summary>
        /// Adds a given line to be drawn with a given style. 
        /// </summary>
        /// <param name="circle">The circle to be drawn.</param>
        /// <param name="style">The style to be used while drawing.</param>
        public void AddCircle(Circle circle, ObjectDrawingStyle style)
        {
            // If it's not there
            if (!_circleStyles.ContainsKey(circle))
            {
                // Then simply add it
                _circleStyles.Add(circle, style);

                // We're done
                return;
            }

            // Otherwise it's there. If we got a higher style, then override it
            if (style > _circleStyles[circle])
            {
                // Remove it
                _circleStyles.Remove(circle);

                // Add it with this higher style
                _circleStyles.Add(circle, style);
            }
        }

        /// <summary>
        /// Adds a given segment to be drawn with a given style. It can be specified whether the segment 
        /// should be shifted at the second point.
        /// </summary>
        /// <param name="point1">The first point of the segment.</param>
        /// <param name="point2">The second point of the segment.</param>
        /// <param name="style">The style to be used while drawing.</param>
        /// <param name="shifted">Indicates whether the segment should be shifted at the second point.</param>
        public void AddSegment(Point point1, Point point2, ObjectDrawingStyle style, bool shifted)
        {
            // Get the right dictionary where we're going to add the segment based on the shift state
            (shifted ? _lineShiftedSegments : _lineSegments)
                // Make sure the line is in the dictionary and add the segment
                .GetOrAdd(new Line(point1, point2), () => new List<(Point, Point, ObjectDrawingStyle)>()).Add((point1, point2, style));
        }

        /// <summary>
        /// Adds a label of a given object to the figure.
        /// </summary>
        /// <param name="analyticObject">The object to be labeled.</param>
        /// <param name="label">The label of the object.</param>
        public void AddLabel(IAnalyticObject analyticObject, string label)
        {
            // If the object is already there, make aware
            if (_labels.ContainsKey(analyticObject))
                throw new DrawerException("Cannot label the same object twice.");

            // Otherwise add the label
            _labels.Add(analyticObject, label);
        }

        /// <summary>
        /// Adds the text. The text will be displayed to the right of the picture. It will be rendered with TeX,
        /// i.e. it might (and should) include TeX commands.
        /// </summary>
        /// <param name="text">The text to be displayed.</param>
        public void AddText(string text) => _text = text;

        /// <summary>
        /// Generates a code of the picture using the drawing data holding necessary MetaPost-related information to do so.
        /// </summary>
        /// <param name="drawingData">The data for drawing holding for example names of needed macros.</param>
        /// <returns>The MetaPost code of the figure.</returns>
        public string ToCode(MetapostDrawingData drawingData)
        {
            // Helper function that converts a point to a string readable by MetaPost
            string ConvertPoint(Point point) =>
                // Include the scaling variable and both coordinates
                // CultureInfo.InvariantCulture is there to make sure we will get a decimal point
                $"{drawingData.ScaleVariable}*({point.X.ToString(CultureInfo.InvariantCulture)},{point.Y.ToString(CultureInfo.InvariantCulture)})";

            // Prepare the resulting code
            var code = new StringBuilder();

            #region Lines

            // Clone the line segments dictionary so we can safely add things to this cloned one
            var segments = _lineSegments.ToDictionary(pair => pair.Key, pair => new List<(Point, Point, ObjectDrawingStyle)>(pair.Value));

            #region Extend shifted segments

            // Go through the shifted segments...
            _lineShiftedSegments.ForEach(pair =>
            {
                // Deconstruct
                var (line, shiftedSegments) = pair;

                // Make sure this line is in the new dictionary and get its segments
                var currentSegments = segments.GetOrAdd(line, () => new List<(Point, Point, ObjectDrawingStyle)>());

                // Shift each segment
                shiftedSegments.ForEach(triple =>
                {
                    // Deconstruct
                    var (A, B, style) = triple;

                    // Get the shift value
                    var shift = drawingData.ShiftLength;

                    // Shift
                    var C = AnalyticHelpers.ShiftSegment(A, B, shift);

                    // Our extended segment is AC and inherits the style
                    currentSegments.Add((A, C, style));
                });
            });

            #endregion

            #region Prepare segments for all lines

            // Prepare the lines that we're going to handle by taking ones that have a segment
            // and adding those that are stated to be drawn explicitly. We're going to draw each
            segments.Keys.Union(_lineStyles.Keys).ForEach(line =>
            {
                // We need to be able to sort points on lines, so we create a simple comparer that does it
                // The 'smallest' point will be the topmost leftmost one and the 'largest' will be the bottommost rightmost one.
                var comparer = Comparer<Point>.Create((point1, point2) =>
                {
                    // If there the first point is more on the left than the other, then it's 'smaller'
                    if (point1.X.Rounded() < point2.X.Rounded())
                        return -1;

                    // Or the other way around
                    if (point1.X.Rounded() > point2.X.Rounded())
                        return 1;

                    // Otherwise they lie on the same vertical lie
                    // We say if the first one is more at the top, then it's 'smaller'
                    if (point1.Y.Rounded() > point2.Y.Rounded())
                        return -1;

                    // Or the other way around
                    if (point1.Y.Rounded() < point2.Y.Rounded())
                        return 1;

                    // Or they're the same
                    return 0;
                });

                // We're going to keep our points in a SortedList and use it as an identity dictionary 
                // (i.e. it's mapping points to themselves)
                var segmentPoints = new SortedList<Point, Point>(comparer);

                // We also need to maintain a list of styles of particular segments that we have
                // Each segment has a starting point that is mapped to the style of the segment
                // The null value for a style indicates that the segment is not drawn at all
                var segmentStyles = new Dictionary<Point, ObjectDrawingStyle?>();

                // Prepare an empty list of segments to be drawn
                var segmentsToDraw = new List<(Point, Point, ObjectDrawingStyle)>();

                // If there are segments of this line, add them to be drawn
                if (segments.ContainsKey(line))
                    segmentsToDraw.AddRange(segments[line]);

                #region Creating a segment from passing points

                // First find the points that are supposed to visually lie on this line
                var passingPoints = (_linePoints.GetOrDefault(line) ?? Enumerable.Empty<Point>())
                        // Append those of them that are shifted (the shift will be resolved later)
                        .Concat(_shiftLinePoints.GetOrDefault(line) ?? Enumerable.Empty<Point>())
                        // Sort them
                        .OrderBy(point => point, comparer)
                        // Enumerate
                        .ToArray();

                // If there are at least two of them... 
                if (passingPoints.Length >= 2)
                {
                    // Then the first (leftmost) and last (rightmost) make a segment
                    var left = passingPoints[0];
                    var right = passingPoints[passingPoints.Length - 1];

                    // Some of them might be shifted, we need to check that
                    var isLeftShifted = _shiftLinePoints.GetOrDefault(line)?.Contains(left) ?? false;
                    var isRightShifted = _shiftLinePoints.GetOrDefault(line)?.Contains(right) ?? false;

                    // If the left is shifted, do the shift
                    if (isLeftShifted)
                        left = AnalyticHelpers.ShiftSegment(right, left, drawingData.ShiftLength);

                    // If the right is shifted, do the shift
                    if (isRightShifted)
                        right = AnalyticHelpers.ShiftSegment(left, right, drawingData.ShiftLength);

                    // Add the segment
                    segmentsToDraw.Add((left, right, _lineStyles[line]));
                }
                // Otherwise we want to construct the line across to the pictures, but only if 
                // it has a style, i.e. it is even requested to be drawn (it might happen 
                // that we're drawing just some segments of this line)
                else if (_lineStyles.ContainsKey(line))
                {
                    // In order to make the line to go across the picture, we need the bounding box points
                    // Get the leftmost topmost and rightmost bottommost point
                    var leftUpperCorner = _pointStyles.Keys.OrderBy(point => point, comparer).First();
                    var rightBottomCorner = _pointStyles.Keys.OrderBy(point => point, comparer).Last();

                    // Shift the points according to the bounding box offset
                    leftUpperCorner += new Point(-drawingData.BoundingBoxOffset, drawingData.BoundingBoxOffset);
                    rightBottomCorner += new Point(drawingData.BoundingBoxOffset, -drawingData.BoundingBoxOffset);

                    // Create the other two corners
                    var leftBottomCorner = new Point(leftUpperCorner.X, rightBottomCorner.Y);
                    var rightUpperCorner = new Point(rightBottomCorner.X, leftUpperCorner.Y);

                    // Create the bounding lines
                    var intersections = new[]
                    {
                        // Upper horizontal
                        new Line(leftUpperCorner, rightUpperCorner),

                        // Bottom horizontal
                        new Line(leftBottomCorner, rightBottomCorner),

                        // Left vertical
                        new Line(leftUpperCorner, leftBottomCorner),

                        // Right vertical
                        new Line(rightUpperCorner, rightBottomCorner)
                    }
                    // Intersect them with our line
                    .Select(_line => _line.IntersectionWith(line))
                    // Take existing intersections (there might be parallelity)
                    .Where(point => point != null)
                    // Unwrap
                    .Select(point => point.Value)
                    // Take those that lie within the bounding box
                    .Where(point => comparer.Compare(point, leftUpperCorner) >= 0 && comparer.Compare(point, rightBottomCorner) <= 0)
                    // Distinct ones (it might happen a line intersects two of them in the corner)
                    .Distinct()
                    // Order
                    .OrderBy(point => point, comparer)
                    // Enumerate
                    .ToArray();

                    // There should be two of them
                    if (intersections.Length != 2)
                        throw new AnalyticException("Analytic geometric must be flawed, cannot determine intersections with the bounding box correctly");

                    // If we have two of them, we can create the segment from them
                    segmentsToDraw.Add((intersections[0], intersections[1], _lineStyles[line]));
                }

                #endregion

                #region Preparing the final non-overlapping segments 

                // Go through the prepared segments to draw
                segmentsToDraw.ForEach(triple =>
                {
                    // Deconstruct
                    var (point1, point2, segmentStyle) = triple;

                    // Order them
                    var newLeftPoint = comparer.Compare(point1, point2) < 0 ? point1 : point2;
                    var newRightPoint = comparer.Compare(point1, point2) < 0 ? point2 : point1;

                    // Make sure the points are added to the points of the already segmented line
                    segmentPoints.TryAdd(newLeftPoint, newLeftPoint);
                    segmentPoints.TryAdd(newRightPoint, newRightPoint);

                    #region Reevaluating individual segment styles

                    // We're going to rebuild the styles
                    var newSegmentStyles = new Dictionary<Point, ObjectDrawingStyle?>();

                    // Go through the segments
                    for (var i = 0; i < segmentPoints.Count - 1; i++)
                    {
                        // Get the left and right point
                        var leftPoint = segmentPoints.Keys[i];
                        var rightPoint = segmentPoints.Keys[i + 1];

                        // It's going to be useful to know whether this segment is part of the segment being inserted
                        // This happens when the new one has its left point to the right (or equal)
                        var isThisSubsegmentOfNewOne = comparer.Compare(newLeftPoint, leftPoint) <= 0
                            // And its left point to the left (or equal)
                            && comparer.Compare(newRightPoint, rightPoint) <= 0;

                        // If the segment has a style...
                        if (segmentStyles.ContainsKey(leftPoint))
                        {
                            // Get the style 
                            var currentStyle = segmentStyles[leftPoint];

                            // This style might be overridden by the current style, if this 
                            // segment is contained in the one being inserted
                            var shouldWeOverrideStyle = isThisSubsegmentOfNewOne
                                // And is higher than the current one
                                && (currentStyle == null || segmentStyle > currentStyle.Value);

                            // If we should override the style, do it
                            if (shouldWeOverrideStyle)
                                currentStyle = segmentStyle;

                            // Add the style to the new dictionary
                            newSegmentStyles.Add(leftPoint, currentStyle);

                            // And we're done
                            continue;
                        }

                        // If we got here, the segment doesn't have a style yet. These options are possible:
                        // 
                        // 1. It is the new segment being inserted that doesn't overlap any other segment. 
                        // 2. It is an empty segment that got inserted when the new segment has been inserted to the right or left.
                        // 3. It is a segment that was created by splitting an existing segment.
                        //
                        // Let us handle these cases individually
                        // 
                        // The first one happened either if the segment is on the very left, i.e. its points are the first two points 
                        var firstCaseHappened = (segmentPoints.Keys[0] == leftPoint && segmentPoints.Keys[1] == rightPoint)
                            // Or when it is on the very right, i.e. its points are the last two points of the big segment
                            || (segmentPoints.Keys[segmentPoints.Count - 2] == leftPoint && segmentPoints.Keys[segmentPoints.Count - 1] == rightPoint);

                        // If it happened...
                        if (firstCaseHappened)
                        {
                            // Then the segment gets the style it's been assigned to it
                            newSegmentStyles.Add(leftPoint, segmentStyle);

                            // And we're done
                            continue;
                        }

                        // We now know that the first case didn't happen, which also means there are at least 3 points
                        // The second case happened either if the segment is almost on the very left, i.e. its points are the second and third
                        var secondCaseHappened = (segmentPoints.Keys[1] == leftPoint && segmentPoints.Keys[2] == rightPoint)
                            // Or when it is almost on the very right, i.e. its points are the penultimate point and the point before
                            || (segmentPoints.Keys[segmentPoints.Count - 3] == leftPoint && segmentPoints.Keys[segmentPoints.Count - 2] == rightPoint);

                        // If it happened...
                        if (secondCaseHappened)
                        {
                            // Then the segment isn't drawn and gets 'null' style
                            newSegmentStyles.Add(leftPoint, null);

                            // And we're done
                            continue;
                        }

                        // We're left with the third case. In this case we need to have 
                        // a look at the segment that got split in order for us to get this 
                        // segment in the first place. We know it exists at the left index
                        var originalStyle = segmentStyles[segmentPoints.Keys[i - 1]];

                        // This style might get changed if our split part is contained in it
                        var shouldWeOverrideOriginalStyle = isThisSubsegmentOfNewOne
                            // And is higher than the current one
                            && (originalStyle == null || segmentStyle > originalStyle.Value);

                        // If we should override the style, do it
                        if (shouldWeOverrideOriginalStyle)
                            originalStyle = segmentStyle;

                        // We finally know the style for this segment
                        newSegmentStyles.Add(leftPoint, originalStyle);
                    }

                    // Now we can finally override the current segment styles
                    segmentStyles = newSegmentStyles;

                    #endregion
                });

                #endregion

                #region Write drawing commands

                // Go through the points starting segments
                segmentStyles.ForEach(pair =>
                {
                    // Deconstruct
                    var (leftPoint, style) = pair;

                    // If the style is null, i.e. this is not a segment to be drawn, we're done
                    if (style == null)
                        return;

                    // Otherwise get the right point for the segment, i.e. the one after left
                    var rightPoint = segmentPoints.Keys[segmentPoints.IndexOfKey(leftPoint) + 1];

                    // Get the macro name
                    var macroName = drawingData.LineSegmentMacros.GetOrDefault(style.Value)
                        // Make sure it's known when it's not present
                        ?? throw new DrawerException($"The style {style} doesn't have its macro defined for drawing line segments.");

                    // Append the drawing command
                    code.AppendLine($"draw {macroName}({ConvertPoint(leftPoint)}, {ConvertPoint(rightPoint)});");
                });

                #endregion
            });

            #endregion

            #endregion

            #region Circles

            // Go through the circles
            _circleStyles.ForEach(pair =>
            {
                // Deconstruct
                var (circle, style) = pair;

                // Get the macro name
                var macroName = drawingData.CircleMacros.GetOrDefault(style)
                    // Make sure it's known when it's not present
                    ?? throw new DrawerException($"The style {style} doesn't have its macro defined for drawing circles.");

                // Use the macro to draw the circle
                code.AppendLine($"draw {macroName}({ConvertPoint(circle.Center)}, {drawingData.ScaleVariable}*{circle.Radius.ToString(CultureInfo.InvariantCulture)});");
            });

            #endregion

            #region Point marks

            // Go through the points
            _pointStyles.ForEach(pair =>
            {
                // Deconstruct
                var (point, style) = pair;

                // Get the macro name
                var macroName = drawingData.PointMarkMacros.GetOrDefault(style)
                    // Make sure it's known when it's not present
                    ?? throw new DrawerException($"The style {style} doesn't have its macro defined for marking points.");

                // Use the macro to mark the point
                code.AppendLine($"draw {macroName}({ConvertPoint(point)});");
            });

            #endregion

            #region Labels

            // Go through the labels
            _labels.ForEach(pair =>
            {
                // Deconstruct
                var (analyticObject, label) = pair;

                // Switch based on the object type
                switch (analyticObject)
                {
                    // Point case
                    case Point point:

                        // Use the macro to do the drawing of the label
                        code.AppendLine($"draw {drawingData.PointLabelMacro}(btex {label} etex, {ConvertPoint(point)});");

                        break;

                    // TODO: Figure out line and circle cases
                    case Line _:
                    case Circle _:
                        break;

                    // If something else...
                    default:
                        throw new DrawerException($"Unhandled type of {nameof(IAnalyticObject)}: {analyticObject.GetType()}");
                }
            });

            #endregion

            #region Text

            // If there is text to be included, do so via the provided macro
            if (!string.IsNullOrEmpty(_text))
                code.AppendLine($"draw {drawingData.TextMacro}({_text});");

            #endregion

            // Return the resulting code
            return code.ToString();
        }

        /// <summary>
        /// Calculates a heuristic numeric evaluation of the badness of the figure. The higher the ranking, the worse the figure.
        /// </summary>
        /// <returns>The badness ranking.</returns>
        public double CalculateVisualBadness()
        {
            // For each point we calculate the standard deviation of the distances to the other points
            // and pick the highest one. That way, if there are two close points or two fairly distances
            // points, then their deviations will be high.

            // Find the maximal distance between two points
            var maxDistance = _pointStyles.Keys.ToArray().UnorderedPairs().Select(pair => pair.Item1.DistanceTo(pair.Item2)).Max();

            // Rank each point
            return _pointStyles.Keys.Select(point =>
            {
                // For each point
                var distances = _pointStyles.Keys
                    // That is different from this one
                    .Where(_point => _point != point)
                    // We calculate the distance to it normalized to the interval (0,1]
                    .Select(_point => point.DistanceTo(_point) / maxDistance)
                    // Enumerate
                    .ToArray();

                // We want to calculate the standard deviation of these distances, so we need the average
                var averageDistance = distances.Average();

                // And now we can do the math
                return distances.Select(distance => (distance - averageDistance).Squared()).Sum();
            })
            // Find the worst point
            .Max();
        }

        #endregion
    }
}
