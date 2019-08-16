using FluentAssertions;
using GeoGen.Core;
using GeoGen.Utilities;
using NUnit.Framework;
using System.Linq;
using static GeoGen.Core.ComposedConstructions;
using static GeoGen.Core.ConfigurationObjectType;
using static GeoGen.Core.LooseObjectsLayout;
using static GeoGen.Core.PredefinedConstructionType;
using static GeoGen.Core.TheoremType;

namespace GeoGen.TheoremsAnalyzer.Test
{
    /// <summary>
    /// The test class for <see cref="TransitivityDeriver"/>.
    /// </summary>
    [TestFixture]
    public class TransitivityDeriverTest
    {
        #region TransitivityDeriver instance

        /// <summary>
        /// The instance of the deriver.
        /// </summary>
        private TransitivityDeriver Deriver => new TransitivityDeriver();

        #endregion

        [Test]
        public void Test_With_Three_Equal_Angles_In_Midpoint_Configuration()
        {
            // Create objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Midpoint, A, B);
            var E = new ConstructedConfigurationObject(Midpoint, A, C);
            var F = new ConstructedConfigurationObject(Midpoint, B, C);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(ThreePoints, D, E, F);

            // Create all angles 
            var angles = new[]
            {
                new AngleTheoremObject(new LineTheoremObject(B, D, A), new LineTheoremObject(D, F)),
                new AngleTheoremObject(new LineTheoremObject(D, F), new LineTheoremObject(F, E)),
                new AngleTheoremObject(new LineTheoremObject(F, E), new LineTheoremObject(A, C, E))
            };

            // Create all theorems about their equalities
            var theorems = angles.Subsets(2).Select(subset => new Theorem(configuration, EqualAngles, subset.ToArray())).ToList();

            // Any time we take two theorems, we can deduce the third
            theorems.Subsets(2).ForEach(assumedTheorems =>
            {
                // Call the deriver
                var result = Deriver.Derive(configuration, theorems, assumedTheorems).ToList();

                // There should be one derived theorems
                result.Count.Should().Be(1);

                // Deconstruct
                var (fact1, fact2, conclusion) = result.First();

                // Our facts should be assumed theorems
                assumedTheorems.ToSet(Theorem.EquivalencyComparer).SetEquals(new[] { fact1, fact2 }.ToSet()).Should().BeTrue();

                // Our conclusion should be the other theorem
                theorems.ToSet(Theorem.EquivalencyComparer).Except(assumedTheorems).Single().IsEquivalentTo(conclusion).Should().BeTrue();
            });
        }

        [Test]
        public void Test_With_Two_Equal_Angles_And_One_Smaller_Theorem_In_Midpoint_Configuration()
        {
            // Create objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Midpoint, A, B);
            var E = new ConstructedConfigurationObject(Midpoint, A, C);
            var F = new ConstructedConfigurationObject(Midpoint, B, C);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(ThreePoints, D, E, F);

            // Create all angles 
            var angles = new[]
            {
                new AngleTheoremObject(new LineTheoremObject(A, E, C), new LineTheoremObject(D, E)),
                new AngleTheoremObject(new LineTheoremObject(A, E, C), new LineTheoremObject(B, F, C)),
                new AngleTheoremObject(new LineTheoremObject(F, D), new LineTheoremObject(D, E))
            };

            // Create theorems about equalities with the last angle
            var theorems = new[]
            {
                new Theorem(configuration, EqualAngles, new[] {angles[0], angles[2] }),
                new Theorem(configuration, EqualAngles, new[] {angles[1], angles[2] }),
            };

            // We should need just one of these theorem to deduce the other, 
            // cause the equality of angles 0 and 1 should be deduced and assumed
            // automatically because it can be defined in a smaller configuration
            theorems.ForEach(assumedTheorem =>
            {
                // Call the deriver
                var result = Deriver.Derive(configuration, theorems, new[] { assumedTheorem }).ToList();

                // There should be one derived theorems
                result.Count.Should().Be(1);

                // Deconstruct
                var (fact1, fact2, conclusion) = result.First();

                // Our assumed theorem should be assumed theorems
                new[] { fact1, fact2 }.Any(t => t.IsEquivalentTo(assumedTheorem)).Should().BeTrue();

                // Other should be stating that the first two angles are equal
                new[] { fact1, fact2 }.Any(t => t.IsEquivalentTo(new Theorem(configuration, EqualAngles, new[] { angles[0], angles[1] }))).Should().BeTrue();
            });
        }

