using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Generator.Constructing.Arguments.ArgumentsToString;
using GeoGen.Generator.Constructing.Arguments.Container;
using NUnit.Framework;

namespace GeoGen.Generator.Test.Constructing.Arguments.Container
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

        [Test]
        public void Arguments_To_String_Provider_Cannot_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new ArgumentsContainer(null));
        }

        [Test]
        public void Test_Container_One_Set_Two_Elements()
        {
            var container = new ArgumentsContainer(new ArgumentsToStringProvider());

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
            var container = new ArgumentsContainer(new ArgumentsToStringProvider());

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
            var container = new ArgumentsContainer(new ArgumentsToStringProvider());

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
      
        [Test]
        public void Test_Remove_Elements_From()
        {
            var args = Enumerable.Range(0, 15).Select(i => new List<ConstructionArgument> {NextArgument()}).ToList();

            var container = new ArgumentsContainer(new ArgumentsToStringProvider());

            foreach (var arguments in args)
            {
                container.Add(arguments);
            }

            var newContainer = new ArgumentsContainer(new ArgumentsToStringProvider());

            var argumentsToRemove = args
                    .Skip(5)
                    .Take(4)
                    .Select(
                        arguments =>
                        {
                            var oldArgument = arguments[0] as ObjectConstructionArgument;
                            var newArgument = new ObjectConstructionArgument(oldArgument?.PassedObject);

                            return new List<ConstructionArgument> {newArgument};
                        });

            foreach (var arguments in argumentsToRemove)
            {
                newContainer.Add(arguments);
            }

            container.RemoveElementsFrom(newContainer);

            Assert.AreEqual(4, newContainer.Count());
            Assert.AreEqual(11, container.Count());
        }
    }
}