using GeoGen.Core;
using GeoGen.Utilities;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// The default implementation of <see cref="IInferenceRuleManager"/> that gets
    /// all available <see cref="InferenceRule"/>s in an <see cref="InferenceRuleManagerData"/> object.
    /// </summary>
    public class InferenceRuleManager : IInferenceRuleManager
    {
        #region Private fields

        /// <summary>
        /// The dictionary mapping theorem types to inference rules that prove theorems of this type.
        /// </summary>
        private readonly Dictionary<TheoremType, List<InferenceRule>> _conclusionTypeToGeneralRules = new Dictionary<TheoremType, List<InferenceRule>>();

        /// <summary>
        /// The dictionary mapping theorem types to inference rules with their assumption of this type (one assumption per assumption group).
        /// </summary>
        private readonly Dictionary<TheoremType, List<(InferenceRule rule, Theorem assumptionTemplate)>> _assumptionTypeToGeneralRules = new Dictionary<TheoremType, List<(InferenceRule, Theorem)>>();

        /// <summary>
        /// The dictionary mapping constructions to inference rules with their declared object with this construction.
        /// </summary>
        private readonly Dictionary<Construction, List<(InferenceRule rule, ConstructedConfigurationObject objectTemplateTemplate)>> _constructionToObjectRules = new Dictionary<Construction, List<(InferenceRule, ConstructedConfigurationObject)>>();

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="InferenceRuleManager"/> class.
        /// </summary>
        /// <param name="data">The data for the manager containing <see cref="InferenceRule"/>s.</param>
        public InferenceRuleManager(InferenceRuleManagerData data)
        {
            // Ensure the data is not null
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            // Go through the rules
            foreach (var rule in data.Rules)
            {
                // If there are no declared objects, then it is a general rule
                if (rule.DeclaredObjects.IsEmpty())
                {
                    // Categorize it based on the type of the conclusion it proves
                    _conclusionTypeToGeneralRules.GetValueOrCreateNewAddAndReturn(rule.Conclusion.Type).Add(rule);

                    // Categorize it based on the assumptions. From each group we just
                    // need to take one assumption and it is not important which one.
                    foreach (var assumption in rule.AssumptionGroups.Select(group => group.First()))
                    {
                        // Get the right list for the type
                        _assumptionTypeToGeneralRules.GetValueOrCreateNewAddAndReturn(assumption.Type)
                            // Add the rule with the assumption 
                            .Add((rule, assumption));
                    }

                    // Move on
                    continue;
                }

                // If we got here, then we have an object rule
                // Go through the declared objects
                foreach (var declaredObject in rule.DeclaredObjects)
                {
                    // Get the right list for this construction
                    _constructionToObjectRules.GetValueOrCreateNewAddAndReturn(declaredObject.Construction)
                        // Add the rule with the object
                        .Add((rule, declaredObject));
                }
            }
        }

        #endregion

        #region IInferenceRuleManager implementation

        /// <inheritdoc/>
        public IEnumerable<InferenceRule> GetGeneralRulesThatProve(TheoremType theoremType)
            // Try to get it from the conclusion type dictionary or return an empty enumerable
            => _conclusionTypeToGeneralRules.GetValueOrDefault(theoremType) ?? Enumerable.Empty<InferenceRule>();

        /// <inheritdoc/>
        public IEnumerable<(InferenceRule rule, Theorem assumptionTemplate)> GetGeneralRulesWithAssumptionsOfType(TheoremType theoremType)
            // Try to get it from the assumption type dictionary or return an empty enumerable
            => _assumptionTypeToGeneralRules.GetValueOrDefault(theoremType) ?? Enumerable.Empty<(InferenceRule, Theorem)>();

        /// <inheritdoc/>
        public IEnumerable<(InferenceRule rule, ConstructedConfigurationObject objectTemplateTemplate)> GetObjectRulesWithObjectWithConstruction(Construction construction)
            // Try to get it from the construction to object dictionary or return an empty enumerable
            => _constructionToObjectRules.GetValueOrDefault(construction) ?? Enumerable.Empty<(InferenceRule, ConstructedConfigurationObject)>();

        #endregion
    }
}