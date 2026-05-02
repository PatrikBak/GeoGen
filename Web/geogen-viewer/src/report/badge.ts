/**
 * Map an inference rule type to its CSS badge class. Mirrors the names hardcoded in
 * `src/styles/report.css` so badges render with the right color background regardless of
 * which section emits them (proved theorems, proof tree, trace table).
 */
export function badgeClassFor(rule: string): string {
  switch (rule) {
    case "AssumedProven":
      return "badge assumed";
    case "TrivialTheorem":
      return "badge trivial";
    case "CustomRule":
      return "badge custom";
    case "ReformulatedTheorem":
      return "badge reform";
    case "EqualityTransitivity":
      return "badge transit";
    case "InferableFromSymmetry":
      return "badge sym";
    case "DefinableSimpler":
      return "badge simpler";
    default:
      return "badge other";
  }
}
