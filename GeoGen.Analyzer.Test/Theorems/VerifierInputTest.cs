using System;
using System.Linq;
using GeoGen.Analyzer.Objects.GeometricalObjects;
using GeoGen.Analyzer.Objects.GeometricalObjects.Container;
using GeoGen.Analyzer.Test.TestHelpers;
using GeoGen.Analyzer.Theorems;
using GeoGen.Core.Configurations;
using GeoGen.Core.Utilities;
using GeoGen.Utilities;
using Moq;
using NUnit.Framework;
using static GeoGen.Analyzer.Test.TestHelpers.Utilities;

namespace GeoGen.Analyzer.Test.Theorems
{
    [TestFixture]
    public class VerifierInputTest
    {
        private class Info
        {
            public int LineCalls;
            public int PointCalls;
            public int CircleCalls;
            public ConfigurationObjectsMap MapForGetObjects;
            public ConfigurationObjectsMap SecondMapForGetNewObjects;
        }

        private static IContextualContainer Container(Info info)
        {
            var container = new Mock<IContextualContainer>();

            container.Setup(s => s.GetObjects<LineObject>(It.IsAny<ConfigurationObjectsMap>()))
                    .Returns<ConfigurationObjectsMap>
                    (
                        map =>
                        {
                            if (map == info.MapForGetObjects)
                                info.LineCalls++;

                            return Enumerable.Empty<LineObject>();
                        }
                    );

            container.Setup(s => s.GetObjects<PointObject>(It.IsAny<ConfigurationObjectsMap>()))
                    .Returns<ConfigurationObjectsMap>
                    (
                        map =>
                        {
                            if (map == info.MapForGetObjects)
                                info.PointCalls++;

                            return Enumerable.Empty<PointObject>();
                        }
                    );

            container.Setup(s => s.GetObjects<CircleObject>(It.IsAny<ConfigurationObjectsMap>()))
                    .Returns<ConfigurationObjectsMap>
                    (
                        map =>
                        {
                            if (map == info.MapForGetObjects)
                                info.CircleCalls++;

                            return Enumerable.Empty<CircleObject>();
                        }
                    );

            container.Setup(s => s.GetNewObjects<PointObject>(It.IsAny<ConfigurationObjectsMap>(), It.IsAny<ConfigurationObjectsMap>()))
                    .Returns<ConfigurationObjectsMap, ConfigurationObjectsMap>
                    (
                        (oldMap, newMap) =>
                        {
                            if (oldMap == info.MapForGetObjects && newMap == info.SecondMapForGetNewObjects)
                                info.PointCalls++;

                            return Enumerable.Empty<PointObject>();
                        }
                    );

            container.Setup(s => s.GetNewObjects<LineObject>(It.IsAny<ConfigurationObjectsMap>(), It.IsAny<ConfigurationObjectsMap>()))
                    .Returns<ConfigurationObjectsMap, ConfigurationObjectsMap>
                    (
                        (oldMap, newMap) =>
                        {
                            if (oldMap == info.MapForGetObjects && newMap == info.SecondMapForGetNewObjects)
                                info.LineCalls++;

                            return Enumerable.Empty<LineObject>();
                        }
                    );

            container.Setup(s => s.GetNewObjects<CircleObject>(It.IsAny<ConfigurationObjectsMap>(), It.IsAny<ConfigurationObjectsMap>()))
                    .Returns<ConfigurationObjectsMap, ConfigurationObjectsMap>
                    (
                        (oldMap, newMap) =>
                        {
                            if (oldMap == info.MapForGetObjects && newMap == info.SecondMapForGetNewObjects)
                                info.CircleCalls++;

                            return Enumerable.Empty<CircleObject>();
                        }
                    );


            return container.Object;
        }

