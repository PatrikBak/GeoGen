using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using static GeoGen.Core.TheoremType;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Represents a class that communicates with an <see cref="IInferenceRuleManager"/> and schedules usages of <see cref="InferenceRule"/>s
    /// in a sophisticated way so that they are used only if they have a chance to prove something new. These schedules are returned in
    /// <see cref="ScheduleData"/> objects that correspond to the data part of <see cref="InferenceRuleApplierInput"/>.
    /// <para>
    /// There are three methods that can add new schedules: <see cref="PerformInitialScheduling(IEnumerable{Theorem}, Configuration)"/>,
    /// <see cref="ScheduleAfterProving(Theorem)"/>, <see cref="ScheduleAfterDiscoveringObject(ConstructedConfigurationObject)"/>.
    /// The scheduled data are then retrieved via <see cref="NextScheduledData"/>.
    /// </para>
    /// <para>
    /// The scheduler uses separate queues for general rules and object rules. General rules are always preferred over object ones when it 
    /// comes to getting the next scheduled data. The idea behind this decision is that general rules generally prove more theorems (surprisingly).
    /// </para>
    /// <para>
    /// The scheduler also supports invalidation of schedules via <see cref="InvalidateObject(ConstructedConfigurationObject)"/> and 
    /// <see cref="InvalidateTheorem(Theorem)"/> methods. This is useful when some object or theorem is no longer in its normal form
    /// (for more information see the documentation of <see cref="NormalizationHelper"/>).
    /// </para>
    /// </summary>
    public class Scheduler
    {
        #region Dependencies

        /// <summary>
        /// The provider of inference rules that provides them categorized.
        /// </summary>
        private readonly IInferenceRuleManager _manager;

        #endregion

        #region Private fields

        /// <summary>
        /// The queue for general rules (for the definition see the documentation of <see cref="IInferenceRuleManager"/>).
        /// </summary>
        private readonly Queue<ScheduleData> _generalDataQueue = new Queue<ScheduleData>();

        /// <summary>
        /// The queue for object rules (for the definition see the documentation of <see cref="IInferenceRuleManager"/>).
        /// </summary>
        private readonly Queue<ScheduleData> _objectDataQueue = new Queue<ScheduleData>();

        /// <summary>
        /// The set of schedule data that have been scheduled but are no longer valid.
        /// </summary>
        private readonly HashSet<ScheduleData> _invalidatedData = new HashSet<ScheduleData>();

        /// <summary>
        /// The dictionary mapping assumption types to object rules (for the definition see the documentation of 
        /// <see cref="IInferenceRuleManager"/>) that have been scheduled before. The idea is to re-schedule 
        /// these rules when a new theorem is proved and it might be a missing assumption.
        /// </summary>
        private readonly Dictionary<TheoremType, List<(Theorem assumptionTemplate, ScheduleData data)>> _assumptionTypeToObjectData = new Dictionary<TheoremType, List<(Theorem, ScheduleData)>>();

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Scheduler"/> class.
        /// </summary>
        /// <param name="manager">The provider of inference rules that provides them categorized.</param>
        public Scheduler(IInferenceRuleManager manager)
        {
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Performs initial scheduling for a given configuration and given list of theorems to prove. This is done in the following way:
        /// <list type="number">
        /// <item>General rules that with premapped conclusions to the theorems to prove.</item>
        /// <item>General rules that prove equalities (theorems with type <see cref="EqualObjects"/>).</item>
        /// <item>Object rules for all objects.</item>
        /// </list>
        /// </summary>
        /// <param name="theoremsToProve">The unproven theorems that we want to prove.</param>
        /// <param name="configuration">The configuration where the unproven theorems hold.</param>
        public void PerformInitialScheduling(IEnumerable<Theorem> theoremsToProve, Configuration configuration)
        {
            #region General rules that prove our theorems

            // Go through all the theorems to prove
            foreach (var theoremToprove in theoremsToProve)
            {
                // For a given theorem take the rules that prove it
                foreach (var rule in _manager.GetGeneralRulesThatProve(theoremToprove.Type))
                {
                    // Prepare a data with the premapped conclusion
                    var data = new ScheduleData(rule, premappedConclusion: theoremToprove);

                    // Schedule it
                    _generalDataQueue.Enqueue(data);
                }
            }

            #endregion

            #region General rule that prove equalities

            // Go through the general rules that prove an equality
            foreach (var rule in _manager.GetGeneralRulesThatProve(EqualObjects))
            {
                // Prepare a data
                var data = new ScheduleData(rule);

                // Schedule it
                _generalDataQueue.Enqueue(data);
            }

            #endregion

            #region Object rules for the configuration's objects

            // Schedule object rules for every object
            // NOTE: Going backwards might help a bit because it is more likely that newer objects 
            //       will yield something useful than the older ones.
            foreach (var constructedObject in configuration.ConstructedObjects.Reverse())
            {
                // Go through the rules that use this an object with the same construction and introduce objects
                foreach (var (rule, templateObject) in _manager.GetObjectRulesWithObjectWithConstruction(constructedObject.Construction))
                {
                    // Prepare a data with the premapped object
                    var data = new ScheduleData(rule, premappedObject: (templateObject, constructedObject));

                    // Schedule it
                    _objectDataQueue.Enqueue(data);

                    // Categorize it based on assumption types
                    CategorizeObjectDataBasedOnAssumptionTypes(data);
                }
            }

            #endregion
        }

        /// <summary>
        /// Performs scheduling of everything relevant that can use a given proven theorem as a premapped assumption.
        /// </summary>
        /// <param name="provedTheorem">The proved theorem that should be used as a premapped assumption.</param>
        public void ScheduleAfterProving(Theorem provedTheorem)
        {
            #region Schedule general rules 

            // Go through the general theorems with an assumption of this type
            foreach (var (rule, assumption) in _manager.GetGeneralRulesWithAssumptionsOfType(provedTheorem.Type))
            {
                // Prepare a data with the premapped assumption
                var newData = new ScheduleData(rule, premappedAssumption: (assumption, provedTheorem));

                // Schedule it
                _generalDataQueue.Enqueue(newData);
            }

            #endregion

            #region Schedule saved object rules

            // Take the object rules that have an assumption of the current type
            _assumptionTypeToObjectData.GetValueOrDefault(provedTheorem.Type)
                // Do the scheduling for every data and every assumption template
                ?.ForEach(pair =>
                {
                    // Deconstruct
                    var (assumptionTemplate, oldObjectData) = pair;

                    // Reconstruct the data so that they keep the premapped object and also include the premapped assumption
                    var newData = new ScheduleData(oldObjectData.InferenceRule, oldObjectData.PremappedObject, premappedAssumption: (assumptionTemplate, provedTheorem));

                    // Schedule it
                    _objectDataQueue.Enqueue(newData);
                });

            #endregion
        }

        /// <summary>
        /// Performs scheduling of everything relevant that can use a given new object as a premapped object.
        /// </summary>
        /// <param name="newObject">The object that should be premapped in new schedules.</param>
        public void ScheduleAfterDiscoveringObject(ConstructedConfigurationObject newObject)
        {
            // Go through the relevant object rules
            foreach (var (rule, templateObject) in _manager.GetObjectRulesWithObjectWithConstruction(newObject.Construction))
            {
                // Prepare a data with the premapped object
                var newData = new ScheduleData(rule, premappedObject: (templateObject, newObject));

                // Schedule this data
                _objectDataQueue.Enqueue(newData);

                // Categorize it based on assumption types
                CategorizeObjectDataBasedOnAssumptionTypes(newData);
            }
        }

        /// <summary>
        /// Returns the next <see cref="ScheduleData"/> according to the scheduling algorithms.
        /// </summary>
        /// <returns>The next scheduled data; or null, if nothing is scheduled.</returns>
        public ScheduleData NextScheduledData()
        {
            // Try until we find something...
            while (true)
            {
                // Prepare the data that is going to be scheduled
                ScheduleData scheduledData = null;

                // If there is a scheduled general rule, then we will use it
                if (_generalDataQueue.Any())
                    scheduledData = _generalDataQueue.Dequeue();
                // Otherwise if there is a scheduled object-specific rule, then it's its turn
                else if (_objectDataQueue.Any())
                    scheduledData = _objectDataQueue.Dequeue();

                // If nothing is scheduled, we're done
                if (scheduledData == null)
                    return null;

                // If the data is no longer valid, keep looking
                if (_invalidatedData.Contains(scheduledData))
                    continue;

                // If we got here, the data is okay
                return scheduledData;
            }
        }

        /// <summary>
        /// Invalidates every rule that has a given object premapped. 
        /// </summary>
        /// <param name="constructedObject">The object that is no longer valid.</param>
        public void InvalidateObject(ConstructedConfigurationObject constructedObject)
        {
            // Go through the object rules
            _objectDataQueue
                // That have this object premapped
                .Where(data => constructedObject.Equals(data.PremappedObject?.realObject))
                // // Add each to the set of invalidated data
                .ForEach(data => _invalidatedData.Add(data));

            // Go through the saved object data categorized by assumptions
            _assumptionTypeToObjectData.Values
                // Ensure all that contain the object are removed
                .ForEach(list => list.RemoveAll(pair => constructedObject.Equals(pair.data.PremappedObject?.realObject)));
        }

        /// <summary>
        /// Invalidates every rule that has a given theorem premapped as an assumption or conclusion.
        /// </summary>
        /// <param name="theorem">The theorem that is no longer valid.</param>
        public void InvalidateTheorem(Theorem theorem)
            // Go through the general rules
            => _generalDataQueue
                // As well as object rules
                .Concat(_objectDataQueue)
                // That have a premapped assumption equal to this theorem
                .Where(data => data.PremappedAssumption?.realAssumption.Equals(theorem)
                    // Or a premapped conclusion equal to this theorem
                    ?? theorem.Equals(data.PremappedConclusion))
                // Add each to the set of invalidated data
                .ForEach(data => _invalidatedData.Add(data));

        #endregion

        #region Private methods

        /// <summary>
        /// Categorizes a given object data based on assumption templates it contains and adds
        /// this information to <see cref="_assumptionTypeToObjectData"/>.
        /// </summary>
        /// <param name="data">The object data rule.</param>
        private void CategorizeObjectDataBasedOnAssumptionTypes(ScheduleData data)
            // From every assumption group
            => data.InferenceRule.AssumptionGroups
                // Take any theorem
                .Select(group => group.First())
                // And handle it
                .ForEach(assumption =>
                {
                    // Get the right list for the assumption 
                    _assumptionTypeToObjectData.GetOrAdd(assumption.Type, () => new List<(Theorem, ScheduleData)>())
                        // Add the data together with the assumption
                        .Add((assumption, data));
                });

        #endregion
    }
}