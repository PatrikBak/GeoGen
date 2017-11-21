using System;
using System.Collections.Generic;
using GeoGen.AnalyticalGeometry.AnalyticalObjects;

namespace GeoGen.AnalyticalGeometry.RandomObjects
{
    /// <summary>
    /// A default implementation of <see cref="IRandomObjectsProvider"/>.
    /// </summary>
    internal sealed class RandomObjectsProvider : IRandomObjectsProvider
    {
        #region Public constants

        /// <summary>
        /// The maximal value of generated doubles.
        /// </summary>
        public const double MaximalRandomValue = 10.0;

        #endregion

        #region Private fields

        /// <summary>
        /// The dictionary mapping types of objects to the sets of currently
        /// generated objects of that type.
        /// </summary>
        private readonly Dictionary<Type, HashSet<IAnalyticalObject>> _objects;

        /// <summary>
        /// The randomness provider.
        /// </summary>
        private readonly IRandomnessProvider _random;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new random objects provided that uses a given
        /// randomness provider.
        /// </summary>
        /// <param name="random">The randomness provider.</param>
        public RandomObjectsProvider(IRandomnessProvider random)
        {
            _random = random ?? throw new ArgumentNullException(nameof(random));
            _objects = new Dictionary<Type, HashSet<IAnalyticalObject>>();
        }

        #endregion

        #region IRandomObjectsProvider methods

        /// <summary>
        /// Generates the next random object.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <returns>The next random object.</returns>
        public IAnalyticalObject NextRandomObject<T>() where T : IAnalyticalObject
        {
            // Pull the type
            var type = typeof(T);

            // Find the corresponding set to the type.
            var set = FindSetForType(type);

            // Initialize result
            IAnalyticalObject result;

            // Generate
            while (true)
            {
                if (type == typeof(Point))
                    result = RandomPoint();
                else if (type == typeof(Line))
                    result = RandomLine();
                else if (type == typeof(Circle))
                    result = RandomCircle();
                else
                    throw new Exception("Unhandled case");

                // If adding the result to the set caused the change of the set,
                // then we've found the right object and we can break.
                if (set.Add(result))
                    break;

                // Otherwise we continue with the random generation. 
            }

            // And return the result.
            return result;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Finds the set of objects for given type. If the type 
        /// hasn't been registered yet, this method will do that.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The set.</returns>
        private HashSet<IAnalyticalObject> FindSetForType(Type type)
        {
            if (_objects.ContainsKey(type))
                return _objects[type];

            var set = new HashSet<IAnalyticalObject>();
            _objects.Add(type, set);

            return set;
        }

        /// <summary>
        /// Creates a random point..
        /// </summary>
        /// <returns>The random point.</returns>
        private Point RandomPoint()
        {
            var x = _random.NextDouble(MaximalRandomValue);
            var y = _random.NextDouble(MaximalRandomValue);

            return new Point(x, y);
        }

        /// <summary>
        /// Creates a random circle.
        /// </summary>
        /// <returns>The random circle.</returns>
        private Circle RandomCircle()
        {
            var center = RandomPoint();

            // The radius will be in the interval (0, MaximalRandomValue]
            var radius = MaximalRandomValue - _random.NextDouble(MaximalRandomValue);

            return new Circle(center, radius);
        }

        /// <summary>
        /// Creates a random line.
        /// </summary>
        /// <returns>The random line.</returns>
        private Line RandomLine()
        {
            // We generate in loop until we're successful
            while (true)
            {
                // Generate two points
                var point1 = RandomPoint();
                var point2 = RandomPoint();

                // If there are not the same
                if (point1 != point2)
                {
                    // We'll free to return and construct the line
                    return new Line(point1, point2);
                }
            }
        }

        #endregion
    }
}