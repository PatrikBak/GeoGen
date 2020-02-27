using FluentAssertions;
using GeoGen.Utilities;
using NUnit.Framework;
using System;
using System.Linq;

namespace GeoGen.Core.Tests
{
    /// <summary>
    /// The test class for <see cref="Constructions"/>
    /// </summary>
    [TestFixture]
    public class ConstructionsTest
    {
        [Test]
        public void Test_That_All_Predefined_Constructions_Are_Accessible()
        {
            // Get all predefined types
            Enum.GetValues(typeof(PredefinedConstructionType))
                // Cast them to the right type
                .Cast<PredefinedConstructionType>()
                // Check each
                .ForEach(type =>
                {
                    // Get the construction
                    var construction = Constructions.GetPredefinedconstruction(type);

                    // Assert
                    construction.Should().NotBeNull();
                    construction.Type.Should().Be(type);
                });
        }
    }
}
