using GeoGen.Core;
using GeoGen.Utilities;
using System.Collections.Generic;
using static GeoGen.Core.ComposedConstructions;
using static GeoGen.Core.PredefinedConstructions;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// The default implementation of <see cref=IObjectIntroducer/>.
    /// <para>
    /// TODO: Replace this temporary implementation.
    /// </para>
    /// </summary>
    public class ObjectIntroducer : IObjectIntroducer
    {
        /// <inheritdoc/>
        public IEnumerable<IEnumerable<ConstructedConfigurationObject>> IntroduceObjects(IReadOnlyList<ConstructedConfigurationObject> availableObjects)
        {
            foreach (var availableObject in availableObjects)
            {
                if (availableObject.Construction.Name.Equals(nameof(ReflectionInLine)))
                {
                    var A = availableObject.PassedArguments.FlattenedList[1];

                    yield return new ConstructedConfigurationObject(Midpoint, availableObject, A).ToEnumerable();
                }

                else if (availableObject.Construction.Name.Equals(nameof(ReflectionInLineFromPoints)))
                {
                    var A = availableObject.PassedArguments.FlattenedList[0];

                    yield return new ConstructedConfigurationObject(Midpoint, availableObject, A).ToEnumerable();
                }

                else if (availableObject.Construction.Name.Equals(nameof(PerpendicularBisector)))
                {
                    var A = availableObject.PassedArguments.FlattenedList[0];
                    var B = availableObject.PassedArguments.FlattenedList[1];

                    yield return new ConstructedConfigurationObject(Midpoint, A, B).ToEnumerable();
                }

                else if (availableObject.Construction.Name.Equals(nameof(ParallelogramPoint)))
                {
                    var A = availableObject.PassedArguments.FlattenedList[0];
                    var B = availableObject.PassedArguments.FlattenedList[1];
                    var C = availableObject.PassedArguments.FlattenedList[2];

                    yield return new ConstructedConfigurationObject(Midpoint, B, C).ToEnumerable();
                    yield return new ConstructedConfigurationObject(Median, A, B, C).ToEnumerable();
                }

                else if (availableObject.Construction.Name.Equals(nameof(Incenter)))
                {
                    var A = availableObject.PassedArguments.FlattenedList[0];
                    var B = availableObject.PassedArguments.FlattenedList[1];
                    var C = availableObject.PassedArguments.FlattenedList[2];
                    var I = availableObject;

                    yield return new ConstructedConfigurationObject(Incircle, A, B, C).ToEnumerable();
                    yield return new ConstructedConfigurationObject(InternalAngleBisector, A, B, C).ToEnumerable();
                    yield return new ConstructedConfigurationObject(InternalAngleBisector, B, A, C).ToEnumerable();
                    yield return new ConstructedConfigurationObject(InternalAngleBisector, C, A, B).ToEnumerable();
                    yield return new ConstructedConfigurationObject(Circumcenter, B, I, C).ToEnumerable();
                    yield return new ConstructedConfigurationObject(Circumcenter, C, I, A).ToEnumerable();
                    yield return new ConstructedConfigurationObject(Circumcenter, A, I, B).ToEnumerable();
                }

                else if (availableObject.Construction.Name.Equals(nameof(Centroid)))
                {
                    var A = availableObject.PassedArguments.FlattenedList[0];
                    var B = availableObject.PassedArguments.FlattenedList[1];
                    var C = availableObject.PassedArguments.FlattenedList[2];

                    yield return new ConstructedConfigurationObject(Median, A, B, C).ToEnumerable();
                    yield return new ConstructedConfigurationObject(Median, B, A, C).ToEnumerable();
                    yield return new ConstructedConfigurationObject(Median, C, A, B).ToEnumerable();
                }

                else if (availableObject.Construction.Name.Equals(nameof(Incircle)))
                {
                    var A = availableObject.PassedArguments.FlattenedList[0];
                    var B = availableObject.PassedArguments.FlattenedList[1];
                    var C = availableObject.PassedArguments.FlattenedList[2];

                    yield return new ConstructedConfigurationObject(NinePointCircle, A, B, C).ToEnumerable();
                }

                else if (availableObject.Construction.Name.Equals(nameof(Excenter)))
                {
                    var A = availableObject.PassedArguments.FlattenedList[0];
                    var B = availableObject.PassedArguments.FlattenedList[1];
                    var C = availableObject.PassedArguments.FlattenedList[2];
                    var E = availableObject;

                    yield return new ConstructedConfigurationObject(ExternalAngleBisector, B, A, C).ToEnumerable();
                    yield return new ConstructedConfigurationObject(ExternalAngleBisector, C, A, B).ToEnumerable();
                    yield return new ConstructedConfigurationObject(InternalAngleBisector, A, B, C).ToEnumerable();
                    yield return new ConstructedConfigurationObject(Circumcenter, B, E, C).ToEnumerable();
                    yield return new ConstructedConfigurationObject(Circumcenter, C, E, A).ToEnumerable();
                    yield return new ConstructedConfigurationObject(Circumcenter, A, E, B).ToEnumerable();
                }

                else if (availableObject.Construction.Name.Equals(nameof(Excircle)))
                {
                    var A = availableObject.PassedArguments.FlattenedList[0];
                    var B = availableObject.PassedArguments.FlattenedList[1];
                    var C = availableObject.PassedArguments.FlattenedList[2];

                    yield return new ConstructedConfigurationObject(NinePointCircle, A, B, C).ToEnumerable();
                }

                else if (availableObject.Construction.Name.Equals(nameof(Midline)))
                {
                    var A = availableObject.PassedArguments.FlattenedList[0];
                    var B = availableObject.PassedArguments.FlattenedList[1];
                    var C = availableObject.PassedArguments.FlattenedList[2];

                    yield return new[]
                    {
                        new ConstructedConfigurationObject(Midpoint, A, B),
                        new ConstructedConfigurationObject(Midpoint, A, C)
                    };
                }

                else if (availableObject.Construction.Name.Equals(nameof(Median)))
                {
                    var A = availableObject.PassedArguments.FlattenedList[0];
                    var B = availableObject.PassedArguments.FlattenedList[1];
                    var C = availableObject.PassedArguments.FlattenedList[2];

                    yield return new ConstructedConfigurationObject(ParallelogramPoint, A, B, C).ToEnumerable();
                    yield return new ConstructedConfigurationObject(Centroid, A, B, C).ToEnumerable();
                    yield return new ConstructedConfigurationObject(Midpoint, B, C).ToEnumerable();
                }

                else if (availableObject.Construction.Name.Equals(nameof(TangentLine)))
                {
                    var A = availableObject.PassedArguments.FlattenedList[0];
                    var B = availableObject.PassedArguments.FlattenedList[1];
                    var C = availableObject.PassedArguments.FlattenedList[2];

                    yield return new ConstructedConfigurationObject(Circumcenter, A, B, C).ToEnumerable();
                }

                else if (availableObject.Construction.Name.Equals(nameof(LineThroughCircumcenter)))
                {
                    var A = availableObject.PassedArguments.FlattenedList[0];
                    var B = availableObject.PassedArguments.FlattenedList[1];
                    var C = availableObject.PassedArguments.FlattenedList[2];

                    yield return new ConstructedConfigurationObject(Circumcenter, A, B, C).ToEnumerable();
                }

                else if (availableObject.Construction.Name.Equals(nameof(OppositePointOnCircumcircle)))
                {
                    var A = availableObject.PassedArguments.FlattenedList[0];
                    var B = availableObject.PassedArguments.FlattenedList[1];
                    var C = availableObject.PassedArguments.FlattenedList[2];

                    yield return new ConstructedConfigurationObject(Circumcenter, A, B, C).ToEnumerable();
                }

                else if (availableObject.Construction.Name.Equals(nameof(MidpointOfArc)))
                {
                    var A = availableObject.PassedArguments.FlattenedList[0];
                    var B = availableObject.PassedArguments.FlattenedList[1];
                    var C = availableObject.PassedArguments.FlattenedList[2];

                    yield return new ConstructedConfigurationObject(ExternalAngleBisector, A, B, C).ToEnumerable();
                }

                else if (availableObject.Construction.Name.Equals(nameof(MidpointOfOppositeArc)))
                {
                    var A = availableObject.PassedArguments.FlattenedList[0];
                    var B = availableObject.PassedArguments.FlattenedList[1];
                    var C = availableObject.PassedArguments.FlattenedList[2];

                    yield return new ConstructedConfigurationObject(InternalAngleBisector, A, B, C).ToEnumerable();
                }
            }
        }
    }
}