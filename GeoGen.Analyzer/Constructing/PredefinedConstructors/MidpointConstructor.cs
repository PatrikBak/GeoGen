using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Analyzer.AnalyticalGeometry;
using GeoGen.Analyzer.Objects;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Constructions.PredefinedConstructions;
using GeoGen.Core.Theorems;

namespace GeoGen.Analyzer.Constructing.PredefinedConstructors
{
    internal class MidpointConstructor : IPredefinedConstructor
    {
        public Type PredefinedConstructionType { get; } = typeof(Midpoint);

        public ConstructorOutput Apply(IReadOnlyList<ConstructionArgument> arguments, IObjectsContainer container)
        {
            if (arguments == null)
                throw new ArgumentNullException(nameof(arguments));

            try
            {
                var setArgument = (SetConstructionArgument) arguments[0];
                var passedPoints = setArgument.PassedArguments.ToList();

                var obj1 = ((ObjectConstructionArgument) passedPoints[0]).PassedObject;
                var obj2 = ((ObjectConstructionArgument) passedPoints[1]).PassedObject;

                var point1 = container.Get<Point>(obj1);
                var point2 = container.Get<Point>(obj2);

                var result = AnalyticalHelpers.Midpoint(point1, point2);

                if (result == null)
                    return null;

                var objects = new List<TheoremObject>
                {
                    new SingleTheoremObject(point1.ConfigurationObject),
                    new SingleTheoremObject(point2.ConfigurationObject),
                    new SingleTheoremObject(result.ConfigurationObject)
                };

                var collinearityTheorem = new Theorem(TheoremType.CollinearPoints, objects);

                return new ConstructorOutput
                {
                    Objects = new List<GeometricalObject> {result},
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