/**
 * Per-theorem-type colors used for the legend swatches. Mirror the CSS variables in
 * `src/styles/colors.css` exactly — keep the two in sync if a value changes.
 *
 * The fallback (#ff0033) matches `--color-default` and is used for any theorem type the
 * dumper produces that we haven't given a swatch — keeps the legend defensible against
 * schema growth.
 */
export const THEOREM_TYPE_COLORS: Record<string, string> = {
  ParallelLines: "#0088ff",
  PerpendicularLines: "#ff0033",
  EqualLineSegments: "#00cc44",
  CollinearPoints: "#aa00ff",
  ConcyclicPoints: "#00ccdd",
  ConcurrentLines: "#ff7700",
  TangentCircles: "#ff00aa",
  LineTangentToCircle: "#ff00aa",
  Incidence: "#777777",
  EqualObjects: "#cc6600",
};

export const DEFAULT_THEOREM_COLOR = "#ff0033";

export function colorForType(type: string): string {
  return THEOREM_TYPE_COLORS[type] ?? DEFAULT_THEOREM_COLOR;
}
