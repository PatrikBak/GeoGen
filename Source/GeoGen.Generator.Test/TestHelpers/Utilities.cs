using Moq;

namespace GeoGen.Generator.Test.TestHelpers
{
    internal static class Utilities
    {
        public static T SimpleMock<T>() where T : class
        {
            return new Mock<T>().Object;
        }
    }
}
