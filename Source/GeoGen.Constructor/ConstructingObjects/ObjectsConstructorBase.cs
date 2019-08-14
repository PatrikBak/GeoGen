using GeoGen.AnalyticGeometry;
using GeoGen.Core;
using System;
using System.Linq;

namespace GeoGen.Constructor
{
    /// <summary>
    /// A base class for <see cref="IObjectsConstructor"/>s that simplify implementors 
    /// so they can deal directly with analytic objects.
    /// </summary>
    public abstract class ObjectsConstructorBase : IObjectsConstructor
    {
        #region Private fields

        /// <summary>
        /// The tracer for unexpected analytic exceptions.
        /// </summary>
        private readonly IConstructorFailureTracer _tracer;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectsConstructorBase"/> class.
        /// </summary>
        /// <param name="tracer">The tracer for unexpected analytic exceptions.</param>
        protected ObjectsConstructorBase(IConstructorFailureTracer tracer)
        {
            _tracer = tracer;
        }

        #endregion

        #region IObjectsConstructor implementation

        /// <summary>
        /// Creates a function that can perform the actual geometric construction of a given
        /// constructed configuration object, finding the needed objects in a given picture.
        /// </summary>
        /// <param name="constructedObjects">The object to be constructed.</param>
        /// <returns>The function that can perform the actual construction using the context from a given picture.</returns>
        public Func<Picture, IAnalyticObject> Construct(ConstructedConfigurationObject configurationObject)
        {
            // Return the function that takes a picture as an input and returns the result of a construction
            return picture =>
            {
                // Get the analytic versions of the objects passed in the arguments       
                var analyticObjects = configurationObject.PassedArguments.FlattenedList.Select(picture.Get).ToArray();

                try
                {
                    // Try to call the abstract function to get the actual result
                    return Construct(analyticObjects);
                }
                // Just in case, if there is an analytic exception
                catch (AnalyticException e)
                {
                    // We trace it
                    _tracer?.TraceUnexpectedConstructionFailure(configurationObject, analyticObjects, e.Message);

                    // And return null indicating the constructor didn't work out
                    return null;
                }
            };
        }

        #endregion

        #region Protected abstract methods

        /// <summary>
        /// Performs the actual construction of an analytic object based on the analytic objects given as an input.
        /// The order of the objects of the input is based on the <see cref="Arguments.FlattenedList"/>.
        /// </summary>
        /// <param name="input">The analytic objects to be used as an input.</param>
        /// <returns>The constructed analytic object, if the construction was successful; or null otherwise.</returns>
        protected abstract IAnalyticObject Construct(IAnalyticObject[] input);

        #endregion
    }
}