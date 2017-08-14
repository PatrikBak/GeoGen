using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Generator.Constructor.Arguments.Container;
using Moq;
using NUnit.Framework;

namespace GeoGen.Generator.Test.Constructor.Arguments.Container
{
    [TestFixture]
    public class ConstructionArgumentEqualityComparerTest
    {
        private static int _lastConfigurationObjectId = 1;

        private static ObjectConstructionArgument NextArgument()
        {
            var cObject = new LooseConfigurationObject(ConfigurationObjectType.Point)
            {
                Id = _lastConfigurationObjectId++
            };

            return new ObjectConstructionArgument(cObject);
        }

        private static ConstructionArgumentEqualityComparer TestComparer()
        {
            var mock = new Mock<IEqualityComparer<ConfigurationObject>>();

            mock.Setup(comparer => comparer.GetHashCode(It.IsAny<ConfigurationObject>())).Returns(1);

            mock.Setup(comparer => comparer.Equals(It.IsAny<ConfigurationObject>(), It.IsAny<ConfigurationObject>()))
                .Returns<ConfigurationObject, ConfigurationObject>((o1, o2) => o1.Id == o2.Id);

            return new ConstructionArgumentEqualityComparer(mock.Object);
        }

        [Test]
        public void Test_Two_Same_Object_Arguments()
        {
            var comparer = TestComparer();
            var arg = NextArgument();
            Assert.True(comparer.Equals(arg, arg));
        }

        [Test]
        public void Test_Two_Distinct_Object_Arguments()
        {
            var comparer = TestComparer();
            var arg1 = NextArgument();
            var arg2 = NextArgument();
            Assert.False(comparer.Equals(arg1, arg2));
        }

        [Test]
        public void Test_Two_Same_Set_Arguments()
        {
            var comparer = TestComparer();
            var arg1 = NextArgument();
            var arg2 = NextArgument();
            var set1 = new SetConstructionArgument(new HashSet<ConstructionArgument> {arg1, arg2});
            var set2 = new SetConstructionArgument(new HashSet<ConstructionArgument> {arg1, arg2});
            Assert.True(comparer.Equals(set1, set2));
        }

        [Test]
        public void Test_Two_Distint_Set_Arguments1()
        {
            var comparer = TestComparer();
            var arg1 = NextArgument();
            var arg2 = NextArgument();
            var arg3 = NextArgument();
            var set1 = new SetConstructionArgument(new HashSet<ConstructionArgument> {arg1, arg2, arg3});
            var set2 = new SetConstructionArgument(new HashSet<ConstructionArgument> {arg1, arg2});
            Assert.False(comparer.Equals(set1, set2));
        }

        [Test]
        public void Test_Two_Distint_Set_Arguments2()
        {
            var comparer = TestComparer();
            var arg1 = NextArgument();
            var arg2 = NextArgument();
            var arg3 = NextArgument();
            var set1 = new SetConstructionArgument(new HashSet<ConstructionArgument> {arg1, arg2});
            var set2 = new SetConstructionArgument(new HashSet<ConstructionArgument> {arg1, arg3});
            Assert.False(comparer.Equals(set1, set2));
        }

        [Test]
        public void Test_Two_Same_Composed_Arguments()
        {
            var comparer = TestComparer();
            var arg1 = NextArgument();
            var arg2 = NextArgument();
            var arg3 = NextArgument();
            var arg4 = NextArgument();
            var arg5 = NextArgument();

            var innerSet1 = new SetConstructionArgument(new HashSet<ConstructionArgument> {arg1, arg2});
            var innerSet2 = new SetConstructionArgument(new HashSet<ConstructionArgument> {arg3, arg4});
            var innerSet3 = new SetConstructionArgument(new HashSet<ConstructionArgument> {arg2, arg5});

            var middleSet1 = new SetConstructionArgument(new HashSet<ConstructionArgument> {innerSet1, innerSet2});
            var middleSet2 = new SetConstructionArgument(new HashSet<ConstructionArgument> {innerSet2, innerSet3});

            var final1 = new SetConstructionArgument(new HashSet<ConstructionArgument> {middleSet1, middleSet2});

            innerSet1 = new SetConstructionArgument(new HashSet<ConstructionArgument> { arg2, arg1 });
            innerSet2 = new SetConstructionArgument(new HashSet<ConstructionArgument> { arg4, arg3 });
            innerSet3 = new SetConstructionArgument(new HashSet<ConstructionArgument> { arg5, arg2 });

            middleSet1 = new SetConstructionArgument(new HashSet<ConstructionArgument> { innerSet3, innerSet2 });
            middleSet2 = new SetConstructionArgument(new HashSet<ConstructionArgument> { innerSet2, innerSet1 });

            var final2 = new SetConstructionArgument(new HashSet<ConstructionArgument> { middleSet1, middleSet2 });

            Assert.IsTrue(comparer.Equals(final1, final2));
        }

        [Test]
        public void Test_Two_Distint_Composed_Arguments()
        {
            var comparer = TestComparer();
            var arg1 = NextArgument();
            var arg2 = NextArgument();
            var arg3 = NextArgument();
            var arg4 = NextArgument();
            var arg5 = NextArgument();

            var innerSet1 = new SetConstructionArgument(new HashSet<ConstructionArgument> { arg1, arg2 });
            var innerSet2 = new SetConstructionArgument(new HashSet<ConstructionArgument> { arg3, arg4 });
            var innerSet3 = new SetConstructionArgument(new HashSet<ConstructionArgument> { arg2, arg5 });

            var middleSet1 = new SetConstructionArgument(new HashSet<ConstructionArgument> { innerSet1, innerSet2 });
            var middleSet2 = new SetConstructionArgument(new HashSet<ConstructionArgument> { innerSet2, innerSet3 });

            var final1 = new SetConstructionArgument(new HashSet<ConstructionArgument> { middleSet1, middleSet2 });

            innerSet1 = new SetConstructionArgument(new HashSet<ConstructionArgument> { arg2, arg1 });
            innerSet2 = new SetConstructionArgument(new HashSet<ConstructionArgument> { arg4, arg3 });
            innerSet3 = new SetConstructionArgument(new HashSet<ConstructionArgument> { arg5, arg4 });

            middleSet1 = new SetConstructionArgument(new HashSet<ConstructionArgument> { innerSet3, innerSet2 });
            middleSet2 = new SetConstructionArgument(new HashSet<ConstructionArgument> { innerSet2, innerSet1 });

            var final2 = new SetConstructionArgument(new HashSet<ConstructionArgument> { middleSet1, middleSet2 });

            Assert.IsFalse(comparer.Equals(final1, final2));
        }
    }
}