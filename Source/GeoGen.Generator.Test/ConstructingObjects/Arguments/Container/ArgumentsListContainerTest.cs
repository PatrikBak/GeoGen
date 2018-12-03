//using System;
//using System.Collections.Generic;
//using System.Linq;
//using GeoGen.Core.Configurations;
//using GeoGen.Core.Constructions.Arguments;
//using GeoGen.Generator.ConstructingConfigurations.ObjectToString;
//using GeoGen.Generator.ConstructingConfigurations.ObjectToString.ObjectIdResolving;
//using GeoGen.Generator.ConstructingObjects.Arguments.ArgumentsListToString;
//using GeoGen.Generator.ConstructingObjects.Arguments.Container;
//using NUnit.Framework;
//using static GeoGen.Generator.Test.TestHelpers.ConfigurationObjects;

//namespace GeoGen.Generator.Test.ConstructingObjects.Arguments.Container
//{
//    [TestFixture]
//    public class ArgumentsListContainerTest
//    {
//        private static ArgumentsListContainer Container()
//        {
//            var idResolver = new DefaultObjectIdResolver();
//            var objectProvider = new DefaultObjectToStringProvider(idResolver);
//            //var argumentsProvider = new ArgumentsListToStringProvider(objectProvider);

//            //return new ArgumentsListContainer(argumentsProvider);
//            return null;
//        }

//        [Test]
//        public void Test_Arguments_To_String_Provider_Cant_Be_Null()
//        {
//            Assert.Throws<ArgumentNullException>(() => new ArgumentsListContainer(null));
//        }

//        [Test]
//        public void Test_Container_One_Set_With_Two_Elements()
//        {
//            var container = Container();

//            var args = Objects(2, ConfigurationObjectType.Point)
//                    .Select(obj => new ObjectConstructionArgument(obj))
//                    .ToList();

//            var a1 = args[0];
//            var a2 = args[1];

//            var set1 = new SetConstructionArgument(new HashSet<ConstructionArgument> {a1, a2});
//            var set2 = new SetConstructionArgument(new HashSet<ConstructionArgument> {a2, a1});

//            container.Add(new List<ConstructionArgument> {set1});
//            container.Add(new List<ConstructionArgument> {set2});

//            Assert.AreEqual(1, container.Count());
//        }

//        [Test]
//        public void Test_Container_One_Element_And_Two_Elements_Set()
//        {
//            var container = Container();

//            var args = Objects(3, ConfigurationObjectType.Point)
//                    .Select(obj => new ObjectConstructionArgument(obj))
//                    .ToList();

//            var a1 = args[0];
//            var a2 = args[1];
//            var a3 = args[2];

//            var set1 = new SetConstructionArgument(new HashSet<ConstructionArgument> {a1, a2});
//            var set2 = new SetConstructionArgument(new HashSet<ConstructionArgument> {a2, a1});

//            container.Add(new List<ConstructionArgument> {a3, set1});
//            container.Add(new List<ConstructionArgument> {a3, set2});
//            container.Add(new List<ConstructionArgument> {set1, a3});
//            container.Add(new List<ConstructionArgument> {set2, a3});

//            Assert.AreEqual(2, container.Count());
//        }

//        [Test]
//        public void Test_Container_Two_Elements_Sets_Of_Size_Two()
//        {
//            var container = Container();

//            var args = Objects(4, ConfigurationObjectType.Point)
//                    .Select(obj => new ObjectConstructionArgument(obj))
//                    .ToList();

//            var a1 = args[0];
//            var a2 = args[1];
//            var a3 = args[2];
//            var a4 = args[3];

//            var set1 = new SetConstructionArgument(new HashSet<ConstructionArgument> {a1, a2});
//            var set2 = new SetConstructionArgument(new HashSet<ConstructionArgument> {a1, a3});
//            var set3 = new SetConstructionArgument(new HashSet<ConstructionArgument> {a1, a4});
//            var set4 = new SetConstructionArgument(new HashSet<ConstructionArgument> {a2, a3});
//            var set5 = new SetConstructionArgument(new HashSet<ConstructionArgument> {a2, a4});
//            var set6 = new SetConstructionArgument(new HashSet<ConstructionArgument> {a3, a4});

//            container.Add(new List<ConstructionArgument> {new SetConstructionArgument(new HashSet<ConstructionArgument> {set1, set6})});
//            container.Add(new List<ConstructionArgument> {new SetConstructionArgument(new HashSet<ConstructionArgument> {set2, set5})});
//            container.Add(new List<ConstructionArgument> {new SetConstructionArgument(new HashSet<ConstructionArgument> {set3, set4})});

//            container.Add(new List<ConstructionArgument> {new SetConstructionArgument(new HashSet<ConstructionArgument> {set6, set1})});
//            container.Add(new List<ConstructionArgument> {new SetConstructionArgument(new HashSet<ConstructionArgument> {set5, set2})});
//            container.Add(new List<ConstructionArgument> {new SetConstructionArgument(new HashSet<ConstructionArgument> {set4, set3})});

//            Assert.AreEqual(3, container.Count());
//        }
//    }
//}