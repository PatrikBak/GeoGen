import { useCallback, useRef } from "react";
import { highlightController } from "./HighlightController";

/**
 * Hook used by every clickable row (theorem, proof step, config token, unproved item, to-prove
 * item, trace event). Returns event handlers + a `setRef` callback the caller attaches to the
 * row's outermost element.
 *
 * Highlight semantics: a row hover/click affects only the SVG diagram, not other rows. The
 * controller used to also light up rows that shared object ids with the hovered row, which made
 * hovering "ConcurrentLines: A,B,C" flash every other row mentioning A, B, or C — a visual
 * storm. Now we only update the diagram, and the clicked row gets a `selected` class on
 * itself for direct "I'm pinned" feedback.
 */
export function useRowHighlight(
  ids: readonly string[],
  theoremType: string | null,
  label: string,
) {
  // Capture latest values without re-subscribing the ref callback. The callback should run only
  // when the element identity changes, not on every render.
  const idsRef = useRef(ids);
  const typeRef = useRef(theoremType);
  const labelRef = useRef(label);
  idsRef.current = ids;
  typeRef.current = theoremType;
  labelRef.current = label;

  const elRef = useRef<HTMLElement | null>(null);

  const setRef = useCallback((el: HTMLElement | null) => {
    elRef.current = el;
  }, []);

  const onMouseEnter = useCallback(() => {
    if (highlightController.isAnimationActive()) return;
    highlightController.setPreview({ ids: new Set(idsRef.current), theoremType: typeRef.current });
    highlightController.setStatus(`Preview: ${labelRef.current}`);
  }, []);

  const onMouseLeave = useCallback(() => {
    if (highlightController.isAnimationActive()) return;
    highlightController.setPreview(null);
    // Clear the status text only if no row is currently sticky-selected. Otherwise the user
    // would lose the "Highlighted: …" label by mousing away, which is confusing.
    const selectedExists = document.querySelector("[data-row].selected") !== null;
    if (!selectedExists) {
      highlightController.setStatus("Hover or click a theorem to highlight its objects.");
    }
  }, []);

  const onClick = useCallback((e: React.MouseEvent) => {
    const target = e.target as HTMLElement;
    if (target.closest("[data-no-row-click]")) return;
    const innerRow = target.closest("[data-row]");
    const ourRow = e.currentTarget as HTMLElement;
    if (innerRow && innerRow !== ourRow && ourRow.contains(innerRow)) return;
    if (highlightController.isAnimationActive()) return;

    const layer = { ids: new Set(idsRef.current), theoremType: typeRef.current };

    // Toggle: if already selected, deselect everything; otherwise pin this row + diagram layer.
    if (highlightController.isSelectedRow(elRef.current)) {
      highlightController.setSelectedRow(null);
      highlightController.clearSticky();
      highlightController.setStatus("Hover or click a theorem to highlight its objects.");
    } else {
      highlightController.setSelectedRow(elRef.current);
      // toggleSticky compares by content, so we can't blindly call it twice without it reverting.
      // Instead, set the sticky layer directly: clear then "toggle" applies it.
      highlightController.clearSticky();
      highlightController.toggleSticky(layer);
      highlightController.setPreview(null);
      highlightController.setStatus(`Highlighted: ${labelRef.current}`);
    }
  }, []);

  return { onMouseEnter, onMouseLeave, onClick, setRef };
}

/**
 * Diagram-element ref. Diagram points/lines/segments/circles register themselves with the
 * controller so layer changes (preview/sticky/step-active) update their classNames directly.
 */
export function useDiagramElementRef(elementIds: readonly string[]): (el: SVGElement | null) => void {
  const idsRef = useRef(elementIds);
  idsRef.current = elementIds;
  const elRef = useRef<SVGElement | null>(null);

  return useCallback((el: SVGElement | null) => {
    if (el === elRef.current) return;
    if (elRef.current) highlightController.unregisterDiagramElement(elRef.current);
    elRef.current = el;
    if (el) {
      highlightController.registerDiagramElement(el, idsRef.current);
    }
  }, []);
}
