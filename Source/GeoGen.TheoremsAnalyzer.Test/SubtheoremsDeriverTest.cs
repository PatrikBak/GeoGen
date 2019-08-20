using FluentAssertions;
using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.DependenciesResolver;
using GeoGen.TheoremsFinder;
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

namespace GeoGen.TheoremsAnalyzer.Test
{
    /// <summary>
    /// The test class for <see cref="SubtheoremsDeriver"/>.
    /// </summary>
    [TestFixture]
    public class SubtheoremsDeriverTest
    {
        #region Outputs comparer

        /// <summary>
        /// The equality comparer for <see cref="SubtheoremsDeriverOutput"/>.
        /// </summary>
        private static readonly IEqualityComparer<SubtheoremsDeriverOutput> OutputsComparer = new SimpleEqualityComparer<SubtheoremsDeriverOutput>(
            // Method to say whether two objects are the same
            (o1, o2) =>
            {
                // Prepare the comparer for object equality tuples. The first objects
                // are compared by references. The constructed ones by equivalences
                var equalityTuplesComparer = new SimpleEqualityComparer<(ConfigurationObject, ConstructedConfigurationObject)>(
                     // Return that their objects are equivalent
                     (t1, t2) => t1.Item1 == t2.Item1 && t1.Item2.IsEquivalentTo(t2.Item2));

                // Prepare the comparer for theorem equality tuples
                var theoremTuplesComparer = new SimpleEqualityComparer<(Theorem, Theorem)>(
                     // Return that their objects are equivalent
                     (t1, t2) => t1.Item1.IsEquivalentTo(t2.Item1) && t1.Item2.IsEquivalentTo(t2.Item2));

                // Return true when used equalities are set-equals
                return o1.UsedEqualities.ToSet(equalityTuplesComparer).SetEquals(o2.UsedEqualities)
                     // As well as derived theorems are set-equals
                     && o1.DerivedTheorems.ToSet(theoremTuplesComparer).SetEquals(o2.DerivedTheorems);
            });

        #endregion

        #region Run and check methods

        /// <summary>
        /// Runs the algorithm on a given examined configuration with given template theorems.
        /// </summary>
        /// <param name="examinedConfiguration">The examined configuration.</param>
        /// <param name="templateTheorems">The template theorems.</param>
        /// <returns>The list of results.</returns>
        private List<SubtheoremsDeriverOutput> Run(Configuration examinedConfiguration, IEnumerable<Theorem> templateTheorems)
        {
            // Initialize IoC
            var kernel = IoC.CreateKernel().AddTheoremsFinder().AddConstructor(new PicturesSettings
            {
                NumberOfPictures = 5,
                MaximalAttemptsToReconstructOnePicture = 0,
                MaximalAttemptsToReconstructAllPictures = 0
            });

            // Create the constructor
            var constructor = kernel.Get<IGeometryConstructor>();

            // Create the deriver
            var deriver = new SubtheoremsDeriver(constructor);

            // Draw the examined configuration
            var pictures = constructor.Construct(examinedConfiguration).pictures;

            // Draw the contextual picture
            var contextualPicture = kernel.Get<IContextualPictureFactory>().CreateContextualPicture(pictures);

            // Find the theorems
            var theorems = new TheoremsMap(kernel.GetAll<ITheoremsFinder>().SelectMany(finder => finder.FindAllTheorems(contextualPicture)));

            // Run the algorithm
            return deriver.DeriveTheorems(new SubtheoremsDeriverInput
            {
                ExaminedConfigurationPicture = contextualPicture,
                ExaminedConfigurationTheorems = theorems,
                TemplateConfiguration = templateTheorems.First().Configuration,
                TemplateTheorems = new TheoremsMap(templateTheorems)
            })
            // Enumerate the result
            .ToList();
        }

        /// <summary>
        /// Helper function that asserts equivalence of produced and expected results, 
        /// which should be set-equal.
        /// </summary>
        /// <param name="result">The result produced by the algorithm.</param>
        /// <param name="expected">The expected result.</param>
        private void CheckForEquivalncyOfResults(List<SubtheoremsDeriverOutput> result, IEnumerable<SubtheoremsDeriverOutput> expected)
        {
            // Find the items that are expected and not in the result
            var nonexistingExpectedOutputs = expected.Except(result, OutputsComparer).ToList();

            // Assert its empty
            nonexistingExpectedOutputs.Should().BeEmpty();

            // Find the items that results and not in the expected list
            var unexpectedResults = result.Except(expected, OutputsComparer).ToList();

            // Assert its empty
            unexpectedResults.Should().BeEmpty();
        }

        /// <summary>
        /// Helper function that asserts that the produced result contain every element from
        /// the expected result.
        /// </summary>
        /// <param name="result">The result produced by the algorithm.</param>
        /// <param name="expectedSubset">The expected result to be contained in the result..</param>
        private void CheckThatResultsContain(List<SubtheoremsDeriverOutput> result, IEnumerable<SubtheoremsDeriverOutput> expectedSubset)
        {
            // Find the items that are expected and not in the result
            var nonexistingExpectedOutputs = expectedSubset.Except(result, OutputsComparer).ToList();

            // Assert its empty
            nonexistingExpectedOutputs.Should().BeEmpty();
        }

        #endregion

        #region Two points layouts

        [Test]
        public void Test_Midpoint_Template_And_Configuration_With_Explicit_Midpoints()
        {
            // Create the template configuration's objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var M_ = new ConstructedConfigurationObject(Midpoint, A_, B_);

            // Create the template configuration
            var templateConfiguration = Configuration.DeriveFromObjects(TwoPoints, M_);

            // Create the template theorems
            var templateTheorems = new Theorem[]
            {
                new Theorem(templateConfiguration, EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(A_, M_),
                    new LineSegmentTheoremObject(B_, M_)
                }),

                new Theorem(templateConfiguration, CollinearPoints, A_, B_, M_)
            };

