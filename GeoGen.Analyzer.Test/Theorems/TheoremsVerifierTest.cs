using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Theorems;
using GeoGen.Core.Utilities;
using Moq;
using NUnit.Framework;
using static GeoGen.Analyzer.Test.TestHelpers.Utilities;

namespace GeoGen.Analyzer.Test.Theorems
{
    [TestFixture]
    public class TheoremsVerifierTest
    {
        private static TheoremsVerifier Verifier(IEnumerable<VerifierOutput> outputs, int numberOfOutputs, IObjectsContainer[] containers)
        {
            var verifiers = outputs.Select
            (
                (output, i) =>
                {
                    var mock = new Mock<ITheoremVerifier>();
                    mock.Setup(s => s.TheoremType).Returns(TheoremType.CollinearPoints);
                    mock.Setup(s => s.GetOutput(It.IsAny<VerifierInput>()))
                            .Returns(Enumerable.Repeat(output, numberOfOutputs));

                    return mock.Object;
                }
            ).ToArray();

            var contextualContainer = SimpleMock<IContextualContainer>();

            var managerMock = new Mock<IObjectsContainersManager>();
            managerMock.Setup(s => s.GetEnumerator()).Returns(() => containers.AsEnumerable().GetEnumerator());

            var constructor = new Mock<ITheoremConstructor>();
            constructor.Setup(s => s.Construct(It.IsAny<List<GeometricalObject>>(), It.IsAny<ConfigurationObjectsMap>(), It.IsAny<TheoremType>()))
                    .Returns((Theorem) null);

            return new TheoremsVerifier(verifiers, managerMock.Object, contextualContainer, constructor.Object);
        }

        [Test]
        public void Test_Verifiers_Cant_Be_Null()
        {
            var contextualContainer = SimpleMock<IContextualContainer>();
            var theoremsConstructor = SimpleMock<ITheoremConstructor>();
            var manager = SimpleMock<IObjectsContainersManager>();

            Assert.Throws<ArgumentNullException>(() => new TheoremsVerifier(null, manager, contextualContainer, theoremsConstructor));
        }

        [Test]
        public void Test_Verifiers_Cant_Contain_Null()
        {
            var verifiers = new[] {SimpleMock<ITheoremVerifier>(), null};
            var contextualContainer = SimpleMock<IContextualContainer>();
            var theoremsConstructor = SimpleMock<ITheoremConstructor>();
            var manager = SimpleMock<IObjectsContainersManager>();

            Assert.Throws<ArgumentException>(() => new TheoremsVerifier(verifiers, manager, contextualContainer, theoremsConstructor));
        }

        [Test]
        public void Test_Manager_Cant_Be_Null()
        {
            var verifiers = new[] {SimpleMock<ITheoremVerifier>(), null};
            var contextualContainer = SimpleMock<IContextualContainer>();
            var theoremsConstructor = SimpleMock<ITheoremConstructor>();

            Assert.Throws<ArgumentNullException>(() => new TheoremsVerifier(verifiers, null, contextualContainer, theoremsConstructor));
        }

        [Test]
        public void Test_Contextual_Container_Cant_Be_Null()
        {
            var verifiers = new[] {SimpleMock<ITheoremVerifier>(), null};
            var theoremsConstructor = SimpleMock<ITheoremConstructor>();
            var manager = SimpleMock<IObjectsContainersManager>();

            Assert.Throws<ArgumentNullException>(() => new TheoremsVerifier(verifiers, manager, null, theoremsConstructor));
        }

        [Test]
        public void Test_Theorems_Constructor_Cant_Be_Null()
        {
            var verifiers = new[] {SimpleMock<ITheoremVerifier>(), null};
            var contextualContainer = SimpleMock<IContextualContainer>();
            var manager = SimpleMock<IObjectsContainersManager>();

            Assert.Throws<ArgumentNullException>(() => new TheoremsVerifier(verifiers, manager, contextualContainer, null));
        }

        [Test]
        public void Test_Old_Objects_Cant_Be_Null()
        {
            var verifier = Verifier(new VerifierOutput[0], 0, new IObjectsContainer[0]);

            Assert.Throws<ArgumentNullException>(() => verifier.FindTheorems(null, new List<ConstructedConfigurationObject>()).ToList());
        }

        [Test]
        public void Test_New_Objects_Cant_Be_Null()
        {
            var verifier = Verifier(new VerifierOutput[0], 0, new IObjectsContainer[0]);

            Assert.Throws<ArgumentNullException>(() => verifier.FindTheorems(null, new List<ConstructedConfigurationObject>()).ToList());
        }

        [TestCase(0, 0)]
        [TestCase(1, 1)]
        [TestCase(42, 42)]
        [TestCase(666, 666)]
        public void Test_With_Null_Verifier_Function_That_Accepts_All(int theoremsPerVerifier, int expectedResult)
        {
            var containers = Enumerable.Range(0, 5).Select(i => SimpleMock<IObjectsContainer>()).ToArray();

            var outputs = new[]
            {
                new VerifierOutput
                {
                    VerifierFunction = null
                }
            };

            var verifier = Verifier(outputs, theoremsPerVerifier, containers);

            var result = verifier.FindTheorems(new List<ConfigurationObject>(), new List<ConstructedConfigurationObject>()).Count();

            Assert.AreEqual(expectedResult, result);
        }

        [TestCase(0, 0)]
        [TestCase(1, 0)]
        [TestCase(42, 0)]
        [TestCase(666, 0)]
        public void Test_With_One_Verifier_Rejecting_All(int theoremsPerVerifier, int expectedResult)
        {
            var containers = Enumerable.Range(0, 5).Select(i => SimpleMock<IObjectsContainer>()).ToArray();

            var outputs = new[]
            {
                new VerifierOutput
                {
                    VerifierFunction = container => false
                }
            };

            var verifier = Verifier(outputs, theoremsPerVerifier, containers);

            var result = verifier.FindTheorems(new List<ConfigurationObject>(), new List<ConstructedConfigurationObject>()).Count();

            Assert.AreEqual(expectedResult, result);
        }

        [TestCase(0, 0)]
        [TestCase(1, 3)]
        [TestCase(42, 126)]
        [TestCase(666, 1998)]
        public void Test_With_More_Verifiers(int theoremsPerVerifier, int expectedResult)
        {
            var containers = Enumerable.Range(0, 2).Select(i => SimpleMock<IObjectsContainer>()).ToArray();

            var outputs = new[]
            {
                new VerifierOutput
                {
                    VerifierFunction = container => false
                },
                new VerifierOutput
                {
                    VerifierFunction = null
                },
                new VerifierOutput
                {
                    VerifierFunction = container => true
                },
                new VerifierOutput
                {
                    VerifierFunction = container => container == containers[0]
                },
                new VerifierOutput
                {
                    VerifierFunction = container => container == containers[1]
                },
                new VerifierOutput
                {
                    VerifierFunction = container => container == containers[0] || container == containers[1]
                }
            };

            var verifier = Verifier(outputs, theoremsPerVerifier, containers);

            var result = verifier.FindTheorems(new List<ConfigurationObject>(), new List<ConstructedConfigurationObject>()).Count();

            Assert.AreEqual(expectedResult, result);
        }
    }
}