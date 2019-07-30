using FluentAssertions;
using GeoGen.Utilities;
using NUnit.Framework;
using System;
using System.Linq;

namespace GeoGen.Core.Tests
{
    /// <summary>
    /// The test class for <see cref="LooseObjectsLayoutExtentions"/>.
    /// </summary>
    [TestFixture]
    public class LooseObjectsLayoutExtentionsTest
    {
        [Test]
        public void Test_That_All_Layouts_Have_Defined_Types()
        {
            // Take all layouts
            Enum.GetValues(typeof(LooseObjectsLayout)).Cast<LooseObjectsLayout>()
                // That are distinct from NoLayout
                .Where(layout => layout != LooseObjectsLayout.NoLayout)
                // For each...
                .ForEach(layout =>
                {
                    // Act
                    Action act = () => layout.ObjectTypes();

                    // Assert there's no error
                    act.Should().NotThrow<GeoGenException>();
                });
        }
    }
}