            // Create the examined configuration's objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var M = new ConstructedConfigurationObject(Midpoint, B, C);
            var N = new ConstructedConfigurationObject(Midpoint, A, M);

            // Create the examined configuration
            var examinedConfiguration = Configuration.DeriveFromObjects(ThreePoints, N);

            // Run
            var results = Run(examinedConfiguration, templateTheorems);

            // Check count
            results.Count.Should().Be(4);

            // Check results
            CheckForEquivalncyOfResults(results, new[]
            {
                new SubtheoremsDeriverOutput
                {
                    DerivedTheorems = new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(examinedConfiguration, EqualLineSegments, new[]
                        {
                            new LineSegmentTheoremObject(N, A),
                            new LineSegmentTheoremObject(N, M)
                        }),
                        templateTheorems[0]),

                        (new Theorem(examinedConfiguration, CollinearPoints, A, M, N), templateTheorems[1])
                    },
                    UsedEqualities = new List<(ConfigurationObject originalObject, ConstructedConfigurationObject equalObject)>()
                },

                new SubtheoremsDeriverOutput
                {
                    DerivedTheorems = new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(examinedConfiguration, EqualLineSegments, new[]
                        {
                            new LineSegmentTheoremObject(M, B),
                            new LineSegmentTheoremObject(M, C)
                        }),
                        templateTheorems[0]),

