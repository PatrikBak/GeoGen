using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.AnalyticalGeometry;
using GeoGen.AnalyticalGeometry.AnalyticalObjects;
using GeoGen.AnalyticalGeometry.RandomObjects;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Constructions.PredefinedConstructions;
using GeoGen.Core.Theorems;
using GeoGen.Utilities.Helpers;

namespace GeoGen.Analyzer
{
    internal class LoosePointOnLineFromPointsConstructor : IPredefinedConstructor
    {
        private readonly IRandomnessProvider _provider;

        public Type PredefinedConstructionType { get; } = typeof(LoosePointOnLineFromPoints);

        public LoosePointOnLineFromPointsConstructor(IRandomnessProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

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

                var setArgument = (SetConstructionArgument) arguments[0];
                var passedPoints = setArgument.PassedArguments.ToList();

                ThrowHelper.ThrowExceptionIfNotTrue(passedPoints.Count == 2);

                var obj1 = ((ObjectConstructionArgument) passedPoints[0]).PassedObject;
                var obj2 = ((ObjectConstructionArgument) passedPoints[1]).PassedObject;

                ThrowHelper.ThrowExceptionIfNotTrue(obj1.ObjectType == ConfigurationObjectType.Point);
                ThrowHelper.ThrowExceptionIfNotTrue(obj2.ObjectType == ConfigurationObjectType.Point);

                List<IAnalyticalObject> ConstructorFunction(IObjectsContainer container)
                {
                    if (container == null)
                        throw new ArgumentNullException(nameof(container));

                    var point1 = container.Get<Point>(obj1);
                    var point2 = container.Get<Point>(obj2);

                    var points = new HashSet<Point> {point1, point2};

                    var line = new Line(point1, point2);

                    while (true)
                    {
                        var randomPoint = line.RandomPointOnLine(_provider);

                        if (!points.Contains(randomPoint))
                        {
                            return new List<IAnalyticalObject> {randomPoint};
                        }
                    }
                }

                var objects = new HashSet<TheoremObject>
                {
                    new TheoremObject(obj1),
                    new TheoremObject(obj2),
                    new TheoremObject(constructedObject)
                };
                var collinearityTheorem = new Theorem(TheoremType.CollinearPoints, objects);

                return new ConstructorOutput
                {
                    ConstructorFunction = ConstructorFunction,
                    Theorems = new List<Theorem> {collinearityTheorem}
                };
            }
            catch (Exception)
            {
                throw new AnalyzerException("Incorrect arguments.");
            }
        }
    }
}