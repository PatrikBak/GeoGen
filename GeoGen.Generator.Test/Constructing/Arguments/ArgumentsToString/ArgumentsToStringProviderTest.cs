using System;
using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Generator.Constructing.Arguments.ArgumentsToString;
using NUnit.Framework;

namespace GeoGen.Generator.Test.Constructing.Arguments.ArgumentsToString
{
    [TestFixture]
    public class ArgumentsToStringProviderTest
    {
        private static ArgumentsToStringProvider Provider()
        {
            return new ArgumentsToStringProvider(", ");
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
        public void Test_One_Object()
        {
            var looseObject = new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1};
            var argument = new ObjectConstructionArgument(looseObject);
            var result = Provider().ConvertToString(new List<ConstructionArgument> {argument});

            Assert.AreEqual("1", result);
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

            var result = Provider().ConvertToString(new List<ConstructionArgument> {argument1, argument2, argument3});

            Assert.AreEqual("1, 2, 3", result);
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

            var result = Provider().ConvertToString(finalList);

            Assert.AreEqual("1, {{1, 2}, {2, 3}}, 3", result);
        }
    }
}