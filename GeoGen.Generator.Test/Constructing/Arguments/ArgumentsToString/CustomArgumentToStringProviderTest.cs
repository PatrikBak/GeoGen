using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString.ConfigurationObjectIdResolving;
using GeoGen.Generator.ConstructingObjects.Arguments.ArgumentsToString;
using Moq;
using NUnit.Framework;
using static GeoGen.Generator.Test.TestHelpers.ConfigurationObjects;

namespace GeoGen.Generator.Test.Constructing.Arguments.ArgumentsToString
{
    [TestFixture]
    public class CustomArgumentToStringProviderTest
    {
        private static CustomArgumentToStringProvider Provider()
        {
            var mock = new Mock<IObjectToStringProvider>();
            mock.Setup(s => s.ConvertToString(It.IsAny<ConfigurationObject>()))
                    .Returns<ConfigurationObject>
                    (
                        obj =>
                        {
                            var id = obj.Id ?? throw new Exception();

                            return (id + 10).ToString();
                        }
                    );
            var objectToString = mock.Object;

            return new CustomArgumentToStringProvider(objectToString, "; ");
        }

        [Test]
        public void Test_Object_Provider_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new DefaultArgumentToStringProvider(null));
        }

        [Test]
        public void Test_Argument_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Provider().ConvertArgument(null));
        }

        [Test]
        public void Test_Simple_Object_Argument()
        {
            var obj = new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1};
            var arg = new ObjectConstructionArgument(obj) {Id = 1};
            var asString = Provider().ConvertArgument(arg);

            Assert.AreEqual("11", asString);
        }

        [Test]
        public void Test_Constructed_Object_Argument()
        {
            var obj = new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1};
            var arg = new ObjectConstructionArgument(obj) {Id = 1};
            var constructedObj = ConstructedObject(42, 0, new List<ConstructionArgument> {arg}, 1);
            var finalArg = new ObjectConstructionArgument(constructedObj) {Id = 2};
            var asString = Provider().ConvertArgument(finalArg);

            Assert.AreEqual("11", asString);
        }

        [Test]
        public void Test_Simple_Set_Argument()
        {
            var objs = Objects(3, ConfigurationObjectType.Point);

            var setArg = new SetConstructionArgument
            (
                new HashSet<ConstructionArgument>
                {
                    new SetConstructionArgument
                    (
                        new HashSet<ConstructionArgument>
                        {
                            new ObjectConstructionArgument(objs[0]) {Id = 0},
                            new ObjectConstructionArgument(objs[1]) {Id = 1}
                        }
                    ) {Id = 5},
                    new SetConstructionArgument
                    (
                        new HashSet<ConstructionArgument>
                        {
                            new ObjectConstructionArgument(objs[0]) {Id = 0},
                            new ObjectConstructionArgument(objs[2]) {Id = 2}
                        }
                    ) {Id = 4}
                }
            ) {Id = 6};

            var asString = Provider().ConvertArgument(setArg);

            Assert.AreEqual("{{11; 12}; {11; 13}}", asString);
        }

        [Test]
        public void Test_Complex_Set_Argument()
        {
            var objs = Objects(2, ConfigurationObjectType.Point);
            var arg = new ObjectConstructionArgument(objs[0]) {Id = 1};

            var setArg = new SetConstructionArgument
            (
                new HashSet<ConstructionArgument>
                {
                    arg,
                    new ObjectConstructionArgument(objs[1]) {Id = 2}
                }
            ) {Id = 3};

            var asString = Provider().ConvertArgument(setArg);
            Assert.AreEqual("{11; 12}", asString);
        }

        [Test]
        public void Test_Caching_Is_Happening()
        {
            var callCounter = 0;

            var resolverMock = new Mock<IObjectToStringProvider>();
            resolverMock.Setup(s => s.ConvertToString(It.IsAny<LooseConfigurationObject>()))
                    .Returns<LooseConfigurationObject>
                    (
                        o =>
                        {
                            callCounter++;
                            return o.Id.ToString();
                        }
                    );

            var provider = new CustomArgumentToStringProvider(resolverMock.Object);
            var objs = Objects(2, ConfigurationObjectType.Point);
            var arg = new ObjectConstructionArgument(objs[0]) {Id = 1};

            var setArg = new SetConstructionArgument
            (
                new HashSet<ConstructionArgument>
                {
                    arg,
                    new SetConstructionArgument
                    (
                        new HashSet<ConstructionArgument>
                        {
                            arg,
                            new ObjectConstructionArgument(objs[1]) {Id = 2}
                        }
                    ) {Id = 3}
                }
            ) {Id = 4};

            provider.ConvertArgument(setArg);

            Assert.AreEqual(2, callCounter);
        }
    }
}