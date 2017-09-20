using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString.ConfigurationObjectIdResolving;
using GeoGen.Generator.ConstructingObjects.Arguments.ArgumentsToString;
using GeoGen.Generator.ConstructingObjects.Arguments.Containers;
using GeoGen.Generator.Test.TestHelpers;
using NUnit.Framework;

namespace GeoGen.Generator.Test.Constructing.Arguments.Containers
{
    [TestFixture]
    public class ArgumentContainerTest
    {
        private static ArgumentContainer Container()
        {
            var resolver = new DefaultObjectIdResolver();
            var objectProvider = new DefaultObjectToStringProvider(resolver);
            var provider = new DefaultArgumentToStringProvider(objectProvider);

            return new ArgumentContainer(provider);
        }

        [Test]
        public void Test_Argument_Provider_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new ArgumentContainer(null));
        }

        [Test]
        public void Test_Argument_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Container().AddArgument(null));
        }

        [Test]
        public void Test_Simple_Object_Arguments()
        {
            var container = Container();
            var args = ConfigurationObjects.Objects(4, ConfigurationObjectType.Point)
                    .Select(obj => new ObjectConstructionArgument(obj))
                    .ToList();

            foreach (var argument in args)
            {
                container.AddArgument(argument);
                container.AddArgument(argument);
                container.AddArgument(argument);
            }

            Assert.AreEqual(4, container.Count());
        }

        [Test]
        public void Test_Simple_Set_Arguments()
        {
            var container = Container();
            var args = ConfigurationObjects.Objects(2, ConfigurationObjectType.Point)
                    .Select(obj => new ObjectConstructionArgument(obj))
                    .ToList();

            var set1 = new SetConstructionArgument(new HashSet<ConstructionArgument> { args[0], args[1] });
            var set2 = new SetConstructionArgument(new HashSet<ConstructionArgument> { args[1], args[0] });

            container.AddArgument(set1);
            container.AddArgument(set2);

            Assert.AreEqual(1, container.Count());
        }

        [Test]
        public void Test_Complex_Arguments()
        {
            var container = Container();
            var args = ConfigurationObjects.Objects(4, ConfigurationObjectType.Point)
                    .Select(obj => new ObjectConstructionArgument(obj))
                    .ToList();

            var set1 = new SetConstructionArgument(new HashSet<ConstructionArgument> {args[0], args[1]});
            var set2 = new SetConstructionArgument(new HashSet<ConstructionArgument> {args[0], args[2]});
            var set3 = new SetConstructionArgument(new HashSet<ConstructionArgument> {args[0], args[3]});
            var set4 = new SetConstructionArgument(new HashSet<ConstructionArgument> {args[1], args[2]});
            var set5 = new SetConstructionArgument(new HashSet<ConstructionArgument> {args[1], args[3]});
            var set6 = new SetConstructionArgument(new HashSet<ConstructionArgument> {args[2], args[3]});

            container.AddArgument(new SetConstructionArgument(new HashSet<ConstructionArgument> {set1, set6}));
            container.AddArgument(new SetConstructionArgument(new HashSet<ConstructionArgument> {set2, set5}));
            container.AddArgument(new SetConstructionArgument(new HashSet<ConstructionArgument> {set3, set4}));

            container.AddArgument(new SetConstructionArgument(new HashSet<ConstructionArgument> {set6, set1}));
            container.AddArgument(new SetConstructionArgument(new HashSet<ConstructionArgument> {set5, set2}));
            container.AddArgument(new SetConstructionArgument(new HashSet<ConstructionArgument> {set4, set3}));

            Assert.AreEqual(3, container.Count());
        }
    }
}