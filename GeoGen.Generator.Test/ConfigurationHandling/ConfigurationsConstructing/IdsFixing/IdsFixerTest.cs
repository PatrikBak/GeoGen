using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString;
using GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString.ObjectIdResolving;
using GeoGen.Generator.ConfigurationHandling.ConfigurationsConstructing.IdsFixing;
using GeoGen.Generator.ConfigurationHandling.ObjectsContainer;
using GeoGen.Generator.Constructing.Arguments.ArgumentsToString;
using GeoGen.Generator.Test.TestHelpers;
using NUnit.Framework;
using static GeoGen.Generator.Test.TestHelpers.ConfigurationObjects;

namespace GeoGen.Generator.Test.ConfigurationHandling.ConfigurationsConstructing.IdsFixing
{
    [TestFixture]
    public class IdsFixerTest
    {
        private static IConfigurationObjectsContainer _container;

        private static IdsFixer Fixer()
        {
            var defaultToString = new DefaultConfigurationObjectToStringProvider();
            var argsProvider = new ArgumentsToStringProvider(defaultToString);
            var defaultResolver = new DefaultObjectIdResolver();
            var provider = new DefaultComplexConfigurationObjectToStringProvider(argsProvider, defaultResolver);

            _container = new ConfigurationObjectsContainer(provider);
            var objects = Objects(42, ConfigurationObjectType.Point, includeIds: false).ToList();
            _container.Initialize(objects);

            return new IdsFixer(_container);
        }

        private static DictionaryObjectIdResolver Resolver()
        {
            var dictionary = Enumerable
                    .Range(0, 42)
                    .ToDictionary(i => i, i => i + 1);

            return new DictionaryObjectIdResolver(dictionary);
        }