        [Test]
        public void Test_Container_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new VerifierInput(null, new ConfigurationObjectsMap(), new ConfigurationObjectsMap()));
        }

        [Test]
        public void Test_Old_Objects_Cant_Be_Null()
        {
            var container = SimpleMock<IContextualContainer>();

            Assert.Throws<ArgumentNullException>(() => new VerifierInput(container, null, new ConfigurationObjectsMap()));
        }

        [Test]
        public void Test_New_Objects_Cant_Be_Null()
        {
            var container = SimpleMock<IContextualContainer>();

            Assert.Throws<ArgumentNullException>(() => new VerifierInput(container, new ConfigurationObjectsMap(), null));
        }

        [Test]
        public void Test_All_Objects_Are_Set_Correctly()
        {
            var map1 = new ConfigurationObjectsMap();
            map1.AddAll(ConfigurationObjects.Objects(4, ConfigurationObjectType.Point));
            map1.AddAll(ConfigurationObjects.Objects(4, ConfigurationObjectType.Line));
            map1.AddAll(ConfigurationObjects.Objects(4, ConfigurationObjectType.Circle));

            var map2 = new ConfigurationObjectsMap();
            map2.AddAll(ConfigurationObjects.Objects(2, ConfigurationObjectType.Point));
            map2.AddAll(ConfigurationObjects.Objects(3, ConfigurationObjectType.Line));

            var container = SimpleMock<IContextualContainer>();
            var input = new VerifierInput(container, map1, map2);
            var allObjects = input.AllObjects;

            Assert.IsTrue(allObjects.AllObjects().ToSet().SetEquals(map1.Merge(map2).AllObjects().ToSet()));
        }

        [Test]
        public void Test_All_Getter_Lazily_Call_Container()
        {
            var oldObjects = new ConfigurationObjectsMap();
            var newObjects = new ConfigurationObjectsMap();
            var info = new Info();
            var container = Container(info);
            var input = new VerifierInput(container, oldObjects, newObjects);
            info.MapForGetObjects = input.AllObjects;

            Assert.AreEqual(0, info.LineCalls);
            Assert.AreEqual(0, info.PointCalls);
            Assert.AreEqual(0, info.CircleCalls);

            var x1 = input.AllCircles;
            var x2 = input.AllLines;
            var x3 = input.AllPoints;

            Assert.AreEqual(1, info.LineCalls);
            Assert.AreEqual(1, info.PointCalls);
            Assert.AreEqual(1, info.CircleCalls);

            var x4 = input.AllCircles;
            var x5 = input.AllLines;
            var x6 = input.AllPoints;

            Assert.AreEqual(1, info.LineCalls);
            Assert.AreEqual(1, info.PointCalls);
            Assert.AreEqual(1, info.CircleCalls);
        }

        [Test]
        public void Test_New_Getters_Lazily_Call_Container()
        {
            var oldObjects = new ConfigurationObjectsMap();
            var newObjects = new ConfigurationObjectsMap();
            var info = new Info();
            var container = Container(info);
            var input = new VerifierInput(container, oldObjects, newObjects);
            info.MapForGetObjects = oldObjects;
            info.SecondMapForGetNewObjects = newObjects;

            Assert.AreEqual(0, info.LineCalls);
            Assert.AreEqual(0, info.PointCalls);
            Assert.AreEqual(0, info.CircleCalls);

            var x1 = input.NewCircles;
            var x2 = input.NewLines;
            var x3 = input.NewPoints;

            Assert.AreEqual(1, info.LineCalls);
            Assert.AreEqual(1, info.PointCalls);
            Assert.AreEqual(1, info.CircleCalls);

            var x4 = input.NewCircles;
            var x5 = input.NewLines;
            var x6 = input.NewPoints;

            Assert.AreEqual(1, info.LineCalls);
            Assert.AreEqual(1, info.PointCalls);
            Assert.AreEqual(1, info.CircleCalls);
        }

        [Test]
        public void Test_Old_Getters_Lazily_Call_Container()
        {
            var oldObjects = new ConfigurationObjectsMap();
            var newObjects = new ConfigurationObjectsMap();
            var info = new Info();
            var container = Container(info);
            var input = new VerifierInput(container, oldObjects, newObjects);
            info.MapForGetObjects = oldObjects;

            Assert.AreEqual(0, info.LineCalls);
            Assert.AreEqual(0, info.PointCalls);
            Assert.AreEqual(0, info.CircleCalls);

            var x1 = input.OldCircles;
            var x2 = input.OldLines;
            var x3 = input.OldPoints;

            Assert.AreEqual(1, info.LineCalls);
            Assert.AreEqual(1, info.PointCalls);
            Assert.AreEqual(1, info.CircleCalls);

            var x4 = input.OldCircles;
            var x5 = input.OldLines;
            var x6 = input.OldPoints;

            Assert.AreEqual(1, info.LineCalls);
            Assert.AreEqual(1, info.PointCalls);
            Assert.AreEqual(1, info.CircleCalls);
        }
    }
}