        [Test]
        public void Test_With_Four_Equal_Angles_Without_Deducing_In_Midpoint_Configuration()
        {
            // Create objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Midpoint, A, B);
            var E = new ConstructedConfigurationObject(Midpoint, A, C);
            var F = new ConstructedConfigurationObject(Midpoint, B, C);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(ThreePoints, D, E, F);

            // Create all angles 
            var angles = new[]
            {
                new AngleTheoremObject(new LineTheoremObject(B, D, A), new LineTheoremObject(D, F)),
                new AngleTheoremObject(new LineTheoremObject(D, F), new LineTheoremObject(F, E)),
                new AngleTheoremObject(new LineTheoremObject(F, E), new LineTheoremObject(A, C, E)),
                new AngleTheoremObject(new LineTheoremObject(B, D, A), new LineTheoremObject(A, C, E))
            };

            // Create some theorems about their equalities 
            // that should be enough to see 0 = 1 = 2 = 3
            var theorems = new[]
            {
                new Theorem(configuration, EqualAngles, new[] {angles[0], angles[1] }),
                new Theorem(configuration, EqualAngles, new[] {angles[1], angles[2] }),
                new Theorem(configuration, EqualAngles, new[] {angles[2], angles[3] })
            };

            // Assume all these theorems are assumed
            var result = Deriver.Derive(configuration, theorems, theorems).ToList();

            // We should be able to generate every equality
            result.Select(triple => triple.Item3).ToSet(Theorem.EquivalencyComparer).SetEquals(new[]
            {
                new Theorem(configuration, EqualAngles, new[] {angles[0], angles[2] }),
                new Theorem(configuration, EqualAngles, new[] {angles[0], angles[3] }),
                new Theorem(configuration, EqualAngles, new[] {angles[1], angles[3] })
            })
            .Should().BeTrue();
        }

        [Test]
        public void Test_With_Four_Equal_Angles_With_Deducing_In_Midpoint_Configuration()
        {
            // Create objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Midpoint, A, B);
            var E = new ConstructedConfigurationObject(Midpoint, A, C);
            var F = new ConstructedConfigurationObject(Midpoint, B, C);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(ThreePoints, D, E, F);

            // Create all angles 
            var angles = new[]
            {
                new AngleTheoremObject(new LineTheoremObject(A, E, C), new LineTheoremObject(D, E)),
                new AngleTheoremObject(new LineTheoremObject(A, E, C), new LineTheoremObject(B, F, C)),
                new AngleTheoremObject(new LineTheoremObject(F, D), new LineTheoremObject(D, E)),
                new AngleTheoremObject(new LineTheoremObject(B, F, C), new LineTheoremObject(D, F))
            };

            // Create some theorems about their equalities 
            // that should be enough to see 0 = 1 = 2 = 3 
            var theorems = new[]
            {
                new Theorem(configuration, EqualAngles, new[] {angles[0], angles[1] }),
                new Theorem(configuration, EqualAngles, new[] {angles[1], angles[2] }),
                new Theorem(configuration, EqualAngles, new[] {angles[2], angles[3] })
            };

            // For the assumed theorems takes only the second two
            // The first one should be assumed, cause it's true in 
            // a smaller configuration
            var result = Deriver.Derive(configuration, theorems, theorems.Skip(1)).ToList();

            // We should be able to generate every equality
            result.Select(triple => triple.Item3).ToSet(Theorem.EquivalencyComparer).SetEquals(new[]
            {
                new Theorem(configuration, EqualAngles, new[] {angles[0], angles[2] }),
                new Theorem(configuration, EqualAngles, new[] {angles[0], angles[3] }),
                new Theorem(configuration, EqualAngles, new[] {angles[1], angles[3] })
            })
            .Should().BeTrue();
        }

