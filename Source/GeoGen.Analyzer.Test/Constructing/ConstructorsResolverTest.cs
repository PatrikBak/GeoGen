//using System;
//using GeoGen.Core.Constructions;
//using GeoGen.Core.Constructions.PredefinedConstructions;
//using NUnit.Framework;
//using static GeoGen.Analyzer.Test.TestHelpers.Utilities;

//namespace GeoGen.Analyzer.Test.Constructing
//{
//    [TestFixture]
//    public class ConstructorsResolverTest
//    {
//        private static ConstructorsResolver ConstructorsResolver()
//        {
//            return new ConstructorsResolver
//            (
//                new IPredefinedConstructor[]
//                {
//                    new MidpointConstructor(),
//                    new InteresectionConstructor()
//                },
//                null
//            );
//        }

//        [Test]
//        public void Test_Predefined_Constructors_Cant_Be_Null()
//        {
//            Assert.Throws<ArgumentNullException>(() => new ConstructorsResolver(null, null));
//        }

//        [Test]
//        public void Test_Predefined_Constructors_Cant_Contain_Null()
//        {
//            var resolvers = new IPredefinedConstructor[]
//            {
//                new MidpointConstructor(),
//                null,
//                new InteresectionConstructor()
//            };

//            Assert.Throws<ArgumentException>(() => new ConstructorsResolver(resolvers));
//        }

//        [Test]
//        public void Test_Predefined_Constructors_Cant_Contain_Duplicatate_Types()
//        {
//            var resolvers = new IPredefinedConstructor[]
//            {
//                new MidpointConstructor(),
//                new InteresectionConstructor(),
//                new InteresectionConstructor()
//            };

//            Assert.Throws<ArgumentException>(() => new ConstructorsResolver(resolvers));
//        }

//        [Test]
//        public void Test_Resolve_Construction_Cant_Be_Null()
//        {
//            Assert.Throws<ArgumentNullException>(() => ConstructorsResolver().Resolve(null));
//        }

//        [Test]
//        public void Test_Resolve_Unregisterd_Predefined_Construction()
//        {
//            Assert.Throws<AnalyzerException>(() => ConstructorsResolver().Resolve(SimpleMock<PredefinedConstruction>()));
//        }

//        [Test]
//        public void Test_Resolve_Construction_With_Predefined_Construction()
//        {
//            var midpoint = new Midpoint();
//            var intersection = new Intersection();

//            var constructor1 = ConstructorsResolver().Resolve(midpoint);
//            Assert.AreEqual(typeof(MidpointConstructor), constructor1.GetType());

//            var constructor2 = ConstructorsResolver().Resolve(intersection);
//            Assert.AreEqual(typeof(InteresectionConstructor), constructor2.GetType());
//        }
//    }
//}