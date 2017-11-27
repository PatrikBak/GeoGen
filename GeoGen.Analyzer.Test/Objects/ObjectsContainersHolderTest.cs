using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Analyzer.Objects;
using GeoGen.Core.Configurations;
using Moq;
using NUnit.Framework;
using static GeoGen.Analyzer.Test.TestHelpers.Utilities;

namespace GeoGen.Analyzer.Test.Objects
{
    [TestFixture]
    public class ObjectsContainersHolderTest
    {
        private static ObjectsContainersHolder Holder()
        {
            var factory = new Mock<IObjectsContainersFactory>();
            factory.Setup(s => s.CreateContainer(It.IsAny<IEnumerable<LooseConfigurationObject>>()))
                    .Returns(SimpleMock<IObjectsContainer>);

            return new ObjectsContainersHolder(factory.Object);
        }

        [Test]
        public void Test_Factory_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new ObjectsContainersHolder(null));
        }

        [Test]
        public void Test_Loose_Objects_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Holder().Initialize(null));
        }

        [Test]
        public void Test_Loose_Objects_Cant_Contain_Null()
        {
            var objects = new List<LooseConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 0},
                null,
                new LooseConfigurationObject(ConfigurationObjectType.Line) {Id = 2}
            };

            Assert.Throws<ArgumentException>(() => Holder().Initialize(objects));
        }

        [Test]
        public void Test_Loose_Objects_Cant_Contain_Duplicates()
        {
            var objects = new List<LooseConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1},
                new LooseConfigurationObject(ConfigurationObjectType.Line) {Id = 3},
                new LooseConfigurationObject(ConfigurationObjectType.Line) {Id = 3}
            };

            Assert.Throws<ArgumentException>(() => Holder().Initialize(objects));
        }

        [Test]
        public void Test_Number_Of_Containers_Is_Correct()
        {
            var objects = new List<LooseConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1},
                new LooseConfigurationObject(ConfigurationObjectType.Line) {Id = 2}
            };

            var holder = Holder();

            holder.Initialize(objects);
            
            Assert.AreEqual(ObjectsContainersHolder.NumberOfContainers, holder.Count());
        }
    }
}