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
                // Just in case, if there is an analytic exception, which it normally shouldn't, we will 
                // return null indicating the construct has failed. These exception are possible, because
                // it would be very hard to predict every possible outcome of the fact that the numerical
                // system is not precise. Other exceptions would be a bigger deal and we won't hide them
                catch (AnalyticException)
                {
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