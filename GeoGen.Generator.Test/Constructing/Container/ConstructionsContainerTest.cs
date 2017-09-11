﻿using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions;
using GeoGen.Generator.Constructing.Container;
using GeoGen.Generator.Test.Constructing.Arguments;
using Moq;
using NUnit.Framework;

namespace GeoGen.Generator.Test.Constructing.Container
{
    [TestFixture]
    public class ConstructionsContainerTest
    {
        private static ConstructionsContainer Container()
        {
            return new ConstructionsContainer();
        }

        private static List<Construction> ConstructionsWithId(int[] ids)
        {
            return ids.Select(
                i =>
                {
                    var mock = new Mock<Construction>();
                    mock.Setup(c => c.Id).Returns(i);

                    return mock.Object;
                }).ToList();
        }

        [Test]
        public void Constructions_Not_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Container().Initialize(null));
        }

        [Test]
        public void Constructions_Dont_Have_Distint_Ids()
        {
            int[][] constructionIds =
            {
                new[] {1, 1, 2, 3, 4},
                new[] {1, 1, 6},
                new[] {7, 4, 3, 2, 1, 2}
            };

            foreach (var ids in constructionIds)
            {
                var constructions = ConstructionsWithId(ids);
                Assert.Throws<ArgumentException>(() => Container().Initialize(constructions));
            }
        }

        private static IEnumerable<Construction> CorrectConstructions()
        {
            return new List<Construction>
            {
                ArgumentsGeneratorTest.Midpoint().Construction,
                ArgumentsGeneratorTest.Intersection().Construction,
                ArgumentsGeneratorTest.Projection().Construction,
                ArgumentsGeneratorTest.CircleCenter().Construction,
                ArgumentsGeneratorTest.CircumCircle().Construction,
                ArgumentsGeneratorTest.CrazyConstruction().Construction,
            };
        }

        [Test]
        public void Wrappers_Constructions_Are_Set_Correctly()
        {
            var container = Container();
            container.Initialize(CorrectConstructions());

            Assert.AreEqual(6, container.Count());
            Assert.IsTrue(container.All(c => c.Construction != null));
        }

        [Test]
        public void Wrappers_Dictionaries_Are_Set_Correctly()
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
            container.Initialize(CorrectConstructions());

            var dictionaries = container.Select(w => w.ObjectTypesToNeededCount).ToList();

            int Value(ConfigurationObjectType type, int index)
            {
                var dictionary = dictionaries[index];

                return !dictionary.ContainsKey(type) ? 0 : dictionary[type];
            }

            for (var i = 0; i < dictionaries.Count; i++)
            {
                Assert.AreEqual(Value(ConfigurationObjectType.Point,i), result[i][0]);
                Assert.AreEqual(Value(ConfigurationObjectType.Line, i), result[i][1]);
                Assert.AreEqual(Value(ConfigurationObjectType.Circle, i), result[i][2]);
            }
        }
    }
}