using FluentAssertions;
using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.DependenciesResolver;
using GeoGen.TheoremFinder;
using GeoGen.Utilities;
using Ninject;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using static GeoGen.Core.ComposedConstructions;
using static GeoGen.Core.ConfigurationObjectType;
using static GeoGen.Core.LooseObjectsLayout;
using static GeoGen.Core.PredefinedConstructionType;
using static GeoGen.Core.RandomPointOnConstruction;
using static GeoGen.Core.TheoremType;

namespace GeoGen.TheoremProver.Test
{
    /// <summary>
    /// The test class for <see cref="SubtheoremDeriver"/>.
    /// </summary>
    [TestFixture]
    public class SubtheoremDeriverTest
    {
        #region Run and check methods

        /// <summary>
        /// Runs the algorithm on a given examined configuration with given template theorems.
        /// </summary>
        /// <param name="examinedConfiguration">The examined configuration.</param>
        /// <param name="templateTheorems">The template theorems.</param>
        /// <param name="templateConfiguration">The template configuration.</param>
        /// <returns>The list of results.</returns>
        private List<SubtheoremDeriverOutput> Run(Configuration examinedConfiguration, IEnumerable<Theorem> templateTheorems, Configuration templateConfiguration)
        {
            // Initialize IoC
            var kernel = IoC.CreateKernel()
                // Add theorem finder with no restrictions
                .AddTheoremFinder(new TangentCirclesTheoremFinderSettings
                {
                    ExcludeTangencyInsidePicture = false
                },
                new LineTangentToCircleTheoremFinderSettings
                {
                    ExcludeTangencyInsidePicture = false
                },
                typeof(TheoremType).GetEnumValues().Cast<TheoremType>().Except(new[] { EqualObjects, Incidence }).ToReadOnlyHashSet())
                // Add constructor ignoring inconsistencies
                .AddConstructor(new PicturesSettings
                {
                    NumberOfPictures = 5,
                    MaximalAttemptsToReconstructOnePicture = 0,
                    MaximalAttemptsToReconstructAllPictures = 0
                });

            // Create the constructor
            var constructor = kernel.Get<IGeometryConstructor>();

            // Create the deriver
            var deriver = new SubtheoremDeriver(constructor);

            // Draw the examined configuration
            var pictures = constructor.Construct(examinedConfiguration).pictures;

            // Draw the contextual picture
            var contextualPicture = kernel.Get<IContextualPictureFactory>().CreateContextualPicture(pictures);

            // Find the theorems
            var theorems = kernel.Get<ITheoremFinder>().FindAllTheorems(contextualPicture);

            // Run the algorithm
            return deriver.DeriveTheorems(new SubtheoremDeriverInput
            (
                examinedConfigurationPicture: contextualPicture,
                examinedConfigurationTheorems: theorems,
                templateConfiguration: templateConfiguration,
                templateTheorems: new TheoremMap(templateTheorems)
            ))
            // Enumerate the result
            .ToList();
        }

        /// <summary>
        /// Helper function that asserts equivalence of produced and expected results, 
        /// which should be set-equal.
        /// </summary>
        /// <param name="result">The result produced by the algorithm.</param>
        /// <param name="expected">The expected result.</param>
        private void CheckForEquivalncyOfResults(List<SubtheoremDeriverOutput> result, IEnumerable<SubtheoremDeriverOutput> expected)
        {
            // Find the items that are expected and not in the result
            var nonexistingExpectedOutputs = expected.Except(result).ToList();

            // Assert its empty
            nonexistingExpectedOutputs.Should().BeEmpty();

            // Find the items that results and not in the expected list
            var unexpectedResults = result.Except(expected).ToList();

            // Assert its empty
            unexpectedResults.Should().BeEmpty();
        }

        /// <summary>
        /// Helper function that asserts that the produced result contain every element from
        /// the expected result.
        /// </summary>
        /// <param name="result">The result produced by the algorithm.</param>
        /// <param name="expectedSubset">The expected result to be contained in the result..</param>
        private void CheckThatResultsContain(List<SubtheoremDeriverOutput> result, IEnumerable<SubtheoremDeriverOutput> expectedSubset)
        {
            // Find the items that are expected and not in the result
            var nonexistingExpectedOutputs = expectedSubset.Except(result).ToList();

            // Assert its empty
            nonexistingExpectedOutputs.Should().BeEmpty();
        }

        #endregion

        #region Line Segment template layout

        [Test]
        public void Test_Midpoint_Template_And_Configuration_With_Explicit_Midpoints()
        {
            // Create the template configuration's objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var M_ = new ConstructedConfigurationObject(Midpoint, A_, B_);

            // Create the template configuration
            var templateConfiguration = Configuration.DeriveFromObjects(LineSegment, M_);

            // Create the template theorems
            var templateTheorems = new Theorem[]
            {
                new Theorem(EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(A_, M_),
                    new LineSegmentTheoremObject(B_, M_)
                }),

                new Theorem(CollinearPoints, A_, B_, M_)
            };

            // Create the examined configuration's objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var M = new ConstructedConfigurationObject(Midpoint, B, C);
            var N = new ConstructedConfigurationObject(Midpoint, A, M);

            // Create the examined configuration
            var examinedConfiguration = Configuration.DeriveFromObjects(Triangle, N);

            // Run
            var results = Run(examinedConfiguration, templateTheorems, templateConfiguration);

            // Check results
            CheckForEquivalncyOfResults(results, new[]
            {
                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(EqualLineSegments, new[]
                        {
                            new LineSegmentTheoremObject(N, A),
                            new LineSegmentTheoremObject(N, M)
                        }),
                        templateTheorems[0]),

                        (new Theorem(CollinearPoints, A, M, N), templateTheorems[1])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>(),
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>(),
                    usedFacts: new List<Theorem>()
                ),

                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(EqualLineSegments, new[]
                        {
                            new LineSegmentTheoremObject(M, B),
                            new LineSegmentTheoremObject(M, C)
                        }),
                        templateTheorems[0]),