        [Test]
        public void Container_Is_Not_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new IdsFixer(null));
        }

        [Test]
        public void Argument_Is_Not_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Fixer().FixArgument(null, Resolver()));
        }

        [Test]
        public void Object_Is_Not_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Fixer().FixObject(null, Resolver()));
        }

        [Test]
        public void Resolver_Is_Not_Null()
        {
            var argument = Utilities.SimpleMock<ConstructionArgument>();
            Assert.Throws<ArgumentNullException>(() => Fixer().FixArgument(argument, null));
        }

        [Test]
        public void Loose_Object_Fix()
        {
            var fixer = Fixer();

            var obj = new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 7};
            var fixedObj = fixer.FixObject(obj, Resolver());

            Assert.AreEqual(8, fixedObj.Id);
            Assert.AreSame(_container[8], fixedObj);
        }

        [Test]
        public void Simple_Object_Argument_Fix()
        {
            var fixer = Fixer();

            var obj = _container[7];
            var arg = new ObjectConstructionArgument(obj);
            var fixedArg = fixer.FixArgument(arg, Resolver());

            Assert.IsInstanceOf<ObjectConstructionArgument>(fixedArg);

            var passedObject = ((ObjectConstructionArgument) fixedArg).PassedObject;

            Assert.AreEqual(8, passedObject.Id);
            Assert.AreSame(_container[8], passedObject);
        }

        [Test]
        public void Simple_Nested_Set_Argument_Fix()
        {
            var fixer = Fixer();

            var arg = new SetConstructionArgument
            (
                new HashSet<ConstructionArgument>
                {
                    new SetConstructionArgument
                    (
                        new HashSet<ConstructionArgument>
                        {
                            new ObjectConstructionArgument(_container[1]),
                            new ObjectConstructionArgument(_container[2]),
                        }
                    ),
                    new SetConstructionArgument
                    (
                        new HashSet<ConstructionArgument>
                        {
                            new ObjectConstructionArgument(_container[5]),
                            new ObjectConstructionArgument(_container[1]),
                        }
                    )
                }
            );

            var fixedArg = fixer.FixArgument(arg, Resolver());
            Assert.IsInstanceOf<SetConstructionArgument>(fixedArg);

            var passedArgs = ((SetConstructionArgument) fixedArg).PassedArguments;

            Assert.AreEqual(2, passedArgs.Count);

            var random = passedArgs.First();
            Assert.IsInstanceOf<SetConstructionArgument>(random);

            var castedRandom = (SetConstructionArgument) random;
            var containsSecond = castedRandom.PassedArguments
                    .Any(o => ((ObjectConstructionArgument) o).PassedObject == _container[2]);

            Assert.IsTrue(containsSecond);
        }

        [Test]
        public void Simple_Constructed_Object_Fix()
        {
            var fixer = Fixer();

            var args = new List<ConstructionArgument>
            {
                new SetConstructionArgument
                (
                    new HashSet<ConstructionArgument>
                    {
                        new ObjectConstructionArgument(_container[7]),
                        new ObjectConstructionArgument(_container[8]),
                    }
                )
            };

            var constructedObject = ConstructedObject(42, 0, args);
            _container.Add(constructedObject);
            var fixedObject = fixer.FixObject(constructedObject, Resolver());
            var casted = (ConstructedConfigurationObject) fixedObject;

            Assert.AreEqual(44, fixedObject.Id ?? throw new Exception());
            Assert.AreEqual(constructedObject.Construction, casted.Construction);
            Assert.AreEqual(constructedObject.ObjectType, fixedObject.ObjectType);
            Assert.AreEqual(constructedObject.Index, casted.Index);

            Assert.AreSame(_container[44], fixedObject);

            var newArgs = casted.PassedArguments;
            Assert.AreEqual(1, newArgs.Count);

            var firstArg = newArgs[0];
            Assert.IsInstanceOf<SetConstructionArgument>(firstArg);

            var castedArg = (SetConstructionArgument) firstArg;

            var passedArgs = castedArg.PassedArguments;
            Assert.AreEqual(2, passedArgs.Count);

            bool AnyWithId(int id)
            {
                return passedArgs.Any
                (
                    arg =>
                    {
                        var objectArg = (ObjectConstructionArgument) arg;
                        var passedObject = objectArg.PassedObject;

                        return passedObject == _container[id] && passedObject.Id == id;
                    }
                );
            }

            Assert.IsTrue(AnyWithId(8));
            Assert.IsTrue(AnyWithId(9));
        }

        [Test]
        public void Complex_Nested_Arguments_Fix()
        {
            var fixer = Fixer();

            var args1 = new List<ConstructionArgument>
            {
                new ObjectConstructionArgument(_container[1]),
                new ObjectConstructionArgument(_container[3])
            };
            var obj1 = ConstructedObject(42, 1, args1);
            _container.Add(obj1);

            var args2 = new List<ConstructionArgument>
            {
                new ObjectConstructionArgument(_container[1]),
                new ObjectConstructionArgument(obj1)
            };
            var obj2 = ConstructedObject(42, 1, args2);
            _container.Add(obj2);

            var args3 = new List<ConstructionArgument>
            {
                new ObjectConstructionArgument(obj2),
                new ObjectConstructionArgument(obj1)
            };
            var obj3 = ConstructedObject(42, 1, args3);
            _container.Add(obj3);

            var fixedObj3 = (ConstructedConfigurationObject) fixer.FixObject(obj3, Resolver());
            var fixedObj2 = (ConstructedConfigurationObject) fixer.FixObject(obj2, Resolver());
            var fixedObj1 = (ConstructedConfigurationObject) fixer.FixObject(obj1, Resolver());

            ConfigurationObject Object(int index, ConstructedConfigurationObject constructedObj)
            {
                return ((ObjectConstructionArgument) constructedObj.PassedArguments[index]).PassedObject;
            }

            Assert.AreSame(fixedObj2, Object(0, fixedObj3));
            Assert.AreSame(fixedObj1, Object(1, fixedObj3));
            Assert.AreSame(_container[2], Object(0, fixedObj2));
            Assert.AreSame(fixedObj1, Object(1, fixedObj2));
            Assert.AreSame(_container[2], Object(0, fixedObj1));
            Assert.AreSame(_container[4], Object(1, fixedObj1));
        }
    }
}