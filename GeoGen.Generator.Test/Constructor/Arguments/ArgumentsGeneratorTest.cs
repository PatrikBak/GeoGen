using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Constructions.Parameters;
using GeoGen.Core.Utilities;
using GeoGen.Core.Utilities.Combinator;
using GeoGen.Core.Utilities.Variations;
using GeoGen.Generator.Constructor.Arguments;
using GeoGen.Generator.Constructor.Arguments.Container;
using GeoGen.Generator.Constructor.Arguments.SignatureMatching;
using GeoGen.Generator.Wrappers;
using Moq;
using NUnit.Framework;

namespace GeoGen.Generator.Test.Constructor.Arguments
{
    [TestFixture]
    public class ArgumentsGeneratorTest
    {
        private static List<LooseConfigurationObject> Objets(int count)
        {
            return Enumerable.Range(0, count).
            Select(i => new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = i}).
            ToList();
        }
        
        private ArgumentsGenerator TestGenerator()
        {
            var combinator = new Combinator<ConfigurationObjectType, IEnumerator<ConfigurationObject>>();
            var subsetGenerator = new SubsetsGenerator<ConfigurationObject>();
            var variationsProvider = new VariationsProvider<ConfigurationObject>(subsetGenerator);
            var signatureMatcher = new ConstructionSignatureMatcher();
            var argumentsContainer = new ArgumentsContainer();

            return new ArgumentsGenerator(combinator, signatureMatcher, variationsProvider, argumentsContainer);
        }

        private static ConstructionWrapper Midpoint()
        {
            var mock = new Mock<Construction>();
            mock.Setup(s => s.OutputType).
            Returns(ConfigurationObjectType.Point);
            mock.Setup(s => s.ConstructionParameters).
            Returns
            (
                new List<ConstructionParameter>
                {
                    new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 2)
                });

            var dictonary = new Dictionary<ConfigurationObjectType, int>
            {
                {ConfigurationObjectType.Point, 2}
            };

            return new ConstructionWrapper {Construction = mock.Object, ObjectTypesToNeededCount = dictonary};
        }

        private static ConstructionWrapper Intersection()
        {
            var mock = new Mock<Construction>();
            mock.Setup(s => s.OutputType).
            Returns(ConfigurationObjectType.Point);
            mock.Setup(s => s.ConstructionParameters).
            Returns
            (
                new List<ConstructionParameter>
                {
                    new SetConstructionParameter(new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 2), 2)
                });

            var dictonary = new Dictionary<ConfigurationObjectType, int>
            {
                {ConfigurationObjectType.Point, 4}
            };

            return new ConstructionWrapper {Construction = mock.Object, ObjectTypesToNeededCount = dictonary};
        }

        private static ConfigurationWrapper Configuration(int numberOfPoints)
        {
            var configuration = new Configuration(new HashSet<LooseConfigurationObject>(Objets(numberOfPoints)), new HashSet<ConstructedConfigurationObject>());
            var map = new Dictionary<ConfigurationObjectType, List<ConfigurationObject>> {{ConfigurationObjectType.Point, new List<ConfigurationObject>(configuration.LooseObjects)}};

            return new ConfigurationWrapper {Configuration = configuration, ObjectTypeToObjects = map};
        }

        private static string VerificationString(IReadOnlyList<ConstructionArgument> output)
        {
            return string.Join(", ", output.Select(ArgToString));
        }

        private static string ArgToString(ConstructionArgument arg)
        {
            if (arg is ObjectConstructionArgument objArg)
            {
                var id = objArg.PassedObject.Id;
                var asChar = id;

                return asChar.ToString();
            }

            var setArg = arg as SetConstructionArgument;

            var individualArgs = setArg.PassedArguments.Select(ArgToString).ToList();
            individualArgs.Sort();

            return "{" + string.Join(", ", individualArgs) + "}";
        }

        [Test]
        public void Test()
        {
            var midpoint = Intersection();
            var configuration = Configuration(10);

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var result = TestGenerator().GenerateArguments(configuration, midpoint).Select(VerificationString).ToList();
            stopwatch.Stop();
            Console.WriteLine($"Generated: {result.Count}");
            Console.WriteLine($"Total: {stopwatch.ElapsedMilliseconds}");
            //foreach (var s in result)
            //{
            //    Console.WriteLine(s);
            //}
        }
    }
}