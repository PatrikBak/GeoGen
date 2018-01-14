using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.AnalyticalGeometry;
using GeoGen.Core;
using GeoGen.Utilities.Helpers;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// A constructor for the <see cref="IntersectionFromPoints"/> construction.
    /// </summary>
    internal sealed class IntersectionOfLinesConstructor : PredefinedConstructorBase
    {
        /// <summary>
        /// Constructs a given list of constructed configurations objects. This objects 
        /// should be the result of the same construction.
        /// </summary>
        /// <param name="constructedObjects">The constructed objects list.</param>
        /// <returns>The constructor output.</returns>
        public override ConstructorOutput Construct(List<ConstructedConfigurationObject> constructedObjects)
        {
            if (constructedObjects == null)
                throw new ArgumentNullException(nameof(constructedObjects));

            try
            {
                ThrowHelper.ThrowExceptionIfNotTrue(constructedObjects.Count == 1);

                var constructedObject = constructedObjects[0];
                var arguments = constructedObject.PassedArguments;

                ThrowHelper.ThrowExceptionIfNotTrue(arguments.Count == 1);

                var setArguments = ((SetConstructionArgument) arguments[0]).PassedArguments.ToList();

                ThrowHelper.ThrowExceptionIfNotTrue(setArguments.Count == 2);

                var passedLine1 = ((ObjectConstructionArgument) setArguments[0]).PassedObject;
                var passedLine2 = ((ObjectConstructionArgument) setArguments[1]).PassedObject;
                
                List<AnalyticalObject> ConstructorFunction(IObjectsContainer container)
                {
                    if (container == null)
                        throw new ArgumentNullException(nameof(container));

                    var line1 = container.Get<Line>(passedLine1);
                    var line2 = container.Get<Line>(passedLine2);

                    try
                    {
                        var intersection = line1.IntersectionWith(line2);

                        return intersection == null ? null : new List<AnalyticalObject> {intersection};
                    }
                    catch (ArgumentException)
                    {
                        return null;
                    }
                }
                
                return new ConstructorOutput
                {
                    ConstructorFunction = ConstructorFunction,
                    Theorems = new List<Theorem>()
                };
            }
            catch (Exception)
            {
                throw new AnalyzerException("Incorrect arguments.");
            }
        }
    }
}