using GeoGen.Core;
using System.Collections.Generic;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Represents a service that retrieves categorized <see cref="InferenceRule"/>s. 
    /// The main categorization is into two types of rules:
    /// <list type="number">
    /// <item>
    ///     <term>General rule</term>
    ///     <description><see cref="InferenceRule"/> that does not contain any <see cref="InferenceRule.DeclaredObjects"/>.</description>
    /// </item>
    /// <item>
    ///     <term>Object rule</term>
    ///     <description><see cref="InferenceRule"/> that contains some <see cref="InferenceRule.DeclaredObjects"/>.</description>
    /// </item>
    /// </list>
    /// </summary>
    public interface IInferenceRuleManager
    {
        /// <summary>
        /// Gets the general rules that prove theorems of a given type.
        /// </summary>
        /// <param name="theoremType">The theorem type.</param>
        /// <returns>The general rules that prove the given type.</returns>
        IEnumerable<InferenceRule> GetGeneralRulesThatProve(TheoremType theoremType);

        /// <summary>
        /// Gets the general rules that have an assumption of a given type. From each assumption group 
        /// (<see cref="InferenceRule.AssumptionGroups"/>) exactly one assumption is taken.
        /// </summary>
        /// <param name="theoremType">The theorem type.</param>
        /// <returns>
        /// The general rules with the assumption templates with the given type. It might contain one rule
        /// more times when it has more assumptions of the requested type. 
        /// </returns>
        IEnumerable<(InferenceRule rule, Theorem assumptionTemplate)> GetGeneralRulesWithAssumptionsOfType(TheoremType theoremType);

        /// <summary>
        /// Gets the object rules that use a declared object with a given construction.
        /// </summary>
        /// <param name="construction">The construction.</param>
        /// <returns>
        /// The object rules with the object rules with the given construction. It might contain one rule
        /// more times when this rule has more assumptions with the given construction.
        /// </returns>
        IEnumerable<(InferenceRule rule, ConstructedConfigurationObject objectTemplateTemplate)> GetObjectRulesWithObjectWithConstruction(Construction construction);
    }
}