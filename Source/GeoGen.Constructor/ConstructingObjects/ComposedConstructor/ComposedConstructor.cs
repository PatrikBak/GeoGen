using GeoGen.AnalyticGeometry;
using GeoGen.Core;
using System;
using System.Linq;

namespace GeoGen.Constructor
{
    /// <summary>
    /// The default implementation of <see cref="IComposedConstructor"/> that 
    /// receives a <see cref="ComposedConstruction"/> as a constructor parameter. 
    /// </summary>
    public class ComposedConstructor : ObjectsConstructorBase, IComposedConstructor
    {
        #region Dependencies

        /// <summary>
        /// The resolver of constructors used while constructing the internal configuration of the composed construction.
        /// </summary>
        private readonly IConstructorsResolver _constructionResolver;

        #endregion

        #region Private fields

        /// <summary>
        /// The composed construction performed by the constructor.
        /// </summary>
        private readonly ComposedConstruction _construction;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ComposedConstructor"/> class.
        /// </summary>
        /// <param name="construction">The composed construction performed by the constructor.</param>
        /// <param name="constructionResolver">The resolver of constructors used while constructing the internal configuration of the composed construction.</param>
        /// <param name="tracer">The tracer for unexpected analytic exceptions.</param>
        public ComposedConstructor(ComposedConstruction construction, IConstructorsResolver constructionResolver, IConstructorFailureTracer tracer = null)
            : base(tracer)
        {
            _construction = construction ?? throw new ArgumentNullException(nameof(construction));
            _constructionResolver = constructionResolver ?? throw new ArgumentNullException(nameof(constructionResolver));
        }

        #endregion

        #region ObjectsConstructorBase implementation

        /// <summary>
        /// Performs the actual construction of an analytic object based on the analytic objects given as an input.
        /// The order of the objects of the input is based on the <see cref="Arguments.FlattenedList"/>.
        /// </summary>
        /// <param name="input">The analytic objects to be used as an input.</param>
        /// <returns>The constructed analytic object, if the construction was successful; or null otherwise.</returns>
        protected override IAnalyticObject Construct(IAnalyticObject[] input)
        {
            #region Verify layout

            // First we need to verify that the objects match the layout
            switch (_construction.Configuration.LooseObjectsHolder.Layout)
            {
                // Two points should be always fine
                case LooseObjectsLayout.LineSegment:
                    break;

                // Make sure the points are not collinear
                case LooseObjectsLayout.Triangle:

                    // If yes, the construction shouldn't be possible
                    if (AnalyticHelpers.AreCollinear((Point)input[0], (Point)input[1], (Point)input[2]))
                        return null;

                    break;

                // Make sure no three points are collinear
                case LooseObjectsLayout.Quadrilateral:

                    // Get the points
                    var point1 = (Point)input[0];
                    var point2 = (Point)input[1];
                    var point3 = (Point)input[2];
                    var point4 = (Point)input[3];

                    // Verify any three of them
                    if (AnalyticHelpers.AreCollinear(point1, point2, point3) ||
                        AnalyticHelpers.AreCollinear(point1, point2, point4) ||
                        AnalyticHelpers.AreCollinear(point1, point3, point4) ||
                        AnalyticHelpers.AreCollinear(point2, point3, point4))
                        // If some three are collinear, the construction shouldn't be possible
                        return null;

                    break;

                // Make sure the point doesn't line on the line are not collinear
                case LooseObjectsLayout.ExplicitLineAndPoint:

                    // If yes, the construction shouldn't be possible
                    if (((Line)input[0]).Contains((Point)input[1]))
                        return null;

                    break;

                // Make sure neither of the points lies on the line
                case LooseObjectsLayout.ExplicitLineAndTwoPoints:

                    // Get the line 
                    var line = (Line)input[0];

                    // If at least one point lies on the line, the construction 
                    // shouldn't be possible
                    if (line.Contains((Point)input[1]) || line.Contains((Point)input[2]))
                        return null;

                    break;

                // Default case
                default:
                    throw new ConstructorException($"Unsupported layout: {_construction.Configuration.LooseObjectsHolder.Layout}");
            }

            #endregion

            // Initialize an internal picture in which we're going to construct
            // the configuration that defines our composed construction
            var internalPicture = new Picture();

            // Add the loose objects to the picture
            internalPicture.AddObjects(_construction.Configuration.LooseObjects, () => input.ToList());

            // Add the constructed objects as well
            foreach (var constructedObject in _construction.Configuration.ConstructedObjects)
            {
                // For each one find the construction function
                var constructorFunction = _constructionResolver.Resolve(constructedObject.Construction).Construct(constructedObject);

                // Add the object to the picture using this function that gets passed the internal picture
                internalPicture.TryAdd(constructedObject, () => constructorFunction(internalPicture), out var objectConstructed, out var equalObject);

                // Find out if we have a correct result, i.e. object was constructed without any duplicate
                var correctResult = objectConstructed && equalObject == null;

                // If not, the construction failed
                if (!correctResult)
                    return null;
            }

            // If we are here, then the construction should be fine and the result
            // will be in the internal picture corresponding to the last object 
            // of the configuration that defines our composed construction
            return internalPicture.Get(_construction.Configuration.LastConstructedObject);
        }

        #endregion
    }
}