                        (new Theorem(CollinearPoints, B, C, M), templateTheorems[1])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>(),
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>(),
                    usedFacts: new List<Theorem>()
                )
            });
        }

        [Test]
        public void Test_Midpoint_Template_And_Configuration_With_Implicit_Midpoints()
        {
            // Create the template configuration's objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var M_ = new ConstructedConfigurationObject(Midpoint, A_, B_);

            // Create the template configuration
            var templateConfiguration = Configuration.DeriveFromObjects(LineSegment, M_);

            // Create the template theorems
            var templateTheorems = new Theorem[]
            {
                new Theorem(EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(A_, M_),
                    new LineSegmentTheoremObject(B_, M_)
                }),

                new Theorem(CollinearPoints, A_, B_, M_)
            };

            // Create the examined configuration's objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var O = new ConstructedConfigurationObject(Circumcenter, A, B, C);
            var M = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, O, B, C);

            // Create the examined configuration
            var examinedConfiguration = Configuration.DeriveFromObjects(Triangle, M);

            // Run
            var results = Run(examinedConfiguration, templateTheorems, templateConfiguration);

            // Check results
            CheckForEquivalncyOfResults(results, new[]
            {
                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(EqualLineSegments, new[]
                        {
                            new LineSegmentTheoremObject(M, B),
                            new LineSegmentTheoremObject(M, C)
                        }),
                        templateTheorems[0]),

                        (new Theorem(CollinearPoints, B, C, M), templateTheorems[1])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>
                    {
                        (M, new ConstructedConfigurationObject(Midpoint, B, C))
                    },
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>(),
                    usedFacts: new List<Theorem>()
                )
            });
        }

        [Test]
        public void Test_Reflection_Of_H_In_Midpoint_Of_BC_Is_Point_Opposite_To_A_On_Circumcircle_ABC()
        {
            // Create the template configuration's objects
            var A_ = new LooseConfigurationObject(Point);
            var M_ = new LooseConfigurationObject(Point);
            var B_ = new ConstructedConfigurationObject(PointReflection, A_, M_);

            // Create the template configuration
            var templateConfiguration = Configuration.DeriveFromObjects(LineSegment, B_);

            // Create the template theorems
            var templateTheorems = new[]
            {
                new Theorem(CollinearPoints, A_, B_, M_)
            };

            // Create the examined configuration's objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var H = new ConstructedConfigurationObject(Orthocenter, A, B, C);
            var O = new ConstructedConfigurationObject(Circumcenter, A, B, C);
            var D = new ConstructedConfigurationObject(PointReflection, A, O);
            var M = new ConstructedConfigurationObject(Midpoint, B, C);

            // Create the examined configuration
            var examinedConfiguration = Configuration.DeriveFromObjects(Triangle, M, D, H);

            // Run
            var results = Run(examinedConfiguration, templateTheorems, templateConfiguration);

            // Check count
            results.Count.Should().Be(6);

            // Check results
            CheckForEquivalncyOfResults(results, new[]
            {
                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(CollinearPoints, B, C, M), templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>
                    {
                        (B, new ConstructedConfigurationObject(PointReflection, C, M))
                    },
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>(),
                    usedFacts: new List<Theorem>()
                ),
                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(CollinearPoints, B, C, M), templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>
                    {
                        (C, new ConstructedConfigurationObject(PointReflection, B, M))
                    },
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>(),
                    usedFacts: new List<Theorem>()
                ),
                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(CollinearPoints, A, O, D), templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>(),
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>(),
                    usedFacts: new List<Theorem>()
                ),
                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(CollinearPoints, A, O, D), templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>
                    {
                        (A, new ConstructedConfigurationObject(PointReflection, D, O))
                    },
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>(),
                    usedFacts: new List<Theorem>()
                ),
                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(CollinearPoints, D, M, H), templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>
                    {
                        (H, new ConstructedConfigurationObject(PointReflection, D, M))
                    },
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>(),
                    usedFacts: new List<Theorem>()
                ),
                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(CollinearPoints, D, M, H), templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>
                    {
                        (D, new ConstructedConfigurationObject(PointReflection, H, M))
                    },
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>(),
                    usedFacts: new List<Theorem>()
                )
            });
        }

        [Test]
        public void Test_Random_Point_On_Line_Segment_Bisector_With_O()
        {
            // Create the template configuration's objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var M_ = new ConstructedConfigurationObject(RandomPointOn(PerpendicularBisector), A_, B_);

            // Create the template configuration
            var templateConfiguration = Configuration.DeriveFromObjects(LineSegment, M_);

            // Create the template theorems
            var templateTheorems = new Theorem[]
            {
                new Theorem(EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(A_, M_),
                    new LineSegmentTheoremObject(B_, M_)
                })
            };

            // Create the examined configuration's objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var O = new ConstructedConfigurationObject(Circumcenter, A, B, C);
            var M = new ConstructedConfigurationObject(Midpoint, B, C);

            // Create the examined configuration
            var examinedConfiguration = Configuration.DeriveFromObjects(Triangle, O, M);

            // Run
            var results = Run(examinedConfiguration, templateTheorems, templateConfiguration);

            // Check results
            CheckForEquivalncyOfResults(results, new[]
            {
                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(EqualLineSegments, new[]
                        {
                            new LineSegmentTheoremObject(M, B),
                            new LineSegmentTheoremObject(M, C)
                        }),
                        templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>(),
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>
                    {
                        (M, new ConstructedConfigurationObject(PerpendicularBisector, B, C))
                    },
                    usedFacts: new List<Theorem>()
                ),
                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(EqualLineSegments, new[]
                        {
                            new LineSegmentTheoremObject(O, B),
                            new LineSegmentTheoremObject(O, C)
                        }),
                        templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>(),
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>
                    {
                        (O, new ConstructedConfigurationObject(PerpendicularBisector, B, C))
                    },
                    usedFacts: new List<Theorem>()
                ),
                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(EqualLineSegments, new[]
                        {
                            new LineSegmentTheoremObject(O, C),
                            new LineSegmentTheoremObject(O, A)
                        }),
                        templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>(),
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>
                    {
                        (O, new ConstructedConfigurationObject(PerpendicularBisector, A, C))
                    },
                    usedFacts: new List<Theorem>()
                ),
                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(EqualLineSegments, new[]
                        {
                            new LineSegmentTheoremObject(O, A),
                            new LineSegmentTheoremObject(O, B)
                        }),
                        templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>(),
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>
                    {
                        (O, new ConstructedConfigurationObject(PerpendicularBisector, A, B))
                    },
                    usedFacts: new List<Theorem>()
                )
            });
        }

        [Test]
        public void Test_Random_Point_On_Explicit_Circle()
        {
            // Create the template configuration's objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var c_ = new ConstructedConfigurationObject(CircleWithDiameter, A_, B_);
            var C_ = new ConstructedConfigurationObject(RandomPointOnCircle, c_);

            // Create the template configuration
            var templateConfiguration = Configuration.DeriveFromObjects(LineSegment, A_, B_, c_, C_);

            // Create the template theorems
            var templateTheorems = new Theorem[]
            {
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(C_, A_),
                    new LineTheoremObject(C_, B_)
                })
            };

            // Create the examined configuration's objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, B, A, C);
            var c = new ConstructedConfigurationObject(Circumcircle, A, B, D);

            // Create the examined configuration
            var examinedConfiguration = Configuration.DeriveFromObjects(Triangle, c);

            // Run
            var results = Run(examinedConfiguration, templateTheorems, templateConfiguration);

            // Check results
            CheckForEquivalncyOfResults(results, new[]
            {
                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(PerpendicularLines, new[]
                        {
                            new LineTheoremObject(A, D),
                            new LineTheoremObject(D, B)
                        }),
                        templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>
                    {
                        (c, new ConstructedConfigurationObject(CircleWithDiameter, A, B))
                    },
                    usedFacts: new List<Theorem>(),
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>
                    {
                        (D, c)
                    }
                )
            });
        }

        [Test]
        public void Test_Random_Point_On_Explicit_line()
        {
            // Create the template configuration's objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var l_ = new ConstructedConfigurationObject(PerpendicularBisector, A_, B_);
            var D_ = new ConstructedConfigurationObject(RandomPointOnLine, l_);

            // Create the template configuration
            var templateConfiguration = Configuration.DeriveFromObjects(LineSegment, A_, B_, l_, D_);

            // Create the template theorems
            var templateTheorems = new Theorem[]
            {
                new Theorem(EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(D_, A_),
                    new LineSegmentTheoremObject(D_, B_)
                })
            };

            // Create the examined configuration's objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var M = new ConstructedConfigurationObject(Midpoint, B, C);
            var O = new ConstructedConfigurationObject(Circumcenter, A, B, C);
            var l = new ConstructedConfigurationObject(LineFromPoints, M, O);

            // Create the examined configuration
            var examinedConfiguration = Configuration.DeriveFromObjects(Triangle, l);

            // Run
            var results = Run(examinedConfiguration, templateTheorems, templateConfiguration);

            // Check results
            CheckForEquivalncyOfResults(results, new[]
            {
                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(EqualLineSegments, new[]
                        {
                            new LineSegmentTheoremObject(O, B),
                            new LineSegmentTheoremObject(O, C)
                        }),
                        templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>
                    {
                        (l, new ConstructedConfigurationObject(PerpendicularBisector, B, C))
                    },
                    usedFacts: new List<Theorem>(),
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>
                    {
                        (O, l)
                    }
                ),

                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(EqualLineSegments, new[]
                        {
                            new LineSegmentTheoremObject(M, B),
                            new LineSegmentTheoremObject(M, C)
                        }),
                        templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>
                    {
                        (l, new ConstructedConfigurationObject(PerpendicularBisector, B, C))
                    },
                    usedFacts: new List<Theorem>(),
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>
                    {
                        (M, l)
                    }
                )
            });
        }

        #endregion

        #region Triangle template layout

        [Test]
        public void Test_Equal_Angles_In_Situation_With_Explicit_Circumcenter()
        {
            // Create the template configuration's objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var C_ = new LooseConfigurationObject(Point);
            var O_ = new ConstructedConfigurationObject(Circumcenter, A_, B_, C_);

            // Create the template configuration
            var templateConfiguration = Configuration.DeriveFromObjects(Triangle, O_);

            // Create the template theorems
            var templateTheorems = new[]
            {
                new Theorem(EqualAngles, new[]
                {
                    new AngleTheoremObject(B_, C_, B_, O_),
                    new AngleTheoremObject(C_, O_, B_, C_)
                })
            };

            // Create the examined configuration's objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Midpoint, A, B);
            var E = new ConstructedConfigurationObject(Midpoint, A, C);
            var O = new ConstructedConfigurationObject(Circumcenter, A, D, E);

            // Create the examined configuration
            var examinedConfiguration = Configuration.DeriveFromObjects(Triangle, O);

            // Run
            var results = Run(examinedConfiguration, templateTheorems, templateConfiguration);

            // Check results
            CheckForEquivalncyOfResults(results, new[]
            {
                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(EqualAngles, new[]
                        {
                            new AngleTheoremObject(A, D, A, O),
                            new AngleTheoremObject(D, O, A, D)
                        }),
                        templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>(),
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>(),
                    usedFacts: new List<Theorem>()
                ),
                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(EqualAngles, new[]
                        {
                            new AngleTheoremObject(D, E, D, O),
                            new AngleTheoremObject(E, O, D, E)
                        }),
                        templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>(),
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>(),
                    usedFacts: new List<Theorem>()
                ),
                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(EqualAngles, new[]
                        {
                            new AngleTheoremObject(E, A, E, O),
                            new AngleTheoremObject(A, O, E, A)
                        }),
                        templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>(),
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>(),
                    usedFacts: new List<Theorem>()
                )
            });
        }

        [Test]
        public void Test_Equal_Angles_In_Situation_With_Implicit_Circumcenter()
        {
            // Create the template configuration's objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var C_ = new LooseConfigurationObject(Point);
            var O_ = new ConstructedConfigurationObject(Circumcenter, A_, B_, C_);

            // Create the template configuration
            var templateConfiguration = Configuration.DeriveFromObjects(Triangle, O_);

            // Create the template theorems
            var templateTheorems = new[]
            {
                new Theorem(EqualAngles, new[]
                {
                    new AngleTheoremObject(B_, C_, B_, O_),
                    new AngleTheoremObject(C_, O_, B_, C_)
                })
            };

            // Create the examined configuration's objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Midpoint, A, B);
            var E = new ConstructedConfigurationObject(Midpoint, A, C);
            var o1 = new ConstructedConfigurationObject(PerpendicularBisector, A, D);
            var o2 = new ConstructedConfigurationObject(PerpendicularBisector, D, E);
            var O = new ConstructedConfigurationObject(IntersectionOfLines, o1, o2);

            // Create the examined configuration
            var examinedConfiguration = Configuration.DeriveFromObjects(Triangle, O);

            // Run
            var results = Run(examinedConfiguration, templateTheorems, templateConfiguration);

            // Check results
            CheckForEquivalncyOfResults(results, new[]
            {
                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(EqualAngles, new[]
                        {
                            new AngleTheoremObject(A, D, A, O),
                            new AngleTheoremObject(D, O, A, D)
                        }),
                        templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>
                    {
                        (O, new ConstructedConfigurationObject(Circumcenter, A, D, E))
                    },
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>(),
                    usedFacts: new List<Theorem>()
                ),
                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(EqualAngles, new[]
                        {
                            new AngleTheoremObject(D, E, D, O),
                            new AngleTheoremObject(E, O, D, E)
                        }),
                        templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>
                    {
                        (O, new ConstructedConfigurationObject(Circumcenter, A, D, E))
                    },
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>(),
                    usedFacts: new List<Theorem>()
                ),
                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(EqualAngles, new[]
                        {
                            new AngleTheoremObject(E, A, E, O),
                            new AngleTheoremObject(A, O, E, A)
                        }),
                        templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>
                    {
                        (O, new ConstructedConfigurationObject(Circumcenter, A, D, E))
                    },
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>(),
                    usedFacts: new List<Theorem>()
                )
            });
        }

        [Test]
        public void Test_Medians_Are_Concurrent_With_Explicit_Definitions_Of_Midpoints()
        {
            // Create the template configuration's objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var C_ = new LooseConfigurationObject(Point);
            var D_ = new ConstructedConfigurationObject(Midpoint, B_, C_);
            var E_ = new ConstructedConfigurationObject(Midpoint, C_, A_);
            var F_ = new ConstructedConfigurationObject(Midpoint, A_, B_);

            // Create original configuration
            var templateConfiguration = Configuration.DeriveFromObjects(Triangle, D_, E_, F_);

            // Create the template theorems
            var templateTheorems = new[]
            {
                new Theorem(ConcurrentObjects, new[]
                {
                    new LineTheoremObject(A_, D_),
                    new LineTheoremObject(B_, E_),
                    new LineTheoremObject(C_, F_)
                })
             };

            // Create the examined configuration's objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Midpoint, B, C);
            var E = new ConstructedConfigurationObject(Midpoint, A, C);
            var F = new ConstructedConfigurationObject(Midpoint, A, D);
            var G = new ConstructedConfigurationObject(Midpoint, D, C);

            // Create the examined configuration
            var examinedConfiguration = Configuration.DeriveFromObjects(Triangle, E, F, G);

            // Run
            var results = Run(examinedConfiguration, templateTheorems, templateConfiguration);

            // Check results
            CheckForEquivalncyOfResults(results, new[]
            {
                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(ConcurrentObjects, new[]
                        {
                            new LineTheoremObject(A, G),
                            new LineTheoremObject(D, E),
                            new LineTheoremObject(C, F)
                        }),
                        templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>(),
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>(),
                    usedFacts: new List<Theorem>()
                )
            });
        }

        [Test]
        public void Test_Medians_Are_Concurrent_With_Implicit_Definitions_Of_Midpoints()
        {
            // Create the template configuration's objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var C_ = new LooseConfigurationObject(Point);
            var D_ = new ConstructedConfigurationObject(Midpoint, B_, C_);
            var E_ = new ConstructedConfigurationObject(Midpoint, C_, A_);
            var F_ = new ConstructedConfigurationObject(Midpoint, A_, B_);

            // Create the template configuration
            var templateConfiguration = Configuration.DeriveFromObjects(Triangle, D_, E_, F_);

            // Create the template theorems
            var templateTheorems = new[]
            {
                new Theorem(ConcurrentObjects, new[]
                {
                    new LineTheoremObject(A_, D_),
                    new LineTheoremObject(B_, E_),
                    new LineTheoremObject(C_, F_)
                })
             };

            // Create the examined configuration's objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var O = new ConstructedConfigurationObject(Circumcenter, A, B, C);
            var D = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, O, B, C);
            var E = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, O, C, A);
            var F = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, O, A, B);

            // Create the examined configuration
            var examinedConfiguration = Configuration.DeriveFromObjects(Triangle, D, E, F);

            // Run
            var results = Run(examinedConfiguration, templateTheorems, templateConfiguration);

            // Check results
            CheckForEquivalncyOfResults(results, new[]
            {
                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(ConcurrentObjects, new[]
                        {
                            new LineTheoremObject(A, D),
                            new LineTheoremObject(B, E),
                            new LineTheoremObject(C, F)
                        }),
                        templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>
                    {
                        (D, new ConstructedConfigurationObject(Midpoint, B, C)),
                        (E, new ConstructedConfigurationObject(Midpoint, C, A)),
                        (F, new ConstructedConfigurationObject(Midpoint, A, B))
                    },
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>(),
                    usedFacts: new List<Theorem>()
                )
            });
        }

        [Test]
        public void Test_Orthocentric_Situation_Has_Perpendicular_Lines()
        {
            // Create the template configuration's objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var C_ = new LooseConfigurationObject(Point);
            var H_ = new ConstructedConfigurationObject(Orthocenter, A_, B_, C_);

            // Create the template configuration
            var templateConfiguration = Configuration.DeriveFromObjects(Triangle, H_);

            // Create the template theorems
            var templateTheorems = new[]
            {
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(A_, H_),
                    new LineTheoremObject(B_, C_)
                })
            };

            // Create the examined configuration's objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var H = new ConstructedConfigurationObject(Orthocenter, A, B, C);

            // Create the examined configuration
            var examinedConfiguration = Configuration.DeriveFromObjects(Triangle, H);

            // Run
            var results = Run(examinedConfiguration, templateTheorems, templateConfiguration);

            // Check that perpendicularity of AH and BC can be derived dually
            CheckThatResultsContain(results, new[]
            {
                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(PerpendicularLines, new[]
                        {
                            new LineTheoremObject(A, H),
                            new LineTheoremObject(B, C)
                        }),
                        templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>(),
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>(),
                    usedFacts: new List<Theorem>()
                ),

                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(PerpendicularLines, new[]
                        {
                            new LineTheoremObject(A, H),
                            new LineTheoremObject(B, C)
                        }),
                        templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>
                    {
                        (A, new ConstructedConfigurationObject(Orthocenter, B, H, C))
                    },
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>(),
                    usedFacts: new List<Theorem>()
                )
            });
        }

        [Test]
        public void Test_Miquels_Theorem()
        {
            // Create the template configuration's objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var C_ = new LooseConfigurationObject(Point);
            var D_ = new ConstructedConfigurationObject(RandomPointOnLineFromPoints, A_, B_);
            var E_ = new ConstructedConfigurationObject(RandomPointOnLineFromPoints, B_, C_);
            var F_ = new ConstructedConfigurationObject(RandomPointOnLineFromPoints, C_, A_);

            // Create the template configuration
            var templateConfiguration = Configuration.DeriveFromObjects(Triangle, D_, E_, F_);

            // Create the template theorems
            var templateTheorems = new[]
            {
                new Theorem(ConcurrentObjects, new[]
                {
                    new CircleTheoremObject(A_, E_, F_),
                    new CircleTheoremObject(B_, F_, D_),
                    new CircleTheoremObject(C_, D_, E_),
                })
            };

            // Create the examined configuration's objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Midpoint, A, B);
            var E = new ConstructedConfigurationObject(Midpoint, B, C);
            var F = new ConstructedConfigurationObject(Midpoint, C, A);

            // Create the examined configuration
            var examinedConfiguration = Configuration.DeriveFromObjects(Triangle, D, E, F);

            // Run
            var results = Run(examinedConfiguration, templateTheorems, templateConfiguration);

            // Check results
            // We're not looking for equivalence, because Miquel's theorem
            // produces many theorems that have duplicate objects. Those are
            // technically true, and not important for our purposes (since
            // we're using this algorithm only for ruling out theorems)
            CheckThatResultsContain(results, new[]
            {
                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(ConcurrentObjects, new[]
                        {
                            new CircleTheoremObject(A, E, F),
                            new CircleTheoremObject(B, F, D),
                            new CircleTheoremObject(C, D, E)
                        }),
                        templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>(),
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>(),
                    usedFacts: new List<Theorem>
                    {
                        new Theorem(CollinearPoints, D, A, B),
                        new Theorem(CollinearPoints, E, B, C),
                        new Theorem(CollinearPoints, F, C, A)
                    }
                )
            });
        }

        #endregion

        #region Trapezoid template layout

        [Test]
        public void Test_Tangent_Circles_Because_Of_Homothety()
        {
            // Create the template configuration's objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var C_ = new LooseConfigurationObject(Point);
            var D_ = new LooseConfigurationObject(Point);
            var E_ = new ConstructedConfigurationObject(IntersectionOfLinesFromPoints, A_, C_, B_, D_);

            // Create the template configuration
            var templateConfiguration = Configuration.DeriveFromObjects(Trapezoid, A_, B_, C_, D_, E_);

            // Create the template theorems
            var templateTheorems = new[]
            {
                new Theorem(TangentCircles, new[]
                {
                    new CircleTheoremObject(E_, A_, B_),
                    new CircleTheoremObject(E_, C_, D_)
                })
            };

            // Create the examined configuration's objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Midpoint, A, B);
            var E = new ConstructedConfigurationObject(Midpoint, A, C);
            var F = new ConstructedConfigurationObject(Midpoint, B, C);
            var G = new ConstructedConfigurationObject(Midpoint, D, E);

            // Create the examined configuration
            var examinedConfiguration = Configuration.DeriveFromObjects(Triangle, D, E, F, G);

            // Run
            var results = Run(examinedConfiguration, templateTheorems, templateConfiguration);

            // Check results
            CheckForEquivalncyOfResults(results, new[]
            {
                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(TangentCircles, new[]
                        {
                            new CircleTheoremObject(A, D, E),
                            new CircleTheoremObject(A, B, C)
                        }),
                        templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>
                    {
                        (A, new ConstructedConfigurationObject(IntersectionOfLinesFromPoints, B, D, C, E))
                    },
                    usedFacts: new List<Theorem>
                    {
                        new Theorem(ParallelLines, new[]
                        {
                            new LineTheoremObject(B, C),
                            new LineTheoremObject(D, E)
                        })
                    },
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>()
                ),
                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(TangentCircles, new[]
                        {
                            new CircleTheoremObject(A, D, G),
                            new CircleTheoremObject(A, B, F)
                        }),
                        templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>
                    {
                        (A, new ConstructedConfigurationObject(IntersectionOfLinesFromPoints, B, D, F, G))
                    },
                    usedFacts: new List<Theorem>
                    {
                        new Theorem(ParallelLines, new[]
                        {
                            new LineTheoremObject(D, G),
                            new LineTheoremObject(B, F)
                        })
                    },
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>()
                ),
                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(TangentCircles, new[]
                        {
                            new CircleTheoremObject(A, G, E),
                            new CircleTheoremObject(A, F, C)
                        }),
                        templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>
                    {
                        (A, new ConstructedConfigurationObject(IntersectionOfLinesFromPoints, F, G, C, E))
                    },
                    usedFacts: new List<Theorem>
                    {
                        new Theorem(ParallelLines, new[]
                        {
                            new LineTheoremObject(G, E),
                            new LineTheoremObject(F, C)
                        })
                    },
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>()
                ),
                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(TangentCircles, new[]
                        {
                            new CircleTheoremObject(A, B, C),
                            new CircleTheoremObject(E, F, C)
                        }),
                        templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>
                    {
                        (C, new ConstructedConfigurationObject(IntersectionOfLinesFromPoints, A, E, B, F))
                    },
                    usedFacts: new List<Theorem>
                    {
                        new Theorem(ParallelLines, new[]
                        {
                            new LineTheoremObject(A, B),
                            new LineTheoremObject(E, F)
                        })
                    },
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>()
                ),
                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(TangentCircles, new[]
                        {
                            new CircleTheoremObject(A, B, C),
                            new CircleTheoremObject(B, F, D)
                        }),
                        templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>
                    {
                        (B, new ConstructedConfigurationObject(IntersectionOfLinesFromPoints, A, D, C, F))
                    },
                    usedFacts: new List<Theorem>
                    {
                        new Theorem(ParallelLines, new[]
                        {
                            new LineTheoremObject(A, C),
                            new LineTheoremObject(F, D)
                        })
                    },
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>()
                ),
                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(TangentCircles, new[]
                        {
                            new CircleTheoremObject(A, D, G),
                            new CircleTheoremObject(F, E, G)
                        }),
                        templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>
                    {
                        (G, new ConstructedConfigurationObject(IntersectionOfLinesFromPoints, A, F, D, E))
                    },
                    usedFacts: new List<Theorem>
                    {
                        new Theorem(ParallelLines, new[]
                        {
                            new LineTheoremObject(A, D),
                            new LineTheoremObject(F, E)
                        })
                    },
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>()
                ),
                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(TangentCircles, new[]
                        {
                            new CircleTheoremObject(A, E, G),
                            new CircleTheoremObject(F, D, G)
                        }),
                        templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>
                    {
                        (G, new ConstructedConfigurationObject(IntersectionOfLinesFromPoints, A, F, D, E))
                    },
                    usedFacts: new List<Theorem>
                    {
                        new Theorem(ParallelLines, new[]
                        {
                            new LineTheoremObject(A, E),
                            new LineTheoremObject(F, D)
                        })
                    },
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>()
                )
            });
        }

        [Test]
        public void Test_Equal_Angles_Because_Of_Parallel_Lines()
        {
            // Create the template configuration's objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var C_ = new LooseConfigurationObject(Point);
            var D_ = new LooseConfigurationObject(Point);
            var E_ = new ConstructedConfigurationObject(RandomPoint);
            var F_ = new ConstructedConfigurationObject(RandomPoint);

            // Create the template configuration
            var templateConfiguration = Configuration.DeriveFromObjects(Trapezoid, A_, B_, C_, D_, E_, F_);

            // Create the template theorems
            var templateTheorems = new[]
            {
                new Theorem(EqualAngles, new[]
                {
                    new AngleTheoremObject(A_, B_, E_, F_),
                    new AngleTheoremObject(C_, D_, E_, F_)
                })
            };

            // Create the examined configuration's objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Midpoint, A, B);
            var E = new ConstructedConfigurationObject(Midpoint, C, D);
            var F = new ConstructedConfigurationObject(Midpoint, A, E);

            // Create the examined configuration
            var examinedConfiguration = Configuration.DeriveFromObjects(Triangle, F);

            // Run
            var results = Run(examinedConfiguration, templateTheorems, templateConfiguration);

            // Check results
            CheckThatResultsContain(results, new[]
            {
                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(EqualAngles, new[]
                        {
                            new AngleTheoremObject(B, E, C, F),
                            new AngleTheoremObject(D, F, C, F)
                        }),
                        templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>(),
                    usedFacts: new List<Theorem>
                    {
                        new Theorem(ParallelLines, new[]
                        {
                            new LineTheoremObject(B, E),
                            new LineTheoremObject(D, F)
                        })
                    },
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>()
                ),
                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(EqualAngles, new[]
                        {
                            new AngleTheoremObject(A, C, B, E),
                            new AngleTheoremObject(A, C, D, F)
                        }),
                        templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>(),
                    usedFacts: new List<Theorem>
                    {
                        new Theorem(ParallelLines, new[]
                        {
                            new LineTheoremObject(B, E),
                            new LineTheoremObject(D, F)
                        })
                    },
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>()
                ),
                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(EqualAngles, new[]
                        {
                            new AngleTheoremObject(B, C, B, E),
                            new AngleTheoremObject(B, C, D, F)
                        }),
                        templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>(),
                    usedFacts: new List<Theorem>
                    {
                        new Theorem(ParallelLines, new[]
                        {
                            new LineTheoremObject(B, E),
                            new LineTheoremObject(D, F)
                        })
                    },
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>()
                )
            });
        }

        #endregion

        #region Quadrilateral template layout

        [Test]
        public void Test_Two_Lines_Perpendicular_To_Some_Line_Are_Parallel()
        {
            // Create the template configuration's objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var C_ = new LooseConfigurationObject(Point);
            var D_ = new LooseConfigurationObject(Point);
            var l1_ = new ConstructedConfigurationObject(PerpendicularLineToLineFromPoints, A_, C_, D_);
            var l2_ = new ConstructedConfigurationObject(PerpendicularLineToLineFromPoints, B_, C_, D_);

            // Create the template configuration
            var templateConfiguration = Configuration.DeriveFromObjects(Quadrilateral, l1_, l2_);

            // Create the template theorems
            var templateTheorems = new[]
            {
                new Theorem(ParallelLines, l1_, l2_)
            };

            // Create the examined configuration's objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var I = new ConstructedConfigurationObject(Incenter, A, B, C);
            var l1 = new ConstructedConfigurationObject(PerpendicularLineToLineFromPoints, A, B, C);
            var l2 = new ConstructedConfigurationObject(PerpendicularLineToLineFromPoints, I, B, C);

            // Create the examined configuration
            var examinedConfiguration = Configuration.DeriveFromObjects(Triangle, l1, l2);

            // Run
            var results = Run(examinedConfiguration, templateTheorems, templateConfiguration);

            // Check results
            CheckForEquivalncyOfResults(results, new[]
            {
                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(ParallelLines, l1, l2), templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>(),
                    usedFacts: new List<Theorem>(),
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>()
                )
            });
        }

        #endregion

        #region Cyclic quadrilateral template layout

        [Test]
        public void Test_Equal_Angles_Because_Of_Concylic_Points()
        {
            // Create the template configuration's objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var C_ = new LooseConfigurationObject(Point);
            var D_ = new LooseConfigurationObject(Point);

            // Create the template configuration
            var templateConfiguration = Configuration.DeriveFromObjects(CyclicQuadrilateral, A_, B_, C_, D_);

            // Create the template theorems
            var templateTheorems = new[]
            {
                new Theorem(EqualAngles, new[]
                {
                    new AngleTheoremObject(A_, B_, A_, D_),
                    new AngleTheoremObject(C_, B_, C_, D_)
                })
            };

            // Create the examined configuration's objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var H = new ConstructedConfigurationObject(Orthocenter, A, B, C);
            var D = new ConstructedConfigurationObject(ReflectionInLineFromPoints, H, B, C);

            // Create the examined configuration
            var examinedConfiguration = Configuration.DeriveFromObjects(Triangle, D);

            // Run
            var results = Run(examinedConfiguration, templateTheorems, templateConfiguration);

            // Check results
            CheckForEquivalncyOfResults(results, new[]
            {
                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(EqualAngles, new[]
                        {
                            new AngleTheoremObject(A, B, A, C),
                            new AngleTheoremObject(D, B, D, C)
                        }),
                        templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>(),
                    usedFacts: new List<Theorem>
                    {
                        new Theorem(ConcyclicPoints, A, B, C, D)
                    },
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>()
                ),
                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(EqualAngles, new[]
                        {
                            new AngleTheoremObject(C, A, C, D),
                            new AngleTheoremObject(B, A, B, D)
                        }),
                        templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>(),
                    usedFacts: new List<Theorem>
                    {
                        new Theorem(ConcyclicPoints, A, B, C, D)
                    },
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>()
                ),
                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(EqualAngles, new[]
                        {
                            new AngleTheoremObject(D, A, D, C),
                            new AngleTheoremObject(B, A, B, C)
                        }),
                        templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>(),
                    usedFacts: new List<Theorem>
                    {
                        new Theorem(ConcyclicPoints, A, B, C, D)
                    },
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>()
                ),
                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(EqualAngles, new[]
                        {
                            new AngleTheoremObject(D, A, D, B),
                            new AngleTheoremObject(C, A, C, B)
                        }),
                        templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>(),
                    usedFacts: new List<Theorem>
                    {
                        new Theorem(ConcyclicPoints, A, B, C, D)
                    },
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>()
                ),
                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(EqualAngles, new[]
                        {
                            new AngleTheoremObject(A, C, A, D),
                            new AngleTheoremObject(B, C, B, D)
                        }),
                        templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>(),
                    usedFacts: new List<Theorem>
                    {
                        new Theorem(ConcyclicPoints, A, B, C, D)
                    },
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>()
                ),
                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(EqualAngles, new[]
                        {
                            new AngleTheoremObject(A, B, A, D),
                            new AngleTheoremObject(C, B, C, D)
                        }),
                        templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>(),
                    usedFacts: new List<Theorem>
                    {
                        new Theorem(ConcyclicPoints, A, B, C, D)
                    },
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>()
                )
            });
        }

        #endregion  

        #region Right triangle layout

        [Test]
        public void Test_Theorems_In_Right_Triangle_With_Projected_Vertex()
        {
            // Create the template configuration's objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var C_ = new LooseConfigurationObject(Point);
            var D_ = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, A_, B_, C_);

            // Create the template configuration
            var templateConfiguration = Configuration.DeriveFromObjects(RightTriangle, A_, B_, C_, D_);

            // Create the template theorems
            var templateTheorems = new Theorem[]
            {
                new Theorem(EqualAngles, new[]
                {
                    new AngleTheoremObject(A_, B_, A_, D_),
                    new AngleTheoremObject(A_, C_, C_, D_)
                }),

                new Theorem(LineTangentToCircle, new TheoremObject[]
                {
                    new LineTheoremObject(A_, B_),
                    new CircleTheoremObject(A_, C_, D_),
                })
            };

            // Create the examined configuration's objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, A, B, C);
            var E = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, D, A, B);

            // Create the examined configuration
            var examinedConfiguration = Configuration.DeriveFromObjects(Triangle, E);

            // Run
            var results = Run(examinedConfiguration, templateTheorems, templateConfiguration);

            // Check results
            CheckForEquivalncyOfResults(results, new[]
            {
                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(EqualAngles, new[]
                        {
                            new AngleTheoremObject(D, A, D, E),
                            new AngleTheoremObject(D, B, B, E)
                        }),
                        templateTheorems[0]),

                        (new Theorem(LineTangentToCircle, new TheoremObject[]
                        {
                            new LineTheoremObject(A, D),
                            new CircleTheoremObject(B, D, E)
                        }),
                        templateTheorems[1])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>(),
                    usedFacts: new List<Theorem>
                    {
                        new Theorem(PerpendicularLines, new[]
                        {
                            new LineTheoremObject(A, D),
                            new LineTheoremObject(B, D)
                        })
                    },
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>()
                ),
                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(EqualAngles, new[]
                        {
                            new AngleTheoremObject(D, B, D, E),
                            new AngleTheoremObject(D, A, A, E)
                        }),
                        templateTheorems[0]),

                        (new Theorem(LineTangentToCircle, new TheoremObject[]
                        {
                            new LineTheoremObject(B, D),
                            new CircleTheoremObject(A, D, E)
                        }),
                        templateTheorems[1])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>(),
                    usedFacts: new List<Theorem>
                    {
                        new Theorem(PerpendicularLines, new[]
                        {
                            new LineTheoremObject(A, D),
                            new LineTheoremObject(B, D)
                        })
                    },
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>()
                )
            });
        }

        [Test]
        public void Test_With_Tangent_Circles_Because_Of_Right_Angles()
        {
            // Create the template configuration's objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var C_ = new LooseConfigurationObject(Point);
            var D_ = new ConstructedConfigurationObject(RandomPointOnLineFromPoints, B_, C_);
            var E_ = new ConstructedConfigurationObject(RandomPointOn(CircleWithDiameter), D_, B_);

            // Create the template configuration
            var templateConfiguration = Configuration.DeriveFromObjects(RightTriangle, A_, B_, C_, D_, E_);

            // Create the template theorems
            var templateTheorems = new Theorem[]
            {
                new Theorem(TangentCircles, new TheoremObject[]
                {
                    new CircleTheoremObject(D_, B_, E_),
                    new CircleTheoremObject(A_, B_, C_),
                })
            };

            // Create the examined configuration's objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, A, B, C);
            var E = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, B, A, C);
            var F = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, A, D, E);

            // Create the examined configuration
            var examinedConfiguration = Configuration.DeriveFromObjects(Triangle, D, E, F);

            // Run
            var results = Run(examinedConfiguration, templateTheorems, templateConfiguration);

            // Check results
            CheckForEquivalncyOfResults(results, new[]
            {
                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(TangentCircles, new TheoremObject[]
                        {
                            new CircleTheoremObject(A, C, D),
                            new CircleTheoremObject(A, E, F)
                        }),
                        templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>(),
                    usedFacts: new List<Theorem>
                    {
                        new Theorem(PerpendicularLines, new[]
                        {
                            new LineTheoremObject(A, F),
                            new LineTheoremObject(F, E)
                        }),

                        new Theorem(CollinearPoints, A, C, E),
                    },
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>
                    {
                        (D, new ConstructedConfigurationObject(CircleWithDiameter, A, C))
                    }
                ),
                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(TangentCircles, new TheoremObject[]
                        {
                            new CircleTheoremObject(A, C, D),
                            new CircleTheoremObject(A, E, F)
                        }),
                        templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>(),
                    usedFacts: new List<Theorem>
                    {
                        new Theorem(PerpendicularLines, new[]
                        {
                            new LineTheoremObject(A, D),
                            new LineTheoremObject(D, C)
                        }),

                        new Theorem(CollinearPoints, A, C, E),
                    },
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>
                    {
                        (F, new ConstructedConfigurationObject(CircleWithDiameter, A, E))
                    }
                )
            });
        }

        #endregion

        #region Circle and tangent line template layout

        [Test]
        public void Test_Line_Tangent_To_Circle_Produces_Equal_Angles_In_Simple_Situation()
        {
            // Create the template configuration's objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var C_ = new LooseConfigurationObject(Point);
            var D_ = new LooseConfigurationObject(Point);

            // Create the template configuration
            var templateConfiguration = Configuration.DeriveFromObjects(CircleAndTangentLine, A_, B_, C_, D_);

            // Create the template theorems
            var templateTheorems = new Theorem[]
            {
                new Theorem(EqualAngles, new[]
                {
                    new AngleTheoremObject(A_, D_, A_, B_),
                    new AngleTheoremObject(C_, A_, C_, B_)
                })
            };

            // Create the examined configuration's objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var l = new ConstructedConfigurationObject(InternalAngleBisector, A, B, C);
            var D = new ConstructedConfigurationObject(IntersectionOfLineAndLineFromPoints, l, B, C);
            var m = new ConstructedConfigurationObject(PerpendicularBisector, A, D);
            var E = new ConstructedConfigurationObject(IntersectionOfLineAndLineFromPoints, m, B, C);

            // Create the examined configuration
            var examinedConfiguration = Configuration.DeriveFromObjects(Triangle, E);

            // Run
            var results = Run(examinedConfiguration, templateTheorems, templateConfiguration);

            // Check results
            CheckForEquivalncyOfResults(results, new[]
            {
                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(EqualAngles, new[]
                        {
                            new AngleTheoremObject(E, A, A, B),
                            new AngleTheoremObject(C, A, C, B)
                        }),
                        templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>(),
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>(),
                    usedFacts: new List<Theorem>
                    {
                        new Theorem(LineTangentToCircle, new TheoremObject[]
                        {
                            new LineTheoremObject(E, A),
                            new CircleTheoremObject(A, B, C)
                        })
                    }
                ),

                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(EqualAngles, new[]
                        {
                            new AngleTheoremObject(E, A, A, C),
                            new AngleTheoremObject(B, A, B, C)
                        }),
                        templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>(),
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>(),
                    usedFacts: new List<Theorem>
                    {
                        new Theorem(LineTangentToCircle, new TheoremObject[]
                        {
                            new LineTheoremObject(E, A),
                            new CircleTheoremObject(A, B, C)
                        })
                    }
                )
            });
        }

        [Test]
        public void Test_Line_Tangent_To_Circle_Produces_Equal_Angles_In_Complex_Situation()
        {
            // Create the template configuration's objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var C_ = new LooseConfigurationObject(Point);
            var D_ = new LooseConfigurationObject(Point);

            // Create the template configuration
            var templateConfiguration = Configuration.DeriveFromObjects(CircleAndTangentLine, A_, B_, C_, D_);

            // Create the template theorems
            var templateTheorems = new Theorem[]
            {
                new Theorem(EqualAngles, new[]
                {
                    new AngleTheoremObject(A_, D_, A_, B_),
                    new AngleTheoremObject(C_, A_, C_, B_)
                })
            };

            // Create the examined configuration's objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, A, B, C);
            var E = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, B, A, C);
            var F = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, A, D, E);

            // Create the examined configuration
            var examinedConfiguration = Configuration.DeriveFromObjects(Triangle, D, E, F);

            // Run
            var results = Run(examinedConfiguration, templateTheorems, templateConfiguration);

            // Check results
            CheckThatResultsContain(results, new[]
            {
                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(EqualAngles, new[]
                        {
                            new AngleTheoremObject(A, F, A, D),
                            new AngleTheoremObject(C, D, D, F)
                        }),
                        templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>(),
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>(),
                    usedFacts: new List<Theorem>
                    {
                        new Theorem(LineTangentToCircle, new TheoremObject[]
                        {
                            new LineTheoremObject(C, D),
                            new CircleTheoremObject(A, F, D)
                        })
                    }
                )
            });
        }

        #endregion

        #region Isosceles triangle layout

        [Test]
        public void Test_Isosceles_Triangle_Has_Interesting_Midpoint_Of_Side()
        {
            // Create the template configuration's objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var C_ = new LooseConfigurationObject(Point);
            var M_ = new ConstructedConfigurationObject(Midpoint, B_, C_);

            // Create the template configuration
            var templateConfiguration = Configuration.DeriveFromObjects(IsoscelesTriangle, A_, B_, C_, M_);

            // Create the template theorems
            var templateTheorems = new[]
            {
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(A_, M_),
                    new LineTheoremObject(B_, C_)
                }),
                new Theorem(EqualAngles, new[]
                {
                    new AngleTheoremObject(B_, A_, A_, M_),
                    new AngleTheoremObject(C_, A_, A_, M_)
                })
            };

            // Create the examined configuration's objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var I = new ConstructedConfigurationObject(Incenter, A, B, C);
            var S = new ConstructedConfigurationObject(SecondIntersectionOfCircleAndLineFromPoints, A, I, B, C);
            var M = new ConstructedConfigurationObject(Midpoint, B, C);

            // Create the examined configuration
            var examinedConfiguration = Configuration.DeriveFromObjects(Triangle, I, S, M);

            // Run
            var results = Run(examinedConfiguration, templateTheorems, templateConfiguration);

            // Check results
            CheckForEquivalncyOfResults(results, new[]
            {
                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(PerpendicularLines, new[]
                        {
                            new LineTheoremObject(S, M),
                            new LineTheoremObject(B, C)
                        }),
                        templateTheorems[0]),

                        (new Theorem(EqualAngles, new[]
                        {
                            new AngleTheoremObject(S, M, S, B),
                            new AngleTheoremObject(S, M, S, C)
                        }),
                        templateTheorems[1])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>(),
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>(),
                    usedFacts: new List<Theorem>
                    {
                        new Theorem(EqualLineSegments, new[]
                        {
                            new LineSegmentTheoremObject(S, B),
                            new LineSegmentTheoremObject(S, C)
                        })
                    }
                )
            });
        }

        [Test]
        public void Test_Isosceles_Triangle_With_Two_Points_On_Its_Bisector()
        {
            // Create the template configuration's objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var C_ = new LooseConfigurationObject(Point);
            var D_ = new ConstructedConfigurationObject(RandomPointOn(PerpendicularBisector), B_, C_);

            // Create the template configuration
            var templateConfiguration = Configuration.DeriveFromObjects(IsoscelesTriangle, A_, B_, C_, D_);

            // Create the template theorems
            var templateTheorems = new[]
            {
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(A_, D_),
                    new LineTheoremObject(B_, C_)
                }),
                new Theorem(EqualAngles, new[]
                {
                    new AngleTheoremObject(A_, B_, B_, D_),
                    new AngleTheoremObject(A_, C_, C_, D_)
                })
            };

            // Create the examined configuration's objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var O1 = new ConstructedConfigurationObject(Circumcenter, A, B, C);
            var O2 = new ConstructedConfigurationObject(Circumcenter, O1, B, C);

            // Create the examined configuration
            var examinedConfiguration = Configuration.DeriveFromObjects(Triangle, O2);

            // Run
            var results = Run(examinedConfiguration, templateTheorems, templateConfiguration);

            // Check results
            CheckThatResultsContain(results, new[]
            {
                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(PerpendicularLines, new[]
                        {
                            new LineTheoremObject(O1, O2),
                            new LineTheoremObject(B, C)
                        }),
                        templateTheorems[0]),

                        (new Theorem(EqualAngles, new[]
                        {
                            new AngleTheoremObject(O1, B, B, O2),
                            new AngleTheoremObject(O1, C, C, O2)
                        }),
                        templateTheorems[1])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>(),
                    usedFacts: new List<Theorem>
                    {
                        new Theorem(EqualLineSegments, new[]
                        {
                            new LineSegmentTheoremObject(O1, B),
                            new LineSegmentTheoremObject(O1, C)
                        })
                    },
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>
                    {
                        (O2, new ConstructedConfigurationObject(PerpendicularBisector, B, C))
                    }
                )
            });
        }

        #endregion

        #region Line template layout

        [Test]
        public void Test_Line_And_Perpendicular_Line_At_It()
        {
            // Create the template configuration's objects
            var l_ = new LooseConfigurationObject(Line);
            var A_ = new ConstructedConfigurationObject(RandomPointOnLine, l_);
            var m_ = new ConstructedConfigurationObject(PerpendicularLine, A_, l_);

            // Create the template configuration
            var templateConfiguration = Configuration.DeriveFromObjects(ExplicitLine, l_, A_, m_);

            // Create the template theorems
            var templateTheorems = new[]
            {
                new Theorem(PerpendicularLines, l_, m_)
            };

            // Create the examined configuration's objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var l = new ConstructedConfigurationObject(InternalAngleBisector, A, B, C);
            var m = new ConstructedConfigurationObject(ExternalAngleBisector, A, B, C);

            // Create the examined configuration
            var examinedConfiguration = Configuration.DeriveFromObjects(Triangle, l, m);

            // Run
            var results = Run(examinedConfiguration, templateTheorems, templateConfiguration);

            // Check results
            CheckForEquivalncyOfResults(results, new[]
            {
                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(PerpendicularLines, l, m), templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>
                    {
                        (m, new ConstructedConfigurationObject(PerpendicularLine, A, l))
                    },
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>
                    {
                        (A, l)
                    },
                    usedFacts: new List<Theorem>()
                ),

                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(PerpendicularLines, l, m), templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>
                    {
                        (l, new ConstructedConfigurationObject(PerpendicularLine, A, m))
                    },
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>
                    {
                        (A, m)
                    },
                    usedFacts: new List<Theorem>()
                )
            });
        }

        #endregion

        #region Line and point template layout

        [Test]
        public void Test_Line_And_Projection_From_Point_Outside_It()
        {
            // Create the template configuration's objects
            var l_ = new LooseConfigurationObject(Line);
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new ConstructedConfigurationObject(PerpendicularProjection, A_, l_);

            // Create the template configuration
            var templateConfiguration = Configuration.DeriveFromObjects(ExplicitLineAndPoint, l_, A_, B_);

            // Create the template theorems
            var templateTheorems = new[]
            {
                new Theorem(PerpendicularLines, new[]
                {
                    new LineTheoremObject(A_, B_),
                    new LineTheoremObject(l_)
                })
            };

            // Create the examined configuration's objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var l1 = new ConstructedConfigurationObject(LineFromPoints, B, C);
            var l2 = new ConstructedConfigurationObject(PerpendicularLine, A, l1);
            var D = new ConstructedConfigurationObject(IntersectionOfLines, l1, l2);

            // Create the examined configuration
            var examinedConfiguration = Configuration.DeriveFromObjects(Triangle, l1, l2, D);

            // Run
            var results = Run(examinedConfiguration, templateTheorems, templateConfiguration);

            // Check results
            CheckForEquivalncyOfResults(results, new[]
            {
                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(PerpendicularLines, new[]
                        {
                            new LineTheoremObject(l1),
                            new LineTheoremObject(A, D)
                        }),
                        templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>
                    {
                        (D, new ConstructedConfigurationObject(PerpendicularProjection, A, l1))
                    },
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>(),
                    usedFacts: new List<Theorem>()
                ),

                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(PerpendicularLines, new[]
                        {
                            new LineTheoremObject(l2),
                            new LineTheoremObject(B, D)
                        }),
                        templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>
                    {
                        (D, new ConstructedConfigurationObject(PerpendicularProjection, B, l2))
                    },
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>(),
                    usedFacts: new List<Theorem>()
                ),

                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(PerpendicularLines, new[]
                        {
                            new LineTheoremObject(l2),
                            new LineTheoremObject(C, D)
                        }),
                        templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>
                    {
                        (D, new ConstructedConfigurationObject(PerpendicularProjection, C, l2))
                    },
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>(),
                    usedFacts: new List<Theorem>()
                ),
            });
        }

        #endregion

        #region Line and two points template layout

        [Test]
        public void Test_Line_And_Its_Intersection_With_Two_Points_Outside()
        {
            // Create the template configuration's objects
            var l_ = new LooseConfigurationObject(Line);
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var P_ = new ConstructedConfigurationObject(IntersectionOfLineAndLineFromPoints, l_, A_, B_);

            // Create the template configuration
            var templateConfiguration = Configuration.DeriveFromObjects(ExplicitLineAndTwoPoints, l_, A_, B_, P_);

            // Create the template theorems
            var templateTheorems = new[]
            {
                new Theorem(CollinearPoints, A_, B_, P_)
            };

            // Create the examined configuration's objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var H = new ConstructedConfigurationObject(Orthocenter, A, B, C);
            var l1 = new ConstructedConfigurationObject(PerpendicularLineToLineFromPoints, H, A, B);
            var l2 = new ConstructedConfigurationObject(PerpendicularLineToLineFromPoints, H, A, C);
            var D = new ConstructedConfigurationObject(IntersectionOfLineAndLineFromPoints, l1, A, B);
            var E = new ConstructedConfigurationObject(IntersectionOfLineAndLineFromPoints, l2, A, C);

            // Create the examined configuration
            var examinedConfiguration = Configuration.DeriveFromObjects(Triangle, D, E);

            // Run
            var results = Run(examinedConfiguration, templateTheorems, templateConfiguration);

            // Check results
            CheckForEquivalncyOfResults(results, new[]
            {
                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(CollinearPoints, A, B, D), templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>(),
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>(),
                    usedFacts: new List<Theorem>()
                ),

                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(CollinearPoints, A, C, E), templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>(),
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>(),
                    usedFacts: new List<Theorem>()
                ),

                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(CollinearPoints, A, D, B), templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>
                    {
                        (B, new ConstructedConfigurationObject(IntersectionOfLineAndLineFromPoints, l2, A, D))
                    },
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>(),
                    usedFacts: new List<Theorem>()
                ),

                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(CollinearPoints, A, C, E), templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>
                    {
                        (C, new ConstructedConfigurationObject(IntersectionOfLineAndLineFromPoints, l1, A, E))
                    },
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>(),
                    usedFacts: new List<Theorem>()
                ),

                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(CollinearPoints, C, D, H), templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>
                    {
                        (H, new ConstructedConfigurationObject(IntersectionOfLineAndLineFromPoints, l2, C, D))
                    },
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>(),
                    usedFacts: new List<Theorem>()
                ),

                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(CollinearPoints, B, E, H), templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>
                    {
                        (H, new ConstructedConfigurationObject(IntersectionOfLineAndLineFromPoints, l1, B, E))
                    },
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>(),
                    usedFacts: new List<Theorem>()
                )
            });
        }

        #endregion

        #region Circle template layout

        [Test]
        public void Test_Circle_And_Its_Center()
        {
            // Create the template configuration's objects
            var c_ = new LooseConfigurationObject(Circle);
            var A_ = new ConstructedConfigurationObject(RandomPointOnCircle, c_);
            var B_ = new ConstructedConfigurationObject(RandomPointOnCircle, c_);
            var O_ = new ConstructedConfigurationObject(CenterOfCircle, c_);

            // Create the template configuration
            var templateConfiguration = Configuration.DeriveFromObjects(ExplicitCircle, A_, B_, O_);

            // Create the template theorems
            var templateTheorems = new[]
            {
                new Theorem(EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(O_, A_),
                    new LineSegmentTheoremObject(O_, B_),
                })
            };

            // Create the examined configuration's objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var O = new ConstructedConfigurationObject(Circumcenter, A, B, C);
            var c = new ConstructedConfigurationObject(CircleWithCenterThroughPoint, O, A);

            // Create the examined configuration
            var examinedConfiguration = Configuration.DeriveFromObjects(Triangle, O, c);

            // Run
            var results = Run(examinedConfiguration, templateTheorems, templateConfiguration);

            // Check results
            CheckForEquivalncyOfResults(results, new[]
            {
                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(EqualLineSegments, new[]
                        {
                            new LineSegmentTheoremObject(O, A),
                            new LineSegmentTheoremObject(O, B),
                        }), templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>
                    {
                        (O, new ConstructedConfigurationObject(CenterOfCircle, c))
                    },
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>
                    {
                        (A, c),
                        (B, c)
                    },
                    usedFacts: new List<Theorem>()
                ),

                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(EqualLineSegments, new[]
                        {
                            new LineSegmentTheoremObject(O, B),
                            new LineSegmentTheoremObject(O, C),
                        }), templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>
                    {
                        (O, new ConstructedConfigurationObject(CenterOfCircle, c))
                    },
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>
                    {
                        (B, c),
                        (C, c)
                    },
                    usedFacts: new List<Theorem>()
                ),

                new SubtheoremDeriverOutput
                (
                    derivedTheorems: new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(EqualLineSegments, new[]
                        {
                            new LineSegmentTheoremObject(O, C),
                            new LineSegmentTheoremObject(O, A),
                        }), templateTheorems[0])
                    },
                    usedEqualities: new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>
                    {
                        (O, new ConstructedConfigurationObject(CenterOfCircle, c))
                    },
                    usedIncidencies: new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>
                    {
                        (A, c),
                        (C, c)
                    },
                    usedFacts: new List<Theorem>()
                ),
            });
        }

        #endregion
    }
}