        [Test]
        public void Test_With_Eight_Equal_Angles_Where_Four_Of_Them_Are_Old()
        {
            // Create objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Midpoint, A, B);
            var E = new ConstructedConfigurationObject(Midpoint, A, C);
            var F = new ConstructedConfigurationObject(Midpoint, B, C);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(ThreePoints, D, E, F);

            // Create the angles (the first four are old)
            var angles = new[]
            {
                new AngleTheoremObject(new LineTheoremObject(A, B, D), new LineTheoremObject(B, C, F)),
                new AngleTheoremObject(new LineTheoremObject(A, E, C), new LineTheoremObject(B, C, F)),
                new AngleTheoremObject(new LineTheoremObject(A, B, D), new LineTheoremObject(D, E)),
                new AngleTheoremObject(new LineTheoremObject(A, E, C), new LineTheoremObject(D, E)),
                new AngleTheoremObject(new LineTheoremObject(D, E), new LineTheoremObject(D, F)),
                new AngleTheoremObject(new LineTheoremObject(D, E), new LineTheoremObject(E, F)),
                new AngleTheoremObject(new LineTheoremObject(D, F), new LineTheoremObject(B, C, F)),
                new AngleTheoremObject(new LineTheoremObject(E, F), new LineTheoremObject(B, C, F))
            };

            // Create the theorems that should be enough to deduce all equalities
            var theorems = new[]
            {
                new Theorem(configuration, EqualAngles, new[] { angles[0], angles[1] }),
                new Theorem(configuration, EqualAngles, new[] { angles[2], angles[3] }),
                new Theorem(configuration, EqualAngles, new[] { angles[4], angles[5] }),
                new Theorem(configuration, EqualAngles, new[] { angles[6], angles[7] }),
                new Theorem(configuration, EqualAngles, new[] { angles[0], angles[3] }),
                new Theorem(configuration, EqualAngles, new[] { angles[5], angles[7] }),
                new Theorem(configuration, EqualAngles, new[] { angles[2], angles[4] })
            };

            // Create assumed theorems that should be needed to deduce all equalities with 0 = 1 = 2 = 3
            var assumedTheorems = new[]
            {
                new Theorem(configuration, EqualAngles, new[] { angles[4], angles[5] }),
                new Theorem(configuration, EqualAngles, new[] { angles[6], angles[7] }),
                new Theorem(configuration, EqualAngles, new[] { angles[1], angles[5] }),
                new Theorem(configuration, EqualAngles, new[] { angles[5], angles[6] }),
            };

            // Find the result
            var result = Deriver.Derive(configuration, theorems, assumedTheorems);

            // Prepare all tuples of equalities of new and old objects
            var allEqualities = new[] { angles.Take(4), angles.Skip(4) }.Combine()
                // Make each a theorem
                .Select(twoAngles => new Theorem(configuration, EqualAngles, twoAngles))
                // That have not been established
                .Where(theorem => !assumedTheorems.ToSet(Theorem.EquivalencyComparer).Contains(theorem));

            // Make sure the results derives everything
            result.Select(triple => triple.Item3).ToSet(Theorem.EquivalencyComparer).SetEquals(allEqualities);
        }

        [Test]
        public void Test_With_Four_Collinear_Points()
        {
            // Create objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new ConstructedConfigurationObject(Midpoint, A, B);
            var D = new ConstructedConfigurationObject(Midpoint, A, C);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(TwoPoints, D);

            // Create the theorems
            var theorems = new[]
            {
                new Theorem(configuration, CollinearPoints, A, B, C),
                new Theorem(configuration, CollinearPoints, A, B, D)
            };

            // We just need to assume the second one, the first one should be derived
            var result = Deriver.Derive(configuration, theorems, theorems.Skip(1)).ToList();

            // The result should contain the following theorems
            result.Select(triple => triple.Item3).ToSet(Theorem.EquivalencyComparer).SetEquals(new[]
            {
                new Theorem(configuration, CollinearPoints, A, B, D),
                new Theorem(configuration, CollinearPoints, A, C, D),
                new Theorem(configuration, CollinearPoints, B, C, D)
            })
           .Should().BeTrue();

            // Every triple should have fact1 and fact2
            result.Select(triple => new[] { triple.Item1, triple.Item2 }.ToSet(Theorem.EquivalencyComparer)).ForEach(set =>
            {
                // Equal to 
                set.SetEquals(new[]
                {
                    // The first theorem involving the last objects, i.e. the one with index 1
                    theorems[1],

                    // And the theorem stating that old points are collinear
                    new Theorem(configuration, CollinearPoints, A, B, C)
                })
                .Should().BeTrue();
            });
        }

        [Test]
        public void Test_With_Five_Collinear_Points()
        {
            // Create objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new ConstructedConfigurationObject(Midpoint, A, B);
            var D = new ConstructedConfigurationObject(Midpoint, A, C);
            var E = new ConstructedConfigurationObject(Midpoint, B, C);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(TwoPoints, C, D, E);

            // Create the theorems
            var theorems = new[]
            {
                new Theorem(configuration, CollinearPoints, A, B, C),
                new Theorem(configuration, CollinearPoints, B, C, D),
                new Theorem(configuration, CollinearPoints, C, D, E)
            };

            // We just need to assume the last, the first two should be derived
            var result = Deriver.Derive(configuration, theorems, theorems.Skip(2)).ToList();

            // The result should contain the following theorems
            result.Select(triple => triple.Item3).ToSet(Theorem.EquivalencyComparer).SetEquals(new[]
            {
                new Theorem(configuration, CollinearPoints, A, B, E),
                new Theorem(configuration, CollinearPoints, A, C, E),
                new Theorem(configuration, CollinearPoints, A, D, E),
                new Theorem(configuration, CollinearPoints, B, C, E),
                new Theorem(configuration, CollinearPoints, B, D, E),
                new Theorem(configuration, CollinearPoints, C, D, E)
            })
           .Should().BeTrue();

            // Every triple should have fact1 and fact2
            result.Select(triple => new[] { triple.Item1, triple.Item2 }.ToSet(Theorem.EquivalencyComparer)).ForEach(set =>
            {
                // Equal to 
                set.SetEquals(new[]
                {
                    // The first theorem involving the last objects, i.e. the one with index 2
                    theorems[2],

                    // And the theorem stating that old points are collinear
                    new Theorem(configuration, CollinearPoints, A, B, C, D)
                })
                .Should().BeTrue();
            });
        }

