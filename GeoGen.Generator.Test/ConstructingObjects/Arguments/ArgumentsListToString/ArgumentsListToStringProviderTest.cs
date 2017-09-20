using System;
using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Generator.ConstructingConfigurations.ObjectToString;
using GeoGen.Generator.ConstructingConfigurations.ObjectToString.ObjectIdResolving;
using GeoGen.Generator.ConstructingObjects.Arguments.ArgumentsListToString;
using Moq;
using NUnit.Framework;

namespace GeoGen.Generator.Test.ConstructingObjects.Arguments.ArgumentsListToString
{
    [TestFixture]
    public class ArgumentsListToStringProviderTest
    {
        private static ArgumentsListToStringProvider Provider()
        {
            return new ArgumentsListToStringProvider(DefaultArgumentProvider());
        }

        private static DefaultObjectToStringProvider DefaultArgumentProvider()
        {
            var resolver = new DefaultObjectIdResolver();
            var objectProvider = new DefaultObjectToStringProvider(resolver);

            return objectProvider;
        }

        private static IObjectToStringProvider CustomObjectProvider()
        {
            var mock = new Mock<IObjectToStringProvider>();
            mock.Setup(s => s.ConvertToString(It.IsAny<ConfigurationObject>()))
                    .Returns<ConfigurationObject>(o => (o.Id + 1).ToString());

            return mock.Object;
        }

        [Test]
        public void Test_Default_Provider_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new ArgumentsListToStringProvider(null));
        }

        [Test]
        public void Test_Passed_List_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Provider().ConvertToString(null));
        }

        [Test]
        public void Test_Passed_List_Cant_Be_Empty()
        {
            Assert.Throws<ArgumentException>(() => Provider().ConvertToString(new List<ConstructionArgument>()));
        }

        [Test]
        public void Test_Passed_Object_To_String_Provider_Cant_Be_Null()
        {
            var looseObject = new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1};
            var argument = new ObjectConstructionArgument(looseObject);
            var args = new List<ConstructionArgument> {argument};

            Assert.Throws<ArgumentNullException>(() => Provider().ConvertToString(args, null));
        }

        [Test]
        public void Test_One_Simple_Argument()
        {
            var looseObject = new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1};
            var argument = new ObjectConstructionArgument(looseObject);
            var args = new List<ConstructionArgument> {argument};

            var result1 = Provider().ConvertToString(args);
            var result2 = Provider().ConvertToString(args, CustomObjectProvider());

            Assert.AreEqual("(1)", result1);
            Assert.AreEqual("(2)", result2);
        }

        [Test]
        public void Test_More_Simple_Arguments()
        {
            var looseObject1 = new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1};
            var looseObject2 = new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 2};
            var looseObject3 = new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 3};
            var argument1 = new ObjectConstructionArgument(looseObject1);
            var argument2 = new ObjectConstructionArgument(looseObject2);
            var argument3 = new ObjectConstructionArgument(looseObject3);

            var args = new List<ConstructionArgument> {argument1, argument2, argument3};

            var result1 = Provider().ConvertToString(args);
            var result2 = Provider().ConvertToString(args, CustomObjectProvider());

            Assert.AreEqual("(1,2,3)", result1);
            Assert.AreEqual("(2,3,4)", result2);
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
            var result2 = Provider().ConvertToString(finalList, CustomObjectProvider());

            Assert.AreEqual("(1,{{1;2};{2;3}},3)", result1);
            Assert.AreEqual("(2,{{2;3};{3;4}},4)", result2);
        }

        [Test]
        public void Test_Default_Object_Provider_Is_Used_With_Default_Argument_Provider()
        {
            var looseObject = new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1};
            var argument = new ObjectConstructionArgument(looseObject);
            var args = new List<ConstructionArgument> {argument};

            var result1 = Provider().ConvertToString(args);
            var result2 = Provider().ConvertToString(args, DefaultArgumentProvider());

            Assert.AreEqual(result1, result2);
        }
    }
}