using System;
using System.Collections.Generic;

namespace GeoGen.AnalyticalGeometry
{
    /// <summary>
    /// A default implementation of <see cref="IRandomObjectsProvider"/>.
    /// </summary>
    internal class RandomObjectsProvider : IRandomObjectsProvider
    {
        #region Public constants

        /// <summary>
        /// The maximal value of generated doubles.
        /// </summary>
        public const double MaximalRandomValue = 100.0;

        #endregion

        #region Private fields

        /// <summary>
        /// The dictionary mapping types of objects to the sets of currently
        /// generated objects of that type.
        /// </summary>
        private readonly Dictionary<Type, HashSet<AnalyticalObject>> _objects;

        /// <summary>
        /// The randomness provider.
        /// </summary>
        private readonly IRandomnessProvider _random;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="random">The randomness provider.</param>
        public RandomObjectsProvider(IRandomnessProvider random)
        {
            _random = random ?? throw new ArgumentNullException(nameof(random));
            _objects = new Dictionary<Type, HashSet<AnalyticalObject>>();
        }

        #endregion

        #region IRandomObjectsProvider implementation

        /// <summary>
        /// Generates the next random object mutually distinct from 
        /// all previously generated ones.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <returns>The next random object.</returns>
        public AnalyticalObject NextRandomObject<T>() where T : AnalyticalObject
        {
            // Pull the type
            var type = typeof(T);

            // Find the corresponding set to the type.
            var set = FindSetForType(type);

            // Initialize result
            AnalyticalObject result;

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

            // Return the result.
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
        private HashSet<AnalyticalObject> FindSetForType(Type type)
        {
            // Check if the type isn't already present
            if (_objects.ContainsKey(type))
            {
                // If yes, return the set corresponding to it
                return _objects[type];
            }

            // Otherwise create a new set
            var set = new HashSet<AnalyticalObject>();

            // Register it to the dictionary
            _objects.Add(type, set);

            // And return it
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
            // We generate in loop until we're successful
            while (true)
            {
                // Generate the center
                var center = RandomPoint();

                // Generate the radius (it might be zero)
                var radius = _random.NextDouble(MaximalRandomValue);

                try
                {
                    // Try to construct a circle
                    return new Circle(center, radius);
                }
                catch (AnalyticalException)
                {
                    // If it wasn't successful, then the radius is zero
                    // and we need to try again
                }
            }
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

                try
                {
                    // Try to construct a line
                    return new Line(point1, point2);
                }
                catch (AnalyticalException)
                {
                    // If it wasn't successful, then the points are the same
                    // and we need to try again
                }
            }
        }

        #endregion
    }
}