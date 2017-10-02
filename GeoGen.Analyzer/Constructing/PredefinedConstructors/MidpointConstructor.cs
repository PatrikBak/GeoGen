using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Analyzer.AnalyticalGeometry;
using GeoGen.Analyzer.Objects;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Constructions.PredefinedConstructions;

namespace GeoGen.Analyzer.Constructing.PredefinedConstructors
{
    internal class MidpointConstructor : IPredefinedConstructor
    {
        public Type PredefinedConstructionType { get; } = typeof(Midpoint);

        public List<GeometricalObject> Apply(IReadOnlyList<ConstructionArgument> arguments, IObjectsContainer container)
        {
            if (arguments == null)
                throw new ArgumentNullException(nameof(arguments));

            try
            {
                var setArgument = (SetConstructionArgument) arguments[0];
                var passedPoints = setArgument.PassedArguments.ToList();

                var obj1 = ((ObjectConstructionArgument) passedPoints[0]).PassedObject;
                var obj2 = ((ObjectConstructionArgument) passedPoints[1]).PassedObject;

                var point1 = (Point) container[obj1.Id ?? throw new Exception()];
                var point2 = (Point) container[obj2.Id ?? throw new Exception()];

                var result = AnalyticalHelpers.Midpoint(point1, point2);

                return new List<GeometricalObject> {result};
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}