        [Test]
        public void Test_With_Five_Concyclic_Points()
        {
            // Create objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(MidpointOfOppositeArc, B, A, C);
            var E = new ConstructedConfigurationObject(MidpointOfOppositeArc, C, A, B);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(ThreePoints, D, E);

            // Create the theorems
            var theorems = new[]
            {
                new Theorem(configuration, ConcyclicPoints, A, B, C, D),
                new Theorem(configuration, ConcyclicPoints, A, B, C, E)
            };

            // We just need to assume the second one, the first one should be derived
            var result = Deriver.Derive(configuration, theorems, theorems.Skip(1)).ToList();

            // The result should contain the following theorems
            result.Select(triple => triple.Item3).ToSet(Theorem.EquivalencyComparer).SetEquals(new[]
            {
                new Theorem(configuration, ConcyclicPoints, A, B, C, E),
                new Theorem(configuration, ConcyclicPoints, A, B, D, E),
                new Theorem(configuration, ConcyclicPoints, A, C, D, E),
                new Theorem(configuration, ConcyclicPoints, B, C, D, E)
            })
           .Should().BeTrue();

            // Every triple should have fact1 and fact2
            result.Select(triple => new[] { triple.Item1, triple.Item2 }.ToSet(Theorem.EquivalencyComparer)).ForEach(set =>
            {
                // Equal to 
                set.SetEquals(new[]
                {
                    // The first theorem involving the last objects, i.e. the one with index 1
                    theorems[1],

                    // And the theorem stating that old points are concyclic
                    new Theorem(configuration, ConcyclicPoints, A, B, C, D)
                })
                .Should().BeTrue();
            });
        }

        [Test]
        public void Test_With_Six_Concyclic_Points()
        {
            // Create objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(MidpointOfOppositeArc, B, A, C);
            var E = new ConstructedConfigurationObject(MidpointOfOppositeArc, C, A, B);
            var F = new ConstructedConfigurationObject(MidpointOfOppositeArc, A, B, C);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(ThreePoints, D, E, F);

            // Create the theorems
            var theorems = new[]
            {
                new Theorem(configuration, ConcyclicPoints, A, B, C, D),
                new Theorem(configuration, ConcyclicPoints, A, B, C, E),
                new Theorem(configuration, ConcyclicPoints, B, C, E, F)
            };

            // We just need to assume the last one, the first two should be derived
            var result = Deriver.Derive(configuration, theorems, theorems.Skip(2)).ToList();

            // The result should contain the following theorems
            result.Select(triple => triple.Item3).ToSet(Theorem.EquivalencyComparer).SetEquals(new[]
            {
                new Theorem(configuration, ConcyclicPoints, A, B, C, F),
                new Theorem(configuration, ConcyclicPoints, A, B, D, F),
                new Theorem(configuration, ConcyclicPoints, A, B, E, F),
                new Theorem(configuration, ConcyclicPoints, A, C, D, F),
                new Theorem(configuration, ConcyclicPoints, A, C, E, F),
                new Theorem(configuration, ConcyclicPoints, A, D, E, F),
                new Theorem(configuration, ConcyclicPoints, B, C, D, F),
                new Theorem(configuration, ConcyclicPoints, B, C, E, F),
                new Theorem(configuration, ConcyclicPoints, B, D, E, F),
                new Theorem(configuration, ConcyclicPoints, C, D, E, F)
            })
           .Should().BeTrue();

            // Every triple should have fact1 and fact2
            result.Select(triple => new[] { triple.Item1, triple.Item2 }.ToSet(Theorem.EquivalencyComparer)).ForEach(set =>
            {
                // Equal to 
                set.SetEquals(new[]
                {
                    // The first theorem involving the last objects, i.e. the one with index 2
                    theorems[2],

                    // And the theorem stating that old points are concyclic
                    new Theorem(configuration, ConcyclicPoints, A, B, C, D, E)
                })
                .Should().BeTrue();
            });
        }
    }
}