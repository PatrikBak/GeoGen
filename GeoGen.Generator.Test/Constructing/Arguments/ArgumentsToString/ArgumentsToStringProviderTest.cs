using System;
using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString;
using GeoGen.Generator.Constructing.Arguments.ArgumentsToString;
using Moq;
using NUnit.Framework;

namespace GeoGen.Generator.Test.Constructing.Arguments.ArgumentsToString
{
    [TestFixture]
    public class ArgumentsToStringProviderTest
    {
        private static ArgumentsToStringProvider Provider()
        {
            var provider = new DefaultObjectToStringProvider();

            return new ArgumentsToStringProvider(provider, ", ", " ");
        }

        private static IObjectToStringProvider ObjectProvider()
        {
            var mock = new Mock<IObjectToStringProvider>();
            mock.Setup(s => s.ConvertToString(It.IsAny<ConfigurationObject>()))
                    .Returns<ConfigurationObject>(o => (o.Id + 1).ToString());

            return mock.Object;
        }

        [Test]
        public void Test_List_Is_Not_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Provider().ConvertToString(null));
        }

        [Test]
        public void Test_List_Cannot_Be_Empty()
        {
            Assert.Throws<ArgumentException>(() => Provider().ConvertToString(new List<ConstructionArgument>()));
        }

        [Test]
        public void Custom_Object_To_String_Provider_Cant_Be_Null()
        {
            var looseObject = new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1};
            var argument = new ObjectConstructionArgument(looseObject);
            var args = new List<ConstructionArgument> {argument};

            Assert.Throws<ArgumentNullException>(() => Provider().ConvertToString(args, null));
        }

        [Test]
        public void Test_One_Object()
        {
            var looseObject = new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1};
            var argument = new ObjectConstructionArgument(looseObject);
            var args = new List<ConstructionArgument> {argument};

            var result1 = Provider().ConvertToString(args);
            var result2 = Provider().ConvertToString(args, ObjectProvider());

            Assert.AreEqual("(1)", result1);
            Assert.AreEqual("(2)", result2);
        }

        [Test]
        public void Test_More_Objects()
        {
            var looseObject1 = new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1};
            var looseObject2 = new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 2};
            var looseObject3 = new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 3};
            var argument1 = new ObjectConstructionArgument(looseObject1);
            var argument2 = new ObjectConstructionArgument(looseObject2);
            var argument3 = new ObjectConstructionArgument(looseObject3);

            var args = new List<ConstructionArgument> {argument1, argument2, argument3};

            var result1 = Provider().ConvertToString(args);
            var result2 = Provider().ConvertToString(args, ObjectProvider());

            Assert.AreEqual("(1, 2, 3)", result1);
            Assert.AreEqual("(2, 3, 4)", result2);
        }

        [Test]
        public void Test_Complex_Arguments()
        {
            var looseObject1 = new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1};
            var looseObject2 = new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 2};
            var looseObject3 = new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 3};
            var argument1 = new ObjectConstructionArgument(looseObject1);
            var argument2 = new ObjectConstructionArgument(looseObject2);
            var argument3 = new ObjectConstructionArgument(looseObject3);

            var set1 = new SetConstructionArgument(new HashSet<ConstructionArgument> {argument1, argument2});
            var set2 = new SetConstructionArgument(new HashSet<ConstructionArgument> {argument2, argument3});

            var finalSet = new SetConstructionArgument(new HashSet<ConstructionArgument> {set1, set2});
            var finalList = new List<ConstructionArgument> {argument1, finalSet, argument3};

            var result1 = Provider().ConvertToString(finalList);
            var result2 = Provider().ConvertToString(finalList, ObjectProvider());

            Assert.AreEqual("(1, {{1 2} {2 3}}, 3)", result1);
            Assert.AreEqual("(2, {{2 3} {3 4}}, 4)", result2);
        }
    }
}