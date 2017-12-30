using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions;
using NUnit.Framework;
using static GeoGen.Generator.Test.TestHelpers.Constructions;

namespace GeoGen.Generator.Test.ConstructingObjects
{
    [TestFixture]
    public class ConstructionsContainerTest
    {
        private static ConstructionsContainer Container()
        {
           // return new ConstructionsContainer();
            return null;
        }

        private static IEnumerable<Construction> CorrectConstructions()
        {
            return new List<ConstructionWrapper>
            {
                Midpoint(),
                Intersection(),
                Projection(),
                CircleCenter(),
                CircumCircle(),
                CrazyConstruction()
            }.Select(wrapper => wrapper.Construction);
        }

        [Test]
        public void Test_Constructions_Cant_Be_Null()
        {
            //Assert.Throws<ArgumentNullException>(() => Container().Initialize(null));
        }

        [Test]
        public void Test_Wrappers_Constructions_Are_Set_Correctly()
        {
            var container = Container();
            //container.Initialize(CorrectConstructions());

            Assert.AreEqual(6, container.Count());
            Assert.IsTrue(container.All(c => c.Construction != null));
        }

        [Test]
        public void Test_Wrappers_Dictionaries_Are_Set_Correctly()
        {
            var result = new List<List<int>>
            {
                new List<int> {2, 0, 0},
                new List<int> {4, 0, 0},
                new List<int> {3, 0, 0},
                new List<int> {0, 0, 1},
                new List<int> {3, 0, 0},
                new List<int> {1, 3, 3}
            };

            var container = Container();
            //container.Initialize(CorrectConstructions());

            var dictionaries = container.Select(w => w.ObjectTypesToNeededCount).ToList();

            int Value(ConfigurationObjectType type, int index)
            {
                var dictionary = dictionaries[index];

                return !dictionary.ContainsKey(type) ? 0 : dictionary[type];
            }

            for (var i = 0; i < dictionaries.Count; i++)
            {
                Assert.AreEqual(Value(ConfigurationObjectType.Point, i), result[i][0]);
                Assert.AreEqual(Value(ConfigurationObjectType.Line, i), result[i][1]);
                Assert.AreEqual(Value(ConfigurationObjectType.Circle, i), result[i][2]);
            }
        }

        [Test]
        public void Test_Iterating_Over_Not_Initialized_Container()
        {
            var container = Container();

            Assert.Throws<GeneratorException>(() => container.FirstOrDefault());
        }

        [Test]
        public void Test_Iterating_Over_Incorrectly_Reinitialized_Container()
        {
            var container = Container();

            //container.Initialize(CorrectConstructions());

            try
            {
                //container.Initialize(null);
            }
            catch (Exception)
            {
                // ignored
            }

            Assert.Throws<GeneratorException>(() => container.FirstOrDefault());
        }

        [Test]
        public void Test_Correct_Reinitialization()
        {
            var container = Container();
            var constructions = CorrectConstructions().ToList();
            //container.Initialize(constructions);

            Assert.AreEqual(6, container.Count());
            constructions.RemoveRange(1, 2);
            Assert.AreEqual(6, container.Count());

            //container.Initialize(constructions);
            Assert.AreEqual(4, container.Count());
        }
    }
}