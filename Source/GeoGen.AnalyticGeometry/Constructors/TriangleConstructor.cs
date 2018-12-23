using System;
using System.Collections.Generic;

namespace GeoGen.AnalyticGeometry
{
    /// <summary>
    /// A default of implementation of <see cref="ITriangleConstructor"/>.
    /// </summary>
    public class TriangleConstructor : ITriangleConstructor
    {
        #region Private fields

        /// <summary>
        /// The generator of random doubles.
        /// </summary>
        private readonly IRandomnessProvider _random;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="random">The generator of random numbers.</param>
        public TriangleConstructor(IRandomnessProvider random)
        {
            _random = random ?? throw new ArgumentNullException(nameof(random));
        }

        #endregion

        #region ITriangleConstructor implementation

        /// <summary>
        /// Constructs a random scalene acute-angled triangle.
        /// </summary>
        /// <returns>The list of three points that make the triangle.</returns>
        public List<AnalyticObject> NextScaleneAcuteAngedTriangle()
        {
            // First we normally place two points
            var a = new Point(0, 0);
            var b = new Point(1, 0);

            // In order to generate a third point C, we need to establish 
            // the rules. We want each two angles <A, <B, <C to have the
            // absolute difference at least d, where d is a constant.
            // Now we WLOG assume that <A is the largest angle and <C the least angle.
            // We also want to have <C to be at least 'd' and 90 - <A to be at least 'd'. 
            // In that way we get a triangle that is acute-angled, isn't too flat, 
            // isn't too close to a right-angled triangle and isn't close to a isosceles triangle.
            // It can be shown that if we generate <A from the interval (60+d, 90-d) and then
            // <B from the interval ((180+d-A)/2, A-d), that we will get a wanted result (simple math).
            // In order be able to generate A, we need to have d from the interval (0,15). I'm not
            // sure about the best value, so I put the middle one :)
            const double d = 7.5;

            // Let us generate angles according to our formulas
            var alpha = _random.NextDouble(60 + d, 90 - d);
            var beta = _random.NextDouble((180 + d - alpha) / 2, alpha - d);

            // Now we need to construct a triangle with these angles (and our starting points A, B)
            // A simple way to achieve this is to use tangents. Let C = (x, y). Then the vector
            // C - A = (x, y) has the slope <A and the vector C - B = (x-1, y) has the slope 180 - <B.
            // From this we have two equations:
            //
            // y/x = tan(<A)
            // y/(x-1) = tan(180-<B)
            //
            // One can easy derive that 
            //
            // x = tan(<A) / (tan(<A) - tan(180-<B))
            // y = tan(<A) tan(180-<B) / (tan(<A) - tan(180-<B))
            //
            // It's also easy to see that the denominator can't be 0 :) 
            // (in short, we would have either <A + <B = 180, or 180 - <B = 90 - <A, both are impossible)
            // 
            // Therefore we may happily generate the point C

            // First calculate tangents
            var tanAlpha = Math.Tan(MathematicalHelpers.ToRadians(alpha));
            var tan180MinusBeta = Math.Tan(MathematicalHelpers.ToRadians(180 - beta));

            // Then calculate coordinates
            var x = tan180MinusBeta / (tan180MinusBeta - tanAlpha);
            var y = tanAlpha * tan180MinusBeta / (tan180MinusBeta - tanAlpha);

            // Construct the point
            var c = new Point(x, y);

            // And return all of them
            return new List<AnalyticObject> {a, b, c};
        }

        #endregion
    }
}