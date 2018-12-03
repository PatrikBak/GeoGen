//using System;
//using System.Collections.Generic;
//using System.Linq;
//using GeoGen.Analyzer;
//using GeoGen.Core.Configurations;
//using GeoGen.Core.Constructions.Arguments;
//using GeoGen.Core.Theorems;
//using Moq;
//using NUnit.Framework;
//using static GeoGen.Generator.Test.TestHelpers.Utilities;
//using static GeoGen.Generator.Test.TestHelpers.ConfigurationObjects;

//namespace GeoGen.Generator.Test.ConfigurationsHandling
//{
//    [TestFixture]
//    public class ConfigurationsHandlerTest
//    {
//        private static ConfigurationsHandler Handler(List<int> forbiddenIds)
//        {
//            var analyzer = new Mock<IGradualAnalyzer>();

//            var analyzerCalls = 0;

//            analyzer.Setup(s => s.Analyze(It.IsAny<List<ConfigurationObject>>(), It.IsAny<List<ConstructedConfigurationObject>>()))
//                    .Returns
//                    (
//                        () =>
//                        {
//                            analyzerCalls++;

//                            return new GradualAnalyzerOutput
//                            {
//                                Theorems = Enumerable.Repeat((Theorem) null, analyzerCalls).ToList(),
//                                UnambiguouslyConstructible = analyzerCalls % 3 != 0
//                            };
//                        }
//                    );

//            var container = new Mock<IConfigurationsManager>();
//            //container.Setup(s => s.ForbidConfigurationsContaining(It.IsAny<ConfigurationObject>()))
//            //        .Callback<ConfigurationObject>(o => forbiddenIds.Add(o.Id ?? throw new Exception()));

//            return new ConfigurationsHandler(analyzer.Object, container.Object);
//        }

//        [Test]
//        public void Test_Analyzer_Cant_Be_Null()
//        {
//            var container = SimpleMock<IConfigurationsManager>();

//            Assert.Throws<ArgumentNullException>(() => new ConfigurationsHandler(null, container));
//        }

//        [Test]
//        public void Test_Container_Cant_Be_Null()
//        {
//            var analyzer = SimpleMock<IGradualAnalyzer>();

//            Assert.Throws<ArgumentNullException>(() => new ConfigurationsHandler(analyzer, null));
//        }

//        [Test]
//        public void Test_Configurations_Cant_Be_Null()
//        {
//            Assert.Throws<ArgumentNullException>(() => Handler(new List<int>()).GenerateFinalOutput(null).ToList());
//        }

//        [TestCase(0)]
//        [TestCase(1)]
//        [TestCase(2)]
//        [TestCase(3)]
//        [TestCase(42)]
//        [TestCase(666)]
//        [TestCase(1000)]
//        public void Test_Generating_Output(int n)
//        {
//            var forbiddenIds = new List<int>();

//            var handler = Handler(forbiddenIds);

//            var args = new List<ConstructionArgument>
//            {
//                new ObjectConstructionArgument(new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = -1})
//            };

//            var wrapers = Enumerable.Range(0, n).Select
//            (
//                i => new ConfigurationWrapper
//                {
//                    LastAddedObjects = new List<ConstructedConfigurationObject> {ConstructedObject(42, 0, args, i + 1)}
//                }
//            );

//            var outputs = handler.GenerateFinalOutput(wrapers).ToList();

//            Assert.AreEqual(n, outputs.Count);

//            for (var i = 0; i < n; i++)
//            {
//                Assert.AreEqual(i + 1, outputs[i].Theorems.Count);
//            }

//            Assert.AreEqual(n / 3, forbiddenIds.Count);
//            Assert.IsTrue(forbiddenIds.All(i => i % 3 == 0));
//        }
//    }
//}