//using System;
//using System.Collections.Generic;
//using System.Linq;
//using GeoGen.Core.Configurations;
//using GeoGen.Core.Constructions.Arguments;
//using GeoGen.Core.Generator;
//using GeoGen.Generator.ConstructingConfigurations.IdsFixing;
//using GeoGen.Generator.ConstructingConfigurations.ObjectsContainer;
//using GeoGen.Generator.ConstructingConfigurations.ObjectToString;
//using GeoGen.Generator.ConstructingConfigurations.ObjectToString.ObjectIdResolving;
//using GeoGen.Generator.ConstructingObjects.Arguments.ArgumentsListToString;
//using NUnit.Framework;
//using static GeoGen.Generator.Test.TestHelpers.ConfigurationObjects;
//using static GeoGen.Generator.Test.TestHelpers.Utilities;
//using static GeoGen.Generator.Test.TestHelpers.Configurations;

//namespace GeoGen.Generator.Test.ConstructingConfigurations.IdsFixing
//{
//    [TestFixture]
//    public class IdsFixerTest
//    {
//        private static IConfigurationObjectsContainer _objectsContainer;

//        private static IdsFixer Fixer()
//        {
//            var defaultResolver = new DefaultObjectIdResolver();
//            var defaultToString = new DefaultObjectToStringProvider(defaultResolver);
//            var argsProvider = new ArgumentsListToStringProvider(defaultToString);
//            var provider = new DefaultFullObjectToStringProvider(argsProvider, defaultResolver);

//            _objectsContainer = new ConfigurationObjectsContainer(null, provider);
//            var objects = Objects(42, ConfigurationObjectType.Point, includeIds: false).ToList();
//            //_objectsContainer.Initialize(AsConfiguration(objects));

//            return new IdsFixer(_objectsContainer, DictionaryIdResolver());
//        }

//        private static DictionaryObjectIdResolver DictionaryIdResolver()
//        {
//            var dictionary = Enumerable
//                    .Range(0, 42)
//                    .ToDictionary(i => i, i => i + 1);

//            return new DictionaryObjectIdResolver(1, dictionary);
//        }

//        [Test]
//        public void Test_Objects_Container_Cant_Be_Null()
//        {
//            Assert.Throws<ArgumentNullException>(() => new IdsFixer(null, DictionaryIdResolver()));
//        }

//        [Test]
//        public void Test_Resolver_Cant_Be_Null()
//        {
//            var objectsContainer = SimpleMock<IConfigurationObjectsContainer>();

//            Assert.Throws<ArgumentNullException>(() => new IdsFixer(objectsContainer, null));
//        }

//        [Test]
//        public void Test_Passed_Object_Cant_Be_Null()
//        {
//            Assert.Throws<ArgumentNullException>(() => Fixer().FixObject(null));
//        }

//        [Test]
//        public void Test_Passed_Object_Must_Have_Id()
//        {
//            var fixer = Fixer();
//            var obj = new LooseConfigurationObject(ConfigurationObjectType.Point);

//            Assert.Throws<GeneratorException>(() => fixer.FixObject(obj));
//        }

//        [Test]
//        public void Test_Fixing_Loose_Object()
//        {
//            var fixer = Fixer();

//            var obj = new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 7};
//            var fixedObj = fixer.FixObject(obj);

//            Assert.AreEqual(8, fixedObj.Id);
//            Assert.AreSame(_objectsContainer[8], fixedObj);
//        }

//        [Test]
//        public void Test_Fixing_Simple_Constructed_Object()
//        {
//            var fixer = Fixer();

//            var args = new List<ConstructionArgument>
//            {
//                new SetConstructionArgument
//                (
//                    new HashSet<ConstructionArgument>
//                    {
//                        new ObjectConstructionArgument(_objectsContainer[7]),
//                        new ObjectConstructionArgument(_objectsContainer[8])
//                    }
//                )
//            };

//            var constructedObject = ConstructedObject(42, 0, args);
//            _objectsContainer.Add(constructedObject);
//            var fixedObject = fixer.FixObject(constructedObject);
//            var castedFixedObject = (ConstructedConfigurationObject) fixedObject;

//            Assert.AreEqual(44, fixedObject.Id ?? throw new Exception());
//            Assert.AreSame(constructedObject.Construction, castedFixedObject.Construction);
//            Assert.AreEqual(constructedObject.ObjectType, fixedObject.ObjectType);
//            Assert.AreEqual(constructedObject.Index, castedFixedObject.Index);

//            Assert.AreSame(_objectsContainer[44], fixedObject);

//            var newArgs = castedFixedObject.PassedArguments;
//            Assert.AreEqual(1, newArgs.Count);

//            var firstArg = newArgs[0];
//            Assert.IsInstanceOf<SetConstructionArgument>(firstArg);

//            var castedArg = (SetConstructionArgument) firstArg;

//            var passedArgs = castedArg.PassedArguments;
//            Assert.AreEqual(2, passedArgs.Count);

//            bool AnyWithId(int id)
//            {
//                return passedArgs.Any
//                (
//                    arg =>
//                    {
//                        var objectArg = (ObjectConstructionArgument) arg;
//                        var passedObject = objectArg.PassedObject;

//                        return passedObject == _objectsContainer[id] && passedObject.Id == id;
//                    }
//                );
//            }

//            Assert.IsTrue(AnyWithId(8));
//            Assert.IsTrue(AnyWithId(9));
//        }

//        [Test]
//        public void Test_Fixing_Complex_Nested_Object()
//        {
//            var fixer = Fixer();

//            var args1 = new List<ConstructionArgument>
//            {
//                new ObjectConstructionArgument(_objectsContainer[1]),
//                new ObjectConstructionArgument(_objectsContainer[3])
//            };
//            var obj1 = ConstructedObject(42, 1, args1);
//            _objectsContainer.Add(obj1);

//            var args2 = new List<ConstructionArgument>
//            {
//                new ObjectConstructionArgument(_objectsContainer[1]),
//                new ObjectConstructionArgument(obj1)
//            };
//            var obj2 = ConstructedObject(42, 1, args2);
//            _objectsContainer.Add(obj2);

//            var args3 = new List<ConstructionArgument>
//            {
//                new ObjectConstructionArgument(obj2),
//                new ObjectConstructionArgument(obj1)
//            };
//            var obj3 = ConstructedObject(42, 1, args3);
//            _objectsContainer.Add(obj3);

//            var fixedObj3 = (ConstructedConfigurationObject) fixer.FixObject(obj3);
//            var fixedObj2 = (ConstructedConfigurationObject) fixer.FixObject(obj2);
//            var fixedObj1 = (ConstructedConfigurationObject) fixer.FixObject(obj1);

//            ConfigurationObject Object(int index, ConstructedConfigurationObject constructedObj)
//            {
//                return ((ObjectConstructionArgument) constructedObj.PassedArguments[index]).PassedObject;
//            }

//            Assert.AreSame(fixedObj2, Object(0, fixedObj3));
//            Assert.AreSame(fixedObj1, Object(1, fixedObj3));
//            Assert.AreSame(_objectsContainer[2], Object(0, fixedObj2));
//            Assert.AreSame(fixedObj1, Object(1, fixedObj2));
//            Assert.AreSame(_objectsContainer[2], Object(0, fixedObj1));
//            Assert.AreSame(_objectsContainer[4], Object(1, fixedObj1));
//        }
//    }
//}