                        (new Theorem(examinedConfiguration, CollinearPoints, B, C, M), templateTheorems[1])
                    },
                    UsedEqualities = new List<(ConfigurationObject originalObject, ConstructedConfigurationObject equalObject)>()
                }
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
            var templateConfiguration = Configuration.DeriveFromObjects(TwoPoints, M_);

            // Create the template theorems
            var templateTheorems = new Theorem[]
            {
                new Theorem(templateConfiguration, EqualLineSegments, new[]
                {
                    new LineSegmentTheoremObject(A_, M_),
                    new LineSegmentTheoremObject(B_, M_)
                }),

                new Theorem(templateConfiguration, CollinearPoints, A_, B_, M_)
            };

            // Create the examined configuration's objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var O = new ConstructedConfigurationObject(Circumcenter, A, B, C);
            var M = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, O, B, C);

            // Create the examined configuration
            var examinedConfiguration = Configuration.DeriveFromObjects(ThreePoints, M);

            // Run
            var results = Run(examinedConfiguration, templateTheorems);

            // Check count
            results.Count.Should().Be(2);

            // Check results
            CheckForEquivalncyOfResults(results, new[]
            {
                new SubtheoremsDeriverOutput
                {
                    DerivedTheorems = new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(examinedConfiguration, EqualLineSegments, new[]
                        {
                            new LineSegmentTheoremObject(M, B),
                            new LineSegmentTheoremObject(M, C)
                        }),
                        templateTheorems[0]),

                        (new Theorem(examinedConfiguration, CollinearPoints, B, C, M), templateTheorems[1])
                    },
                    UsedEqualities = new List<(ConfigurationObject originalObject, ConstructedConfigurationObject equalObject)>
                    {
                        (M, new ConstructedConfigurationObject(Midpoint, B, C))
                    }
                }
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
            var templateConfiguration = Configuration.DeriveFromObjects(TwoPoints, B_);

            // Create the template theorems
            var templateTheorems = new[]
            {
                new Theorem(templateConfiguration, CollinearPoints, A_, B_, M_)
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
            var examinedConfiguration = Configuration.DeriveFromObjects(ThreePoints, M, D, H);

            // Run
            var results = Run(examinedConfiguration, templateTheorems);

            // Check count
            results.Count.Should().Be(6);

            // Check results
            CheckForEquivalncyOfResults(results, new[]
            {
                new SubtheoremsDeriverOutput
                {
                    DerivedTheorems = new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(examinedConfiguration, CollinearPoints, B, C, M), templateTheorems[0])
                    },
                    UsedEqualities = new List<(ConfigurationObject originalObject, ConstructedConfigurationObject equalObject)>
                    {
                        (B, new ConstructedConfigurationObject(PointReflection, C, M))
                    }
                },
                new SubtheoremsDeriverOutput
                {
                    DerivedTheorems = new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(examinedConfiguration, CollinearPoints, B, C, M), templateTheorems[0])
                    },
                    UsedEqualities = new List<(ConfigurationObject originalObject, ConstructedConfigurationObject equalObject)>
                    {
                        (C, new ConstructedConfigurationObject(PointReflection, B, M))
                    }
                },
                new SubtheoremsDeriverOutput
                {
                    DerivedTheorems = new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(examinedConfiguration, CollinearPoints, A, O, D), templateTheorems[0])
                    },
                    UsedEqualities = new List<(ConfigurationObject originalObject, ConstructedConfigurationObject equalObject)>()
                },
                new SubtheoremsDeriverOutput
                {
                    DerivedTheorems = new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(examinedConfiguration, CollinearPoints, A, O, D), templateTheorems[0])
                    },
                    UsedEqualities = new List<(ConfigurationObject originalObject, ConstructedConfigurationObject equalObject)>
                    {
                        (A, new ConstructedConfigurationObject(PointReflection, D, O))
                    }
                },
                new SubtheoremsDeriverOutput
                {
                    DerivedTheorems = new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(examinedConfiguration, CollinearPoints, D, M, H), templateTheorems[0])
                    },
                    UsedEqualities = new List<(ConfigurationObject originalObject, ConstructedConfigurationObject equalObject)>
                    {
                        (H, new ConstructedConfigurationObject(PointReflection, D, M))
                    }
                },
                new SubtheoremsDeriverOutput
                {
                    DerivedTheorems = new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(examinedConfiguration, CollinearPoints, D, M, H), templateTheorems[0])
                    },
                    UsedEqualities = new List<(ConfigurationObject originalObject, ConstructedConfigurationObject equalObject)>
                    {
                        (D, new ConstructedConfigurationObject(PointReflection, H, M))
                    }
                }
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
            var templateConfiguration = Configuration.DeriveFromObjects(TwoPoints, M_);

            // Create the template theorems
            var templateTheorems = new Theorem[]
            {
                new Theorem(templateConfiguration, EqualLineSegments, new[]
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
            var examinedConfiguration = Configuration.DeriveFromObjects(ThreePoints, O, M);

            // Run
            var results = Run(examinedConfiguration, templateTheorems);

            // Check results
            CheckForEquivalncyOfResults(results, new[]
            {
                new SubtheoremsDeriverOutput
                {
                    DerivedTheorems = new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(examinedConfiguration, EqualLineSegments, new[]
                        {
                            new LineSegmentTheoremObject(M, B),
                            new LineSegmentTheoremObject(M, C)
                        }),
                        templateTheorems[0])
                    },
                    UsedEqualities = new List<(ConfigurationObject originalObject, ConstructedConfigurationObject equalObject)>()
                },
                new SubtheoremsDeriverOutput
                {
                    DerivedTheorems = new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(examinedConfiguration, EqualLineSegments, new[]
                        {
                            new LineSegmentTheoremObject(O, B),
                            new LineSegmentTheoremObject(O, C)
                        }),
                        templateTheorems[0])
                    },
                    UsedEqualities = new List<(ConfigurationObject originalObject, ConstructedConfigurationObject equalObject)>()
                },
                new SubtheoremsDeriverOutput
                {
                    DerivedTheorems = new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(examinedConfiguration, EqualLineSegments, new[]
                        {
                            new LineSegmentTheoremObject(O, C),
                            new LineSegmentTheoremObject(O, A)
                        }),
                        templateTheorems[0])
                    },
                    UsedEqualities = new List<(ConfigurationObject originalObject, ConstructedConfigurationObject equalObject)>()
                },
                new SubtheoremsDeriverOutput
                {
                    DerivedTheorems = new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(examinedConfiguration, EqualLineSegments, new[]
                        {
                            new LineSegmentTheoremObject(O, A),
                            new LineSegmentTheoremObject(O, B)
                        }),
                        templateTheorems[0])
                    },
                    UsedEqualities = new List<(ConfigurationObject originalObject, ConstructedConfigurationObject equalObject)>()
                }
            });
        }

        #endregion

        #region Three points layout

        [Test]
        public void Test_Equal_Angles_In_Situation_With_Explicit_Circumcenter()
        {
            // Create the template configuration's objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var C_ = new LooseConfigurationObject(Point);
            var O_ = new ConstructedConfigurationObject(Circumcenter, A_, B_, C_);

            // Create the template configuration
            var templateConfiguration = Configuration.DeriveFromObjects(ThreePoints, O_);

            // Create the template theorems
            var templateTheorems = new[]
            {
                new Theorem(templateConfiguration, EqualAngles, new[]
                {
                    new AngleTheoremObject(new LineTheoremObject(B_, C_), new LineTheoremObject(B_, O_)),
                    new AngleTheoremObject(new LineTheoremObject(C_, O_), new LineTheoremObject(B_, C_))
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
            var examinedConfiguration = Configuration.DeriveFromObjects(ThreePoints, O);

            // Run
            var results = Run(examinedConfiguration, templateTheorems);

            // Check count
            results.Count.Should().Be(6);

            // Check results
            CheckForEquivalncyOfResults(results, new[]
            {
                new SubtheoremsDeriverOutput
                {
                    DerivedTheorems = new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(examinedConfiguration, EqualAngles, new[]
                        {
                            new AngleTheoremObject(new LineTheoremObject(A, D), new LineTheoremObject(A, O)),
                            new AngleTheoremObject(new LineTheoremObject(D, O), new LineTheoremObject(A, D))
                        }),
                        templateTheorems[0])
                    },
                    UsedEqualities = new List<(ConfigurationObject originalObject, ConstructedConfigurationObject equalObject)>()
                },

                new SubtheoremsDeriverOutput
                {
                    DerivedTheorems = new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(examinedConfiguration, EqualAngles, new[]
                        {
                            new AngleTheoremObject(new LineTheoremObject(D, E), new LineTheoremObject(D, O)),
                            new AngleTheoremObject(new LineTheoremObject(E, O), new LineTheoremObject(D, E))
                        }),
                        templateTheorems[0])
                    },
                    UsedEqualities = new List<(ConfigurationObject originalObject, ConstructedConfigurationObject equalObject)>()
                },

                new SubtheoremsDeriverOutput
                {
                    DerivedTheorems = new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(examinedConfiguration, EqualAngles, new[]
                        {
                            new AngleTheoremObject(new LineTheoremObject(E, A), new LineTheoremObject(E, O)),
                            new AngleTheoremObject(new LineTheoremObject(A, O), new LineTheoremObject(E, A))
                        }),
                        templateTheorems[0])
                    },
                    UsedEqualities = new List<(ConfigurationObject originalObject, ConstructedConfigurationObject equalObject)>()
                }
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
            var templateConfiguration = Configuration.DeriveFromObjects(ThreePoints, O_);

            // Create the template theorems
            var templateTheorems = new[]
            {
                new Theorem(templateConfiguration, EqualAngles, new[]
                {
                    new AngleTheoremObject(new LineTheoremObject(B_, C_), new LineTheoremObject(B_, O_)),
                    new AngleTheoremObject(new LineTheoremObject(C_, O_), new LineTheoremObject(B_, C_))
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
            var examinedConfiguration = Configuration.DeriveFromObjects(ThreePoints, O);

            // Run
            var results = Run(examinedConfiguration, templateTheorems);

            // Check count
            results.Count.Should().Be(6);

            // Check results
            CheckForEquivalncyOfResults(results, new[]
            {
                new SubtheoremsDeriverOutput
                {
                    DerivedTheorems = new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(examinedConfiguration, EqualAngles, new[]
                        {
                            new AngleTheoremObject(new LineTheoremObject(A, D), new LineTheoremObject(A, O)),
                            new AngleTheoremObject(new LineTheoremObject(D, O), new LineTheoremObject(A, D))
                        }),
                        templateTheorems[0])
                    },
                    UsedEqualities = new List<(ConfigurationObject originalObject, ConstructedConfigurationObject equalObject)>
                    {
                        (O, new ConstructedConfigurationObject(Circumcenter, A, D, E))
                    }
                },

                new SubtheoremsDeriverOutput
                {
                    DerivedTheorems = new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(examinedConfiguration, EqualAngles, new[]
                        {
                            new AngleTheoremObject(new LineTheoremObject(D, E), new LineTheoremObject(D, O)),
                            new AngleTheoremObject(new LineTheoremObject(E, O), new LineTheoremObject(D, E))
                        }),
                        templateTheorems[0])
                    },
                    UsedEqualities = new List<(ConfigurationObject originalObject, ConstructedConfigurationObject equalObject)>
                    {
                        (O, new ConstructedConfigurationObject(Circumcenter, A, D, E))
                    }
                },

                new SubtheoremsDeriverOutput
                {
                    DerivedTheorems = new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(examinedConfiguration, EqualAngles, new[]
                        {
                            new AngleTheoremObject(new LineTheoremObject(E, A), new LineTheoremObject(E, O)),
                            new AngleTheoremObject(new LineTheoremObject(A, O), new LineTheoremObject(E, A))
                        }),
                        templateTheorems[0])
                    },
                    UsedEqualities = new List<(ConfigurationObject originalObject, ConstructedConfigurationObject equalObject)>
                    {
                        (O, new ConstructedConfigurationObject(Circumcenter, A, D, E))
                    }
                }
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
            var templateConfiguration = Configuration.DeriveFromObjects(ThreePoints, D_, E_, F_);

            // Create the template theorems
            var templateTheorems = new[]
            {
                new Theorem(templateConfiguration, ConcurrentObjects, new[]
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
            var examinedConfiguration = Configuration.DeriveFromObjects(ThreePoints, E, F, G);

            // Run
            var results = Run(examinedConfiguration, templateTheorems);

            // Check count
            results.Count.Should().Be(6);

            // Check results
            CheckForEquivalncyOfResults(results, new[]
            {
                new SubtheoremsDeriverOutput
                {
                    DerivedTheorems = new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(examinedConfiguration, ConcurrentObjects, new[]
                        {
                            new LineTheoremObject(A, G),
                            new LineTheoremObject(D, E),
                            new LineTheoremObject(C, F)
                        }),
                        templateTheorems[0])
                    },
                    UsedEqualities = new List<(ConfigurationObject originalObject, ConstructedConfigurationObject equalObject)>()
                }
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
            var templateConfiguration = Configuration.DeriveFromObjects(ThreePoints, D_, E_, F_);

            // Create the template theorems
            var templateTheorems = new[]
            {
                new Theorem(templateConfiguration, ConcurrentObjects, new[]
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
            var examinedConfiguration = Configuration.DeriveFromObjects(ThreePoints, D, E, F);

            // Run
            var results = Run(examinedConfiguration, templateTheorems);

            // Check count
            results.Count.Should().Be(6);

            // Check results
            CheckForEquivalncyOfResults(results, new[]
            {
                new SubtheoremsDeriverOutput
                {
                    DerivedTheorems = new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(examinedConfiguration, ConcurrentObjects, new[]
                        {
                            new LineTheoremObject(A, D),
                            new LineTheoremObject(B, E),
                            new LineTheoremObject(C, F)
                        }),
                        templateTheorems[0])
                    },
                    UsedEqualities = new List<(ConfigurationObject originalObject, ConstructedConfigurationObject equalObject)>
                    {
                        (D, new ConstructedConfigurationObject(Midpoint, B, C)),
                        (E, new ConstructedConfigurationObject(Midpoint, C, A)),
                        (F, new ConstructedConfigurationObject(Midpoint, A, B))
                    }
                }
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
            var templateConfiguration = Configuration.DeriveFromObjects(ThreePoints, H_);

            // Create the template theorems
            var templateTheorems = new[]
            {
                new Theorem(templateConfiguration, PerpendicularLines, new[]
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
            var examinedConfiguration = Configuration.DeriveFromObjects(ThreePoints, H);

            // Run
            var results = Run(examinedConfiguration, templateTheorems);

            // Check count
            results.Count.Should().Be(24, "The orthocenter from every three points is the forth one");

            // Check that perpendicularity of AH and BC can be derived dually
            CheckThatResultsContain(results, new[]
            {
                new SubtheoremsDeriverOutput
                {
                    DerivedTheorems = new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(examinedConfiguration, PerpendicularLines, new[]
                        {
                            new LineTheoremObject(A, H),
                            new LineTheoremObject(B, C)
                        }),
                        templateTheorems[0])
                    },
                    UsedEqualities = new List<(ConfigurationObject originalObject, ConstructedConfigurationObject equalObject)>()
                },

                new SubtheoremsDeriverOutput
                {
                    DerivedTheorems = new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(examinedConfiguration, PerpendicularLines, new[]
                        {
                            new LineTheoremObject(A, H),
                            new LineTheoremObject(B, C)
                        }),
                        templateTheorems[0])
                    },
                    UsedEqualities = new List<(ConfigurationObject originalObject, ConstructedConfigurationObject equalObject)>
                    {
                        (A, new ConstructedConfigurationObject(Orthocenter, B, H, C))
                    }
                }
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
            var templateConfiguration = Configuration.DeriveFromObjects(ThreePoints, D_, E_, F_);

            // Create the template theorems
            var templateTheorems = new[]
            {
                new Theorem(templateConfiguration, ConcurrentObjects, new[]
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
            var examinedConfiguration = Configuration.DeriveFromObjects(ThreePoints, D, E, F);

            // Run
            var results = Run(examinedConfiguration, templateTheorems);

            // Check results
            // We're not looking for equivalence, because Miquel's theorem
            // produces many theorems that have duplicate objects. Those are
            // technically true, and not important for our purposes (since
            // we're using this algorithm only for ruling out theorems)
            CheckThatResultsContain(results, new[]
            {
                new SubtheoremsDeriverOutput
                {
                    DerivedTheorems = new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(examinedConfiguration, ConcurrentObjects, new[]
                        {
                            new CircleTheoremObject(A, E, F),
                            new CircleTheoremObject(B, F, D),
                            new CircleTheoremObject(C, D, E)
                        }),
                        templateTheorems[0])
                    },
                    UsedEqualities = new List<(ConfigurationObject originalObject, ConstructedConfigurationObject equalObject)>()
                }
            });
        }

        #endregion

        #region Trapezoid layout

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
                new Theorem(templateConfiguration, TangentCircles, new[]
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
            var examinedConfiguration = Configuration.DeriveFromObjects(ThreePoints, D, E, F, G);

            // Run
            var results = Run(examinedConfiguration, templateTheorems);

            // Check results
            CheckForEquivalncyOfResults(results, new[]
            {
                new SubtheoremsDeriverOutput
                {
                    DerivedTheorems = new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(examinedConfiguration, TangentCircles, new[]
                        {
                            new CircleTheoremObject(A, D, E),
                            new CircleTheoremObject(A, B, C)
                        }),
                        templateTheorems[0])
                    },
                    UsedEqualities = new List<(ConfigurationObject originalObject, ConstructedConfigurationObject equalObject)>
                    {
                        (A, new ConstructedConfigurationObject(IntersectionOfLinesFromPoints, B, D, C, E))
                    }
                },
                new SubtheoremsDeriverOutput
                {
                    DerivedTheorems = new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(examinedConfiguration, TangentCircles, new[]
                        {
                            new CircleTheoremObject(A, D, G),
                            new CircleTheoremObject(A, B, F)
                        }),
                        templateTheorems[0])
                    },
                    UsedEqualities = new List<(ConfigurationObject originalObject, ConstructedConfigurationObject equalObject)>
                    {
                        (A, new ConstructedConfigurationObject(IntersectionOfLinesFromPoints, B, D, F, G))
                    }
                },
                new SubtheoremsDeriverOutput
                {
                    DerivedTheorems = new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(examinedConfiguration, TangentCircles, new[]
                        {
                            new CircleTheoremObject(A, G, E),
                            new CircleTheoremObject(A, F, C)
                        }),
                        templateTheorems[0])
                    },
                    UsedEqualities = new List<(ConfigurationObject originalObject, ConstructedConfigurationObject equalObject)>
                    {
                        (A, new ConstructedConfigurationObject(IntersectionOfLinesFromPoints, F, G, C, E))
                    }
                },
                new SubtheoremsDeriverOutput
                {
                    DerivedTheorems = new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(examinedConfiguration, TangentCircles, new[]
                        {
                            new CircleTheoremObject(A, B, C),
                            new CircleTheoremObject(E, F, C)
                        }),
                        templateTheorems[0])
                    },
                    UsedEqualities = new List<(ConfigurationObject originalObject, ConstructedConfigurationObject equalObject)>
                    {
                        (C, new ConstructedConfigurationObject(IntersectionOfLinesFromPoints, A, E, B, F))
                    }
                },
                new SubtheoremsDeriverOutput
                {
                    DerivedTheorems = new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(examinedConfiguration, TangentCircles, new[]
                        {
                            new CircleTheoremObject(A, B, C),
                            new CircleTheoremObject(B, F, D)
                        }),
                        templateTheorems[0])
                    },
                    UsedEqualities = new List<(ConfigurationObject originalObject, ConstructedConfigurationObject equalObject)>
                    {
                        (B, new ConstructedConfigurationObject(IntersectionOfLinesFromPoints, A, D, C, F))
                    }
                },
                new SubtheoremsDeriverOutput
                {
                    DerivedTheorems = new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(examinedConfiguration, TangentCircles, new[]
                        {
                            new CircleTheoremObject(A, D, G),
                            new CircleTheoremObject(F, E, G)
                        }),
                        templateTheorems[0])
                    },
                    UsedEqualities = new List<(ConfigurationObject originalObject, ConstructedConfigurationObject equalObject)>
                    {
                        (G, new ConstructedConfigurationObject(IntersectionOfLinesFromPoints, A, F, D, E))
                    }
                },
                new SubtheoremsDeriverOutput
                {
                    DerivedTheorems = new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(examinedConfiguration, TangentCircles, new[]
                        {
                            new CircleTheoremObject(A, E, G),
                            new CircleTheoremObject(F, D, G)
                        }),
                        templateTheorems[0])
                    },
                    UsedEqualities = new List<(ConfigurationObject originalObject, ConstructedConfigurationObject equalObject)>
                    {
                        (G, new ConstructedConfigurationObject(IntersectionOfLinesFromPoints, A, F, D, E))
                    }
                }
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
                new Theorem(templateConfiguration, EqualAngles, new[]
                {
                    new AngleTheoremObject(new LineTheoremObject(A_, B_), new LineTheoremObject(E_, F_)),
                    new AngleTheoremObject(new LineTheoremObject(C_, D_), new LineTheoremObject(E_, F_))
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
            var examinedConfiguration = Configuration.DeriveFromObjects(ThreePoints, F);

            // Run
            var results = Run(examinedConfiguration, templateTheorems);

            // Check results
            CheckThatResultsContain(results, new[]
            {
                new SubtheoremsDeriverOutput
                {
                    DerivedTheorems = new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(examinedConfiguration, EqualAngles, new[]
                        {
                            new AngleTheoremObject(new LineTheoremObject(B, E), new LineTheoremObject(C, F)),
                            new AngleTheoremObject(new LineTheoremObject(D, F), new LineTheoremObject(C, F))
                        }),
                        templateTheorems[0])
                    },
                    UsedEqualities = new List<(ConfigurationObject originalObject, ConstructedConfigurationObject equalObject)>()
                },
                new SubtheoremsDeriverOutput
                {
                    DerivedTheorems = new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(examinedConfiguration, EqualAngles, new[]
                        {
                            new AngleTheoremObject(new LineTheoremObject(A, C), new LineTheoremObject(B, E)),
                            new AngleTheoremObject(new LineTheoremObject(A, C), new LineTheoremObject(D, F))
                        }),
                        templateTheorems[0])
                    },
                    UsedEqualities = new List<(ConfigurationObject originalObject, ConstructedConfigurationObject equalObject)>()
                },
                new SubtheoremsDeriverOutput
                {
                    DerivedTheorems = new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(examinedConfiguration, EqualAngles, new[]
                        {
                            new AngleTheoremObject(new LineTheoremObject(B, C), new LineTheoremObject(B, E)),
                            new AngleTheoremObject(new LineTheoremObject(B, C), new LineTheoremObject(D, F))
                        }),
                        templateTheorems[0])
                    },
                    UsedEqualities = new List<(ConfigurationObject originalObject, ConstructedConfigurationObject equalObject)>()
                }
            });
        }

        #endregion

        #region Four points layout

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
            var templateConfiguration = Configuration.DeriveFromObjects(FourPoints, l1_, l2_);

            // Create the template theorems
            var templateTheorems = new[]
            {
                new Theorem(templateConfiguration, ParallelLines, l1_, l2_)
            };

            // Create the examined configuration's objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var G = new ConstructedConfigurationObject(Centroid, A, B, C);
            var l1 = new ConstructedConfigurationObject(PerpendicularLineToLineFromPoints, A, B, C);
            var l2 = new ConstructedConfigurationObject(PerpendicularLineToLineFromPoints, G, B, C);

            // Create the examined configuration
            var examinedConfiguration = Configuration.DeriveFromObjects(ThreePoints, l1, l2);

            // Run
            var results = Run(examinedConfiguration, templateTheorems);

            // Check results
            CheckForEquivalncyOfResults(results, new[]
            {
                new SubtheoremsDeriverOutput
                {
                    DerivedTheorems = new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(examinedConfiguration, ParallelLines, l1, l2), templateTheorems[0])
                    },
                    UsedEqualities = new List<(ConfigurationObject originalObject, ConstructedConfigurationObject equalObject)>()
                }
            });
        }

        #endregion

        #region Four concyclic points layout

        [Test]
        public void Test_Equal_Angles_Because_Of_Concylic_Points()
        {
            // Create the template configuration's objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var C_ = new LooseConfigurationObject(Point);
            var D_ = new LooseConfigurationObject(Point);

            // Create the template configuration
            var templateConfiguration = Configuration.DeriveFromObjects(FourConcyclicPoints, A_, B_, C_, D_);

            // Create the template theorems
            var templateTheorems = new[]
            {
                new Theorem(templateConfiguration, EqualAngles, new[]
                {
                    new AngleTheoremObject(new LineTheoremObject(A_, B_), new LineTheoremObject(A_, D_)),
                    new AngleTheoremObject(new LineTheoremObject(C_, B_), new LineTheoremObject(C_, D_))
                })
            };

            // Create the examined configuration's objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var H = new ConstructedConfigurationObject(Orthocenter, A, B, C);
            var D = new ConstructedConfigurationObject(ReflectionInLineFromPoints, H, B, C);

            // Create the examined configuration
            var examinedConfiguration = Configuration.DeriveFromObjects(ThreePoints, D);

            // Run
            var results = Run(examinedConfiguration, templateTheorems);

            // Check results
            CheckForEquivalncyOfResults(results, new[]
            {
                new SubtheoremsDeriverOutput
                {
                    DerivedTheorems = new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(examinedConfiguration, EqualAngles, new[]
                        {
                            new AngleTheoremObject(new LineTheoremObject(A, B), new LineTheoremObject(A, C)),
                            new AngleTheoremObject(new LineTheoremObject(D, B), new LineTheoremObject(D, C))
                        }),
                        templateTheorems[0])
                    },
                    UsedEqualities = new List<(ConfigurationObject originalObject, ConstructedConfigurationObject equalObject)>()
                },
                new SubtheoremsDeriverOutput
                {
                    DerivedTheorems = new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(examinedConfiguration, EqualAngles, new[]
                        {
                            new AngleTheoremObject(new LineTheoremObject(C, A), new LineTheoremObject(C, D)),
                            new AngleTheoremObject(new LineTheoremObject(B, A), new LineTheoremObject(B, D))
                        }),
                        templateTheorems[0])
                    },
                    UsedEqualities = new List<(ConfigurationObject originalObject, ConstructedConfigurationObject equalObject)>()
                },
                new SubtheoremsDeriverOutput
                {
                    DerivedTheorems = new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(examinedConfiguration, EqualAngles, new[]
                        {
                            new AngleTheoremObject(new LineTheoremObject(D, A), new LineTheoremObject(D, C)),
                            new AngleTheoremObject(new LineTheoremObject(B, A), new LineTheoremObject(B, C))
                        }),
                        templateTheorems[0])
                    },
                    UsedEqualities = new List<(ConfigurationObject originalObject, ConstructedConfigurationObject equalObject)>()
                },
                new SubtheoremsDeriverOutput
                {
                    DerivedTheorems = new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(examinedConfiguration, EqualAngles, new[]
                        {
                            new AngleTheoremObject(new LineTheoremObject(D, A), new LineTheoremObject(D, B)),
                            new AngleTheoremObject(new LineTheoremObject(C, A), new LineTheoremObject(C, B))
                        }),
                        templateTheorems[0])
                    },
                    UsedEqualities = new List<(ConfigurationObject originalObject, ConstructedConfigurationObject equalObject)>()
                },
                new SubtheoremsDeriverOutput
                {
                    DerivedTheorems = new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(examinedConfiguration, EqualAngles, new[]
                        {
                            new AngleTheoremObject(new LineTheoremObject(A, C), new LineTheoremObject(A, D)),
                            new AngleTheoremObject(new LineTheoremObject(B, C), new LineTheoremObject(B, D))
                        }),
                        templateTheorems[0])
                    },
                    UsedEqualities = new List<(ConfigurationObject originalObject, ConstructedConfigurationObject equalObject)>()
                },
                new SubtheoremsDeriverOutput
                {
                    DerivedTheorems = new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(examinedConfiguration, EqualAngles, new[]
                        {
                            new AngleTheoremObject(new LineTheoremObject(A, B), new LineTheoremObject(A, D)),
                            new AngleTheoremObject(new LineTheoremObject(C, B), new LineTheoremObject(C, D))
                        }),
                        templateTheorems[0])
                    },
                    UsedEqualities = new List<(ConfigurationObject originalObject, ConstructedConfigurationObject equalObject)>()
                }
            });
        }

        #endregion  

        #region Three cyclic quadrilaterals on six points layout

        [Test]
        public void Test_Radical_Axis_Theorem()
        {
            // Create the template configuration's objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var C_ = new LooseConfigurationObject(Point);
            var D_ = new LooseConfigurationObject(Point);
            var E_ = new LooseConfigurationObject(Point);
            var F_ = new LooseConfigurationObject(Point);

            // Create the template configuration
            var templateConfiguration = Configuration.DeriveFromObjects(ThreeCyclicQuadrilatersOnSixPoints, A_, B_, C_, D_, E_, F_);

            // Create the template theorems
            var templateTheorems = new[]
            {
                new Theorem(templateConfiguration, ConcurrentObjects, new[]
                {
                    new LineTheoremObject(A_, B_),
                    new LineTheoremObject(C_, D_),
                    new LineTheoremObject(E_, F_)
                })
            };

            // Create the examined configuration's objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var H = new ConstructedConfigurationObject(Orthocenter, A, B, C);
            var H1 = new ConstructedConfigurationObject(ReflectionInLineFromPoints, H, A, B);
            var D = new ConstructedConfigurationObject(SecondIntersectionOfTwoCircumcircles, H, H1, A, B, C);

            // Create the examined configuration
            var examinedConfiguration = Configuration.DeriveFromObjects(ThreePoints, D);

            // Run
            var results = Run(examinedConfiguration, templateTheorems);

            // Check results
            CheckForEquivalncyOfResults(results, new[]
            {
                new SubtheoremsDeriverOutput
                {
                    DerivedTheorems = new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(examinedConfiguration, ConcurrentObjects, new[]
                        {
                            new LineTheoremObject(A, H1),
                            new LineTheoremObject(H, D),
                            new LineTheoremObject(B, C)
                        }),
                        templateTheorems[0])
                    },
                    UsedEqualities = new List<(ConfigurationObject originalObject, ConstructedConfigurationObject equalObject)>()
                }
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
                new Theorem(templateConfiguration, EqualAngles, new[]
                {
                    new AngleTheoremObject(new LineTheoremObject(A_, B_), new LineTheoremObject(A_, D_)),
                    new AngleTheoremObject(new LineTheoremObject(A_, C_), new LineTheoremObject(C_, D_))
                }),

                new Theorem(templateConfiguration, LineTangentToCircle, new TheoremObject[]
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
            var examinedConfiguration = Configuration.DeriveFromObjects(ThreePoints, E);

            // Run
            var results = Run(examinedConfiguration, templateTheorems);

            // Check results
            CheckForEquivalncyOfResults(results, new[]
            {
                new SubtheoremsDeriverOutput
                {
                    DerivedTheorems = new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(examinedConfiguration, EqualAngles, new[]
                        {
                            new AngleTheoremObject(new LineTheoremObject(A, B, E), new LineTheoremObject(B, C, D)),
                            new AngleTheoremObject(new LineTheoremObject(D, E), new LineTheoremObject(A, D))
                        }),
                        templateTheorems[0]),

                        (new Theorem(examinedConfiguration, LineTangentToCircle, new TheoremObject[]
                        {
                            new LineTheoremObject(A, D),
                            new CircleTheoremObject(B, D, E)
                        }), templateTheorems[1])
                    },
                    UsedEqualities = new List<(ConfigurationObject originalObject, ConstructedConfigurationObject equalObject)>()
                },
                new SubtheoremsDeriverOutput
                {
                    DerivedTheorems = new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(examinedConfiguration, EqualAngles, new[]
                        {
                            new AngleTheoremObject(new LineTheoremObject(A, B, E), new LineTheoremObject(A, D)),
                            new AngleTheoremObject(new LineTheoremObject(D, E), new LineTheoremObject(B, C, D))
                        }),
                        templateTheorems[0]),

                        (new Theorem(examinedConfiguration, LineTangentToCircle, new TheoremObject[]
                        {
                            new LineTheoremObject(B, C, D),
                            new CircleTheoremObject(A, D, E)
                        }), templateTheorems[1])
                    },
                    UsedEqualities = new List<(ConfigurationObject originalObject, ConstructedConfigurationObject equalObject)>()
                }
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
                new Theorem(templateConfiguration, TangentCircles, new TheoremObject[]
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
            var examinedConfiguration = Configuration.DeriveFromObjects(ThreePoints, D, E, F);

            // Run
            var results = Run(examinedConfiguration, templateTheorems);

            // Check results
            CheckThatResultsContain(results, new[]
            {
                new SubtheoremsDeriverOutput
                {
                    DerivedTheorems = new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(examinedConfiguration, TangentCircles, new TheoremObject[]
                        {
                            new CircleTheoremObject(A, C, D),
                            new CircleTheoremObject(A, E, F)
                        }),
                        templateTheorems[0])
                    },
                    UsedEqualities = new List<(ConfigurationObject originalObject, ConstructedConfigurationObject equalObject)>()
                }
            });
        }

        #endregion

        #region Line tangent to circle layout

        [Test]
        public void Test_Line_Tangent_To_Circle_Produces_Equal_Angles_In_Simple_Situation()
        {
            // Create the template configuration's objects
            var A_ = new LooseConfigurationObject(Point);
            var B_ = new LooseConfigurationObject(Point);
            var C_ = new LooseConfigurationObject(Point);
            var D_ = new LooseConfigurationObject(Point);

            // Create the template configuration
            var templateConfiguration = Configuration.DeriveFromObjects(CircleFromPointAndItsTangentLineAtOnePoint, A_, B_, C_, D_);

            // Create the template theorems
            var templateTheorems = new Theorem[]
            {
                new Theorem(templateConfiguration, EqualAngles, new[]
                {
                    new AngleTheoremObject(new LineTheoremObject(A_, D_), new LineTheoremObject(A_, B_)),
                    new AngleTheoremObject(new LineTheoremObject(C_, A_), new LineTheoremObject(C_, B_))
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
            var examinedConfiguration = Configuration.DeriveFromObjects(ThreePoints, E);

            // Run
            var results = Run(examinedConfiguration, templateTheorems);

            // Check results
            CheckForEquivalncyOfResults(results, new[]
            {
                new SubtheoremsDeriverOutput
                {
                    DerivedTheorems = new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(examinedConfiguration, EqualAngles, new[]
                        {
                            new AngleTheoremObject(new LineTheoremObject(E, A), new LineTheoremObject(A, B)),
                            new AngleTheoremObject(new LineTheoremObject(C, A), new LineTheoremObject(E, B, D, C))
                        }),
                        templateTheorems[0])
                    },
                    UsedEqualities = new List<(ConfigurationObject originalObject, ConstructedConfigurationObject equalObject)>()
                },

                new SubtheoremsDeriverOutput
                {
                    DerivedTheorems = new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(examinedConfiguration, EqualAngles, new[]
                        {
                            new AngleTheoremObject(new LineTheoremObject(E, A), new LineTheoremObject(A, C)),
                            new AngleTheoremObject(new LineTheoremObject(A, B), new LineTheoremObject(E, B, D, C))
                        }),
                        templateTheorems[0])
                    },
                    UsedEqualities = new List<(ConfigurationObject originalObject, ConstructedConfigurationObject equalObject)>()
                }
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
            var templateConfiguration = Configuration.DeriveFromObjects(CircleFromPointAndItsTangentLineAtOnePoint, A_, B_, C_, D_);

            // Create the template theorems
            var templateTheorems = new Theorem[]
            {
                new Theorem(templateConfiguration, EqualAngles, new[]
                {
                    new AngleTheoremObject(new LineTheoremObject(A_, D_), new LineTheoremObject(A_, B_)),
                    new AngleTheoremObject(new LineTheoremObject(C_, A_), new LineTheoremObject(C_, B_))
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
            var examinedConfiguration = Configuration.DeriveFromObjects(ThreePoints, D, E, F);

            // Run
            var results = Run(examinedConfiguration, templateTheorems);

            // Check results
            CheckThatResultsContain(results, new[]
            {
                new SubtheoremsDeriverOutput
                {
                    DerivedTheorems = new List<(Theorem derivedTheorem, Theorem templateTheorem)>
                    {
                        (new Theorem(examinedConfiguration, EqualAngles, new[]
                        {
                            new AngleTheoremObject(new LineTheoremObject(A, F), new LineTheoremObject(A, D)),
                            new AngleTheoremObject(new LineTheoremObject(B, C, D), new LineTheoremObject(D, E, F))
                        }),
                        templateTheorems[0])
                    },
                    UsedEqualities = new List<(ConfigurationObject originalObject, ConstructedConfigurationObject equalObject)>()
                }
            });
        }

        #endregion
    }
}

