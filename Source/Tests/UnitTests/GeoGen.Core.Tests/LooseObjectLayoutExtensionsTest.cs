using FluentAssertions;
using GeoGen.Utilities;
using NUnit.Framework;

namespace GeoGen.Core.Tests
{
    /// <summary>
    /// The test class for <see cref="LooseObjectLayoutExtensions"/>.
    /// </summary>
    [TestFixture]
    public class LooseObjectLayoutExtensionsTest
    {
        [Test]
        public void Test_That_All_Layouts_Have_Defined_Types()
        {
            // Take all layouts
            Enum.GetValues(typeof(LooseObjectLayout)).Cast<LooseObjectLayout>()
                // For each...
                .ForEach(layout =>
                {
                    // Action of getting types
                    Action getTypes = () => layout.ObjectTypes();

                    // Assert there's no error
                    getTypes.Should().NotThrow<GeoGenException>();
                });
        }
    }
}
