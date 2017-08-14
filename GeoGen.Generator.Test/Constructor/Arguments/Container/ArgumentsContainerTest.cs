using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Utilities;
using GeoGen.Generator.Constructor.Arguments.Container;
using Moq;
using NUnit.Framework;

namespace GeoGen.Generator.Test.Constructor.Arguments.Container
{
    [TestFixture]
    public class ArgumentsContainerTest
    {
        private static int _lastConfigurationObjectId;

        private static ObjectConstructionArgument NextArgument()
        {
            var cObject = new LooseConfigurationObject(ConfigurationObjectType.Point)
            {
                Id = _lastConfigurationObjectId++
            };

            return new ObjectConstructionArgument(cObject);
        }

        private static ArgumentsContainer TestArgumentsContainer()
        {
            var mock = new Mock<IEqualityComparer<ConfigurationObject>>();

            mock.Setup(comparer => comparer.GetHashCode(It.IsAny<ConfigurationObject>())).Returns(1);

            mock.Setup(comparer => comparer.Equals(It.IsAny<ConfigurationObject>(), It.IsAny<ConfigurationObject>()))
                .Returns<ConfigurationObject, ConfigurationObject>((o1, o2) => o1.Id == o2.Id);

            var constructionArgumentEqualityComparer = new ConstructionArgumentEqualityComparer(mock.Object);
            var listEqualityComparer = new ListEqualityComparer<ConstructionArgument>(constructionArgumentEqualityComparer);

            return new ArgumentsContainer(listEqualityComparer);
        }

        [Test]
        public void Test_ConstrucorIConfigurationObjectsComparer_Argument_Not_Null()
        {
            Assert.Throws<ArgumentNullException>(() => { new ArgumentsContainer(null); });
        }

        [Test]
        public void Test_Container_One_Set_Two_Elements()
        {
            var container = TestArgumentsContainer();

            var a1 = NextArgument();
            var a2 = NextArgument();

            var set1 = new SetConstructionArgument(new HashSet<ConstructionArgument> {a1, a2});
            var set2 = new SetConstructionArgument(new HashSet<ConstructionArgument> {a2, a1});

            container.Add(new List<ConstructionArgument> {set1});
            container.Add(new List<ConstructionArgument> {set2});

            Assert.AreEqual(1, container.Count());
        }

        [Test]
        public void Test_Container_One_Element_And_Two_Elements_Set()
        {
            var container = TestArgumentsContainer();

            var a1 = NextArgument();
            var a2 = NextArgument();
            var a3 = NextArgument();

            var set1 = new SetConstructionArgument(new HashSet<ConstructionArgument> {a1, a2});
            var set2 = new SetConstructionArgument(new HashSet<ConstructionArgument> {a2, a1});

            container.Add(new List<ConstructionArgument> {a3, set1});
            container.Add(new List<ConstructionArgument> {a3, set2});
            container.Add(new List<ConstructionArgument> {set1, a3});
            container.Add(new List<ConstructionArgument> {set2, a3});

            Assert.AreEqual(2, container.Count());
        }

        [Test]
        public void Test_Container_Two_Elements_Sets_Of_Size_Two()
        {
            var container = TestArgumentsContainer();

            var a1 = NextArgument();
            var a2 = NextArgument();
            var a3 = NextArgument();
            var a4 = NextArgument();

            var set1 = new SetConstructionArgument(new HashSet<ConstructionArgument> {a1, a2});
            var set2 = new SetConstructionArgument(new HashSet<ConstructionArgument> {a1, a3});
            var set3 = new SetConstructionArgument(new HashSet<ConstructionArgument> {a1, a4});
            var set4 = new SetConstructionArgument(new HashSet<ConstructionArgument> {a2, a3});
            var set5 = new SetConstructionArgument(new HashSet<ConstructionArgument> {a2, a4});
            var set6 = new SetConstructionArgument(new HashSet<ConstructionArgument> {a3, a4});

            container.Add(new List<ConstructionArgument> {new SetConstructionArgument(new HashSet<ConstructionArgument> {set1, set6})});
            container.Add(new List<ConstructionArgument> {new SetConstructionArgument(new HashSet<ConstructionArgument> {set2, set5})});
            container.Add(new List<ConstructionArgument> {new SetConstructionArgument(new HashSet<ConstructionArgument> {set3, set4})});

            container.Add(new List<ConstructionArgument> {new SetConstructionArgument(new HashSet<ConstructionArgument> {set6, set1})});
            container.Add(new List<ConstructionArgument> {new SetConstructionArgument(new HashSet<ConstructionArgument> {set5, set2})});
            container.Add(new List<ConstructionArgument> {new SetConstructionArgument(new HashSet<ConstructionArgument> {set4, set3})});

            Assert.AreEqual(3, container.Count());
        }
    }
}