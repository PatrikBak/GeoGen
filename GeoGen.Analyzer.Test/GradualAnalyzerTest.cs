using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Analyzer.Objects;
using GeoGen.Analyzer.Test.TestHelpers;
using GeoGen.Analyzer.Theorems;
using GeoGen.Core.Configurations;
using GeoGen.Core.Theorems;
using GeoGen.Core.Utilities;
using GeoGen.Utilities;
using Moq;
using NUnit.Framework;
using static GeoGen.Analyzer.Test.TestHelpers.Utilities;

namespace GeoGen.Analyzer.Test
{
    [TestFixture]
    public class GradualAnalyzerTest
    {
        private static readonly Theorem DummyTheorem = null;

        private static List<Theorem> _theorems;

        private static GradualAnalyzer Analyzer(RegistrationResult result = null, bool containerContainsDummy = false)
        {
            var verifier = new Mock<ITheoremsVerifier>();
            verifier.Setup(s => s.FindTheorems(It.IsAny<List<ConfigurationObject>>(), It.IsAny<List<ConstructedConfigurationObject>>()))
                    .Returns(() => new List<Theorem> {DummyTheorem, DummyTheorem});

            var registrar = new Mock<IGeometryRegistrar>();
            registrar.Setup(s => s.Register(It.IsAny<List<ConstructedConfigurationObject>>()))
                    .Returns(result);

            var container = new Mock<ITheoremsContainer>();
            _theorems = new List<Theorem>();

            container.Setup(s => s.Contains(It.IsAny<Theorem>())).Returns<Theorem>(_theorems.Contains);
            container.Setup(s => s.Add(It.IsAny<Theorem>())).Callback<Theorem>(_theorems.Add);
            container.Setup(s => s.GetEnumerator()).Returns(() => _theorems.GetEnumerator());

            if (containerContainsDummy)
                _theorems.Add(DummyTheorem);

            return new GradualAnalyzer(registrar.Object, verifier.Object, container.Object);
        }

        [Test]
        public void Test_Registrar_Cant_Be_Null()
        {
            var registrar = SimpleMock<IGeometryRegistrar>();
            var container = SimpleMock<ITheoremsContainer>();

            Assert.Throws<ArgumentNullException>(() => new GradualAnalyzer(registrar, null, container));
        }

        [Test]
        public void Test_Verifier_Cant_Be_Null()
        {
            var verifier = SimpleMock<ITheoremsVerifier>();
            var container = SimpleMock<ITheoremsContainer>();

            Assert.Throws<ArgumentNullException>(() => new GradualAnalyzer(null, verifier, container));
        }

        [Test]
        public void Test_Container_Cant_Be_Null()
        {
            var verifier = SimpleMock<ITheoremsVerifier>();
            var registrar = SimpleMock<IGeometryRegistrar>();

            Assert.Throws<ArgumentNullException>(() => new GradualAnalyzer(registrar, verifier, null));
        }

        [Test]
        public void Test_Old_Objects_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Analyzer().Analyze(null, new List<ConstructedConfigurationObject>()));
        }

        [Test]
        public void Test_New_Objects_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Analyzer().Analyze(new List<ConfigurationObject>(), null));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Test_Without_Duplicates_And_With_Constructible_Objects(bool containsDummy)
        {
            var oldObjects = ConfigurationObjects.Objects(6, ConfigurationObjectType.Point).Cast<ConfigurationObject>().ToList();
            var newObjects = new List<ConstructedConfigurationObject>();

            var analyzer = Analyzer
            (
                new RegistrationResult
                {
                    GeometricalDuplicates = new Dictionary<ConfigurationObject, ConfigurationObject>(),
                    CanBeConstructed = true
                }, containsDummy
            );

            var output = analyzer.Analyze(oldObjects, newObjects);

            Assert.IsTrue(output.UnambiguouslyConstructible);

            if (containsDummy)
            {
                Assert.AreEqual(0, output.Theorems.Count);
            }
            else
            {
                Assert.AreEqual(2, output.Theorems.Count);
                Assert.IsTrue(output.Theorems.Contains(DummyTheorem));
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Test_Without_Duplicates_And_With_Notconstructible_Objects(bool containsDummy)
        {
            var oldObjects = ConfigurationObjects.Objects(6, ConfigurationObjectType.Point).Cast<ConfigurationObject>().ToList();
            var newObjects = new List<ConstructedConfigurationObject>();

            var analyzer = Analyzer
            (
                new RegistrationResult
                {
                    GeometricalDuplicates = new Dictionary<ConfigurationObject, ConfigurationObject>(),
                    CanBeConstructed = false
                }, containsDummy
            );

            var output = analyzer.Analyze(oldObjects, newObjects);

            Assert.IsFalse(output.UnambiguouslyConstructible);
            Assert.AreEqual(0, output.Theorems.Count);
        }

        [TestCase(true, true)]
        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(false, false)]
        public void Test_With_Duplicates(bool canBeConstructed, bool containsDummy)
        {
            var oldObjects = ConfigurationObjects.Objects(6, ConfigurationObjectType.Point).Cast<ConfigurationObject>().ToList();
            var newObjects = new List<ConstructedConfigurationObject>();

            var analyzer = Analyzer
            (
                new RegistrationResult
                {
                    GeometricalDuplicates = new Dictionary<ConfigurationObject, ConfigurationObject>
                    {
                        {oldObjects[0], oldObjects[1]},
                        {oldObjects[2], oldObjects[3]}
                    },
                    CanBeConstructed = canBeConstructed
                }, containsDummy
            );

            var output = analyzer.Analyze(oldObjects, newObjects);

            Assert.IsFalse(output.UnambiguouslyConstructible);

            Assert.AreEqual(2, output.Theorems.Count);
            Assert.IsFalse(output.Theorems.Contains(DummyTheorem));
            Assert.IsTrue(output.Theorems.All(t => t.Type == TheoremType.SameObjects));

            bool TheoremContaining(ConfigurationObject o1, ConfigurationObject o2)
            {
                return output.Theorems.Any
                (
                    t =>
                    {
                        var involvedObjects = t.InvolvedObjects;

                        if (involvedObjects.Count != 2)
                            return false;

                        if (involvedObjects.Any(o => o.InternalObjects.Count != 1))
                            return false;

                        return involvedObjects
                                .SelectMany(o => o.InternalObjects)
                                .ToSet()
                                .SetEquals(new HashSet<ConfigurationObject> {o1, o2});
                    }
                );
            }

            Assert.IsTrue(output.Theorems.Any(t => TheoremContaining(oldObjects[0], oldObjects[1])));
            Assert.IsTrue(output.Theorems.Any(t => TheoremContaining(oldObjects[2], oldObjects[3])));
        }
    }
}