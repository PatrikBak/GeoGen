using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.TheoremFinder;
using GeoGen.TheoremProver.InferenceRuleProvider;
using GeoGen.TheoremProver.ObjectIntroductionRuleProvider;
using GeoGen.Utilities;
using Ninject;
using System.Diagnostics;
using static GeoGen.Core.ComposedConstructions;
using static GeoGen.Core.ConfigurationObjectType;
using static GeoGen.Core.LooseObjectLayout;
using static GeoGen.Core.PredefinedConstructions;

namespace GeoGen.TheoremProver.IntegrationTest
{
    /// <summary>
    /// The entry class of the application.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The entry method of the application.
        /// </summary>
        /// <param name="arguments">The three arguments:
        /// <list type="number">
        /// <item>Path to the inference rule folder.</item>
        /// <item>The extension of the inference rule files.</item>
        /// <item>Path to the object introduction rule file.</item>
        /// </list> 
        /// </param>
        private static async Task Main(string[] arguments)
        {
            #region Kernel preparation

            // Prepare the settings for the inference rule provider
            var inferenceRuleProviderSettings = new InferenceRuleProviderSettings(ruleFolderPath: arguments[0], fileExtension: arguments[1]);

            // Prepare the settings for the object introduction rule provider
            var objectIntroductionRuleProviderSettings = new ObjectIntroductionRuleProviderSettings(filePath: arguments[2]);

            // Prepare the kernel
            var kernel = Infrastructure.NinjectUtilities.CreateKernel()
                // That constructors configurations
                .AddConstructor()
                // That can find theorems
                .AddTheoremFinder(new TheoremFindingSettings
                                  (
                                      // Look for theorems of any type
                                      soughtTheoremTypes: Enum.GetValues(typeof(TheoremType)).Cast<TheoremType>()
                                          // Except for the EqualObjects that don't have a finder
                                          .Except(TheoremType.EqualObjects.ToEnumerable())
                                          // Enumerate
                                          .ToArray(),

                                      // Exclude in-picture tangencies
                                      new TangentCirclesTheoremFinderSettings(excludeTangencyInsidePicture: true),
                                      new LineTangentToCircleTheoremFinderSettings(excludeTangencyInsidePicture: true)
                                  ))
                // That can prove theorems
                .AddTheoremProver(new TheoremProvingSettings
                (
                    // Use the provider to find the inference rules
                    new InferenceRuleManagerData(await new InferenceRuleProvider.InferenceRuleProvider(inferenceRuleProviderSettings).GetInferenceRulesAsync()),

                    // Use the provider to find the object introduction rules
                    new ObjectIntroducerData(await new ObjectIntroductionRuleProvider.ObjectIntroductionRuleProvider(objectIntroductionRuleProviderSettings).GetObjectIntroductionRulesAsync()),

                    // Setup the prover
                    new TheoremProverSettings
                    (
                        // We will be strict and don't assume simplifiable theorems
                        assumeThatSimplifiableTheoremsAreTrue: false,

                        // We will find trivial theorems for all objects
                        findTrivialTheoremsOnlyForLastObject: false
                    )
                ));

            #endregion

            #region Tests

            // Set up a collecting tracer that the prover will feed inference events to. We rebind the
            // IInferenceTracer binding (defaulted to EmptyInferenceTracer in AddTheoremProver) to a
            // singleton CollectingInferenceTracer so events captured by the prover are visible here.
            var inferenceTracer = new CollectingInferenceTracer();
            kernel.Rebind<IInferenceTracer>().ToConstant(inferenceTracer);

            // Prepare a JSON reports folder next to the binary. The standalone web viewer in
            // Web/geogen-viewer/ consumes this output. We clear it on each run so stale reports
            // never linger.
            var reportsFolder = Path.Combine(AppContext.BaseDirectory, "json-reports");
            if (Directory.Exists(reportsFolder))
                Directory.Delete(reportsFolder, recursive: true);
            Directory.CreateDirectory(reportsFolder);
            var manifestEntries = new List<Json.ManifestEntry>();

            // Take the tests
            new (string Name, Configuration Configuration)[]
            {
                (nameof(PerpendicularBisectorsAreConcurrent), PerpendicularBisectorsAreConcurrent()),
                (nameof(IncenterAndTangentLine), IncenterAndTangentLine()),
                (nameof(Midpoints), Midpoints()),
                (nameof(Parallelogram), Parallelogram()),
                (nameof(HiddenExcenter), HiddenExcenter()),
                (nameof(HiddenMidpoint), HiddenMidpoint()),
                (nameof(LineTangentToCircle), LineTangentToCircle()),
                (nameof(ConcurrencyViaObjectIntroduction), ConcurrencyViaObjectIntroduction()),
                (nameof(SimpleLineSegments), SimpleLineSegments()),
                (nameof(IncenterMedianConcurrency), IncenterMedianConcurrency()),
                (nameof(IncenterContactPoints), IncenterContactPoints()),
                (nameof(OrthicTriangle), OrthicTriangle()),
                (nameof(OrthicTriangleConcyclic), OrthicTriangleConcyclic()),
                (nameof(CentroidReflection), CentroidReflection()),
                (nameof(CentroidParallelogramMidpoint), CentroidParallelogramMidpoint()),
                (nameof(CentroidIsoscelesTrapezoid), CentroidIsoscelesTrapezoid()),
                (nameof(CircumcenterReflection), CircumcenterReflection()),
                (nameof(CircumcenterParallelogram), CircumcenterParallelogram()),
                (nameof(CircumcenterOrthocenter), CircumcenterOrthocenter()),
                (nameof(CentroidParallelogramMidpointWithM), CentroidParallelogramMidpointWithM()),
                (nameof(TrapezoidParallelogramSecondIntersection1), TrapezoidParallelogramSecondIntersection1()),
                (nameof(ReflectionMidpointTangentCircle), ReflectionMidpointTangentCircle()),
            }
            // Perform each
            .ForEach(scenario =>
            {
                var (scenarioName, configuration) = scenario;
                #region Finding theorems

                // Prepare 3 pictures in which the configuration is drawn
                var pictures = kernel.Get<IGeometryConstructor>().ConstructWithUniformLayout(configuration, numberOfPictures: 3).pictures;

                // Prepare a contextual picture
                var contextualPicture = new ContextualPicture(pictures);

                // Find all theorems
                var theorems = kernel.Get<ITheoremFinder>().FindAllTheorems(contextualPicture);

                #endregion

                #region Writing theorems

                // Prepare the formatter of all the output
                var formatter = new OutputFormatter(configuration.AllObjects);

                // Prepare a local function that converts given theorems to a string
                string TheoremString(IEnumerable<Theorem> theorems) =>
                    // If there are no theorems
                    theorems.IsEmpty()
                        // Then return an indication of it 
                        ? "nothing"
                        // Otherwise format each theorem
                        : theorems.Select(formatter.FormatTheorem)
                            // Order alphabetically
                            .Ordered()
                            // Add the index
                            .Select((theoremString, index) => $"[{index + 1}] {theoremString}")
                            // Make each on a separate line
                            .ToJoinedString("\n");

                // Write the configuration and theorems
                Console.WriteLine($"\nConfiguration:\n\n{formatter.FormatConfiguration(configuration).Indent(2)}\n");
                Console.WriteLine($"Theorems:\n\n{TheoremString(theorems.AllObjects).Indent(2)}\n");

                #endregion

                #region Proving theorems

                // Prepare a timer
                var totalTime = new Stopwatch();

                // Start it
                totalTime.Start();

                // Perform the theorem finding with proofs, without any assumed theorems
                var proverOutput = kernel.Get<ITheoremProver>().ProveTheoremsAndConstructProofs(new TheoremMap(), theorems, contextualPicture);

                // Stop the timer
                totalTime.Stop();

                #endregion

                #region Writing results

                // Get the proofs
                var proofString = proverOutput
                    // Sort by the statement
                    .OrderBy(pair => formatter.FormatTheorem(pair.Key))
                    // Format each
                    .Select(pair => formatter.FormatTheoremProof(pair.Value))
                    // Trim
                    .Select(proofString => proofString.Trim())
                    // Make an empty line between each
                    .ToJoinedString("\n\n");

                // Write it
                Console.WriteLine(proofString);

                // Write the unproven theorems too
                Console.WriteLine($"\nUnproved:\n\n{TheoremString(theorems.AllObjects.Except(proverOutput.Keys)).Indent(2)}\n");

                // Report time
                Console.WriteLine($"Total time: {totalTime.ElapsedMilliseconds}");
                Console.WriteLine("----------------------------------------------");

                #endregion

                #region JSON report

                // Pull this scenario's trace events out of the collector. DrainSession matches by
                // configuration reference, so we never accidentally pick up another scenario's events.
                var session = inferenceTracer.DrainSession(configuration);

                // Build the diagram from the first of the three constructed pictures. Picture 0 is
                // the one used for theorem finding; using it here means the diagram coordinates
                // match the analytical state the prover saw.
                Diagram.DiagramModel diagram = null;
                try
                {
                    diagram = Diagram.DiagramBuilder.Build(pictures.First(), formatter, configuration, theorems.AllObjects);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"  (diagram build failed: {e.Message})");
                }

                // Serialize the scenario into the v1 JSON schema. The exporter handles id assignment,
                // theorem deduplication, and proof-tree flattening; we just hand it the live objects.
                var unproved = theorems.AllObjects.Except(proverOutput.Keys).ToArray();
                var manifestEntry = Json.JsonReportExporter.WriteScenario(
                    outputFolder: reportsFolder,
                    scenarioName: scenarioName,
                    configuration: configuration,
                    formatter: formatter,
                    proofs: proverOutput,
                    unprovedTheorems: unproved,
                    trace: session,
                    elapsedMs: totalTime.ElapsedMilliseconds,
                    diagram: diagram);
                Console.WriteLine($"JSON report: {Path.Combine(reportsFolder, manifestEntry.File)}");

                manifestEntries.Add(manifestEntry);

                #endregion
            });

            // Write the top-level manifest linking each per-scenario file.
            Json.JsonReportExporter.WriteManifest(reportsFolder, manifestEntries);
            Console.WriteLine($"Manifest: {Path.Combine(reportsFolder, "manifest.json")}");

            #endregion
        }

        #region Test configurations


        private static Configuration PerpendicularBisectorsAreConcurrent()
        {
            // Create objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var l1 = new ConstructedConfigurationObject(PerpendicularBisector, B, C);
            var l2 = new ConstructedConfigurationObject(PerpendicularBisector, C, A);
            var l3 = new ConstructedConfigurationObject(PerpendicularBisector, A, B);

            // Return the configuration
            return Configuration.DeriveFromObjects(Triangle, A, B, C, l1, l2, l3);
        }


        private static Configuration IncenterAndTangentLine()
        {
            // Create objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var l1 = new ConstructedConfigurationObject(InternalAngleBisector, B, A, C);
            var l2 = new ConstructedConfigurationObject(InternalAngleBisector, C, A, B);
            var D = new ConstructedConfigurationObject(IntersectionOfLines, l1, l2);
            var E = new ConstructedConfigurationObject(TangentLine, D, B, C);

            // Return the configuration
            return Configuration.DeriveFromObjects(Triangle, A, B, C, D, E);
        }

        private static Configuration Midpoints()
        {
            // Create objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Midpoint, A, B);
            var E = new ConstructedConfigurationObject(Midpoint, B, C);
            var F = new ConstructedConfigurationObject(Midpoint, C, A);

            // Return the configuration
            return Configuration.DeriveFromObjects(Triangle, D, E, F);
        }

        private static Configuration Parallelogram()
        {
            // Create objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(PointReflection, B, A);
            var E = new ConstructedConfigurationObject(PointReflection, C, A);

            // Return the configuration
            return Configuration.DeriveFromObjects(Triangle, D, E);
        }

        private static Configuration HiddenExcenter()
        {
            // Create objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Incenter, A, B, C);
            var E = new ConstructedConfigurationObject(OppositePointOnCircumcircle, D, B, C);

            // Return the configuration
            return Configuration.DeriveFromObjects(Triangle, E);
        }

        private static Configuration HiddenMidpoint()
        {
            // Create objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Median, A, B, C);
            var E = new ConstructedConfigurationObject(IntersectionOfLineAndLineFromPoints, D, B, C);

            // Return the configuration
            return Configuration.DeriveFromObjects(Triangle, D, E);
        }

        private static Configuration LineTangentToCircle()
        {
            // Create objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Incenter, A, B, C);
            var E = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, D, B, C);
            var F = new ConstructedConfigurationObject(PointReflection, E, D);
            var G = new ConstructedConfigurationObject(ReflectionInLineFromPoints, F, A, D);

            // Return the configuration
            return Configuration.DeriveFromObjects(Triangle, A, B, C, D, E, F, G);
        }

        private static Configuration ConcurrencyViaObjectIntroduction()
        {
            // Create objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Orthocenter, A, B, C);
            var E = new ConstructedConfigurationObject(Circumcenter, B, C, D);
            var F = new ConstructedConfigurationObject(ParallelogramPoint, A, B, C);
            var G = new ConstructedConfigurationObject(Incenter, B, C, E);

            // Return the configuration
            return Configuration.DeriveFromObjects(Triangle, A, B, C, D, E, F, G);
        }

        private static Configuration SimpleLineSegments()
        {
            // Create objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var l = new ConstructedConfigurationObject(TangentLine, A, B, C);
            var D = new ConstructedConfigurationObject(PerpendicularProjection, B, l);
            var E = new ConstructedConfigurationObject(PerpendicularProjection, C, l);
            var F = new ConstructedConfigurationObject(Midpoint, B, D);
            var G = new ConstructedConfigurationObject(Midpoint, C, E);

            // Return the configuration
            return Configuration.DeriveFromObjects(Triangle, A, B, C, D, E, F, G);
        }

        /// <summary>
        /// Triangle ABC with the incenter and the three contact points — i.e., the incircle's
        /// touchpoints on each side, expressed as perpendicular projections from the incenter.
        /// This is the canonical "incenter + contact triangle" setup that GeoGen's iteration phase
        /// uses as a starting point for problem generation; here we just expose the initial state
        /// to the prover and see what falls out.
        /// </summary>
        private static Configuration IncenterContactPoints()
        {
            // Create objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            // Auto-naming: incenter renders as "D", projections as "E", "F", "G".
            var I = new ConstructedConfigurationObject(Incenter, A, B, C);
            var E = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, I, B, C);
            var F = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, I, A, C);
            var G = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, I, A, B);

            // Return the configuration
            return Configuration.DeriveFromObjects(Triangle, A, B, C, I, E, F, G);
        }

        /// <summary>
        /// Triangle ABC with the incenter, the three contact points (perpendicular projections from
        /// the incenter onto each side), and the median from A. The interesting theorem on this
        /// configuration is that the line through the incenter and one contact point, the line
        /// through the other two contact points, and the median from A are concurrent.
        /// </summary>
        private static Configuration IncenterMedianConcurrency()
        {
            // Create objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            // Auto-naming labels these in declaration order: incenter becomes "D" in rendered output,
            // the three projections become "E", "F", "G", and the median (only line) becomes "l".
            var I = new ConstructedConfigurationObject(Incenter, A, B, C);
            var E = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, I, B, C);
            var F = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, I, A, C);
            var G = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, I, A, B);
            var l = new ConstructedConfigurationObject(Median, A, B, C);

            // Return the configuration
            return Configuration.DeriveFromObjects(Triangle, A, B, C, I, E, F, G, l);
        }

        /// <summary>
        /// Triangle ABC with the three altitude feet — i.e., perpendicular projections of each
        /// vertex onto the opposite side. Together D, E, F form the orthic triangle.
        /// </summary>
        private static Configuration OrthicTriangle()
        {
            // Create objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, A, B, C);
            var E = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, B, A, C);
            var F = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, C, A, B);

            // Return the configuration
            return Configuration.DeriveFromObjects(Triangle, A, B, C, D, E, F);
        }

        /// <summary>
        /// Orthic triangle (D, E, F as altitude feet) plus G = circumcenter of (A, E, F).
        /// The interesting theorem on this configuration is that D, E, F, G are concyclic.
        /// </summary>
        private static Configuration OrthicTriangleConcyclic()
        {
            // Create objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, A, B, C);
            var E = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, B, A, C);
            var F = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, C, A, B);
            var G = new ConstructedConfigurationObject(Circumcenter, A, E, F);

            // Return the configuration
            return Configuration.DeriveFromObjects(Triangle, A, B, C, D, E, F, G);
        }

        /// <summary>
        /// Triangle ABC with centroid G, then D = reflection of A over line BC, and E = centroid
        /// of triangle BCD.
        /// </summary>
        private static Configuration CentroidReflection()
        {
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var G = new ConstructedConfigurationObject(Centroid, A, B, C);
            var D = new ConstructedConfigurationObject(ReflectionInLineFromPoints, A, B, C);
            var E = new ConstructedConfigurationObject(Centroid, B, C, D);
            return Configuration.DeriveFromObjects(Triangle, A, B, C, G, D, E);
        }

        /// <summary>
        /// Triangle ABC with centroid G, then D = parallelogram point of (A, B, G), and
        /// E = midpoint of CD.
        /// </summary>
        private static Configuration CentroidParallelogramMidpoint()
        {
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var G = new ConstructedConfigurationObject(Centroid, A, B, C);
            var D = new ConstructedConfigurationObject(ParallelogramPoint, A, B, G);
            var E = new ConstructedConfigurationObject(Midpoint, C, D);
            return Configuration.DeriveFromObjects(Triangle, A, B, C, G, D, E);
        }

        /// <summary>
        /// Triangle ABC with centroid G, then D = isosceles-trapezoid point built on (A, B, C),
        /// and E = centroid of triangle BCD.
        /// </summary>
        private static Configuration CentroidIsoscelesTrapezoid()
        {
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var G = new ConstructedConfigurationObject(Centroid, A, B, C);
            var D = new ConstructedConfigurationObject(IsoscelesTrapezoidPoint, A, B, C);
            var E = new ConstructedConfigurationObject(Centroid, B, C, D);
            return Configuration.DeriveFromObjects(Triangle, A, B, C, G, D, E);
        }

        /// <summary>
        /// Triangle ABC with circumcenter O, then D = reflection of A in line BC, and E =
        /// circumcenter of triangle BCD.
        /// </summary>
        private static Configuration CircumcenterReflection()
        {
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var O = new ConstructedConfigurationObject(Circumcenter, A, B, C);
            var D = new ConstructedConfigurationObject(ReflectionInLineFromPoints, A, B, C);
            var E = new ConstructedConfigurationObject(Circumcenter, B, C, D);
            return Configuration.DeriveFromObjects(Triangle, A, B, C, O, D, E);
        }

        /// <summary>
        /// Triangle ABC with circumcenter O, then D = parallelogram point of A on {B, C}, and
        /// E = circumcenter of triangle BCD.
        /// </summary>
        private static Configuration CircumcenterParallelogram()
        {
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var O = new ConstructedConfigurationObject(Circumcenter, A, B, C);
            var D = new ConstructedConfigurationObject(ParallelogramPoint, A, B, C);
            var E = new ConstructedConfigurationObject(Circumcenter, B, C, D);
            return Configuration.DeriveFromObjects(Triangle, A, B, C, O, D, E);
        }

        /// <summary>
        /// Same as <see cref="CentroidParallelogramMidpoint"/> but with M = Midpoint(B, C)
        /// declared explicitly. The centroid construction implicitly involves Midpoint(B, C)
        /// (since the A-median passes through it), but the prover only sees it after deriving
        /// it from the construction; declaring it up-front exposes it as a first-class object
        /// other rules can hook into.
        /// </summary>
        private static Configuration CentroidParallelogramMidpointWithM()
        {
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Centroid, A, B, C);
            var E = new ConstructedConfigurationObject(ParallelogramPoint, A, B, D);
            var F = new ConstructedConfigurationObject(Midpoint, C, E);
            var M = new ConstructedConfigurationObject(Midpoint, B, C);
            return Configuration.DeriveFromObjects(Triangle, A, B, C, D, E, F, M);
        }

        /// <summary>
        /// Triangle ABC, D = isosceles-trapezoid point of A on {B, C}, E = parallelogram
        /// point of A on {B, C}, F = second intersection of circumcircles ABE and BCE… —
        /// expressed as <c>SecondIntersectionOfTwoCircumcircles(E, {{A, D}, {B, C}})</c>.
        /// </summary>
        private static Configuration TrapezoidParallelogramSecondIntersection1()
        {
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(IsoscelesTrapezoidPoint, A, B, C);
            var E = new ConstructedConfigurationObject(ParallelogramPoint, A, B, C);
            var F = new ConstructedConfigurationObject(SecondIntersectionOfTwoCircumcircles, E, A, D, B, C);
            return Configuration.DeriveFromObjects(Triangle, A, B, C, D, E, F);
        }

        /// <summary>
        /// Triangle ABC with circumcenter O and orthocenter D, and E = circumcenter of
        /// triangle ABD.
        /// </summary>
        private static Configuration CircumcenterOrthocenter()
        {
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var O = new ConstructedConfigurationObject(Circumcenter, A, B, C);
            var D = new ConstructedConfigurationObject(Orthocenter, A, B, C);
            var E = new ConstructedConfigurationObject(Circumcenter, A, B, D);
            return Configuration.DeriveFromObjects(Triangle, A, B, C, O, D, E);
        }

        /// <summary>
        /// Triangle ABC with D = midpoint of AB, E = reflection of C across AB,
        /// F = foot of perpendicular from E onto line CD, and circle c centered at C
        /// with radius |EF|. Top-ranked LineTangentToCircle theorem from the
        /// CircleWithRadius construction survey: line DE is tangent to c.
        /// </summary>
        private static Configuration ReflectionMidpointTangentCircle()
        {
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Midpoint, A, B);
            var E = new ConstructedConfigurationObject(ReflectionInLineFromPoints, C, A, B);
            var F = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, E, C, D);
            var c = new ConstructedConfigurationObject(CircleWithRadius, C, E, F);
            return Configuration.DeriveFromObjects(Triangle, A, B, C, D, E, F, c);
        }

        #endregion
    }
}