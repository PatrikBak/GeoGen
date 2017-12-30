using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.AnalyticalGeometry;
using GeoGen.AnalyticalGeometry.AnalyticalObjects;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Constructions.PredefinedConstructions;
using GeoGen.Core.Theorems;
using GeoGen.Utilities.Helpers;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// A constructor for the <see cref="IntersectionFromPoints"/> construction.
    /// </summary>
    internal sealed class InteresectionFromLinesConstructor : IPredefinedConstructor
    {
        /// <summary>
        /// Gets the type of this predefined construction.
        /// </summary>
        public Type PredefinedConstructionType { get; } = typeof(IntersectionFromLines);

        /// <summary>
        /// Represents a general constructor of <see cref="ConstructedConfigurationObject"/>s.
        /// </summary>
        public ConstructorOutput Construct(List<ConstructedConfigurationObject> constructedObjects)
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
                
                List<IAnalyticalObject> ConstructorFunction(IObjectsContainer container)
                {
                    if (container == null)
                        throw new ArgumentNullException(nameof(container));

                    var line1 = container.Get<Line>(passedLine1);
                    var line2 = container.Get<Line>(passedLine2);

                    try
                    {
                        return new List<IAnalyticalObject> {line1.IntersectionWith(line2)};
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