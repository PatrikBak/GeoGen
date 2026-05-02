/**
 * Imperative highlight controller. The controller holds preview/sticky/step-active state in
 * module-level refs and applies className changes directly to subscribed DOM elements — no
 * React re-renders fan out through the row tree on hover.
 *
 * Scope of "what gets highlighted":
 *   - Diagram elements (points, lines, segments, circles, labels) register themselves and react
 *     to layer changes.
 *   - Rows (theorem items, trace rows, config tokens, etc.) DO NOT register. They have hover/click
 *     handlers that drive the controller's layers, but they don't react to overlapping id sets.
 *     Why: hovering a theorem that mentions "A" used to light up every other row mentioning "A"
 *     — config tokens, trace rows, to-prove items — which created a visual storm. The user wants
 *     hover to affect only the diagram.
 *   - The single currently-clicked row gets a `selected` class for direct "I'm pinned" feedback,
 *     applied only to the clicked element. No id-overlap matching for rows.
 */

export interface HighlightLayer {
  ids: ReadonlySet<string>;
  theoremType: string | null;
}

interface ElementInfo {
  /** Highlight-ids for this element, stored as a Set for O(1) overlap checks. */
  ids: Set<string>;
  /** The CSS classes the controller has currently applied to the element. */
  applied: string[];
}

const EMPTY_LAYER: HighlightLayer = { ids: new Set(), theoremType: null };

class HighlightControllerImpl {
  private preview: HighlightLayer = EMPTY_LAYER;
  private sticky: HighlightLayer = EMPTY_LAYER;
  private stepActive: HighlightLayer = EMPTY_LAYER;

  /** Diagram elements (and ONLY diagram elements) that should react to layer changes. */
  private idToElements = new Map<string, Set<HTMLElement | SVGElement>>();
  private elementToInfo = new WeakMap<HTMLElement | SVGElement, ElementInfo>();

  /** The single row the user has clicked-to-pin. Carries a `selected` class. */
  private selectedRow: HTMLElement | null = null;

  private status = "Hover or click a theorem to highlight its objects.";
  private statusListeners = new Set<() => void>();

  // ---------- Diagram-element registration ----------

  registerDiagramElement(
    el: HTMLElement | SVGElement,
    ids: readonly string[],
  ): void {
    const info: ElementInfo = { ids: new Set(ids), applied: [] };
    this.elementToInfo.set(el, info);
    for (const id of info.ids) {
      let set = this.idToElements.get(id);
      if (!set) {
        set = new Set();
        this.idToElements.set(id, set);
      }
      set.add(el);
    }
    this.refreshElement(el, info);
  }

  unregisterDiagramElement(el: HTMLElement | SVGElement): void {
    const info = this.elementToInfo.get(el);
    if (!info) return;
    for (const id of info.ids) {
      const set = this.idToElements.get(id);
      if (!set) continue;
      set.delete(el);
      if (set.size === 0) this.idToElements.delete(id);
    }
    this.elementToInfo.delete(el);
  }

  // ---------- Selected-row tracking ----------

  /**
   * Mark a single row as the "pinned" selection. The previous selection (if any) is unselected
   * before the new one is marked, so at most one row carries the `selected` class at a time.
   */
  setSelectedRow(el: HTMLElement | null): void {
    if (this.selectedRow === el) return;
    if (this.selectedRow) this.selectedRow.classList.remove("selected");
    this.selectedRow = el;
    if (el) el.classList.add("selected");
  }

  /** Whether the given element is the current selected row. */
  isSelectedRow(el: HTMLElement | null): boolean {
    return el !== null && this.selectedRow === el;
  }

  // ---------- Layer mutations (diagram only) ----------

  setPreview(layer: HighlightLayer | null): void {
    const next = layer ?? EMPTY_LAYER;
    const prev = this.preview;
    this.preview = next;
    this.applyLayerChange(prev, next);
  }

  toggleSticky(layer: HighlightLayer): void {
    const prev = this.sticky;
    const next = sameLayer(prev, layer) ? EMPTY_LAYER : layer;
    this.sticky = next;
    this.applyLayerChange(prev, next);
  }

  clearSticky(): void {
    if (this.sticky === EMPTY_LAYER) return;
    const prev = this.sticky;
    this.sticky = EMPTY_LAYER;
    this.applyLayerChange(prev, EMPTY_LAYER);
  }

  setStepActive(layer: HighlightLayer | null): void {
    const next = layer ?? EMPTY_LAYER;
    const prev = this.stepActive;
    this.stepActive = next;
    this.applyLayerChange(prev, next);
  }

  isAnimationActive(): boolean {
    return this.stepActive.ids.size > 0;
  }

  // ---------- Status ----------

  getStatus(): string {
    return this.status;
  }

  setStatus(text: string): void {
    if (this.status === text) return;
    this.status = text;
    for (const fn of this.statusListeners) fn();
  }

  subscribeStatus(fn: () => void): () => void {
    this.statusListeners.add(fn);
    return () => this.statusListeners.delete(fn);
  }

  // ---------- Internals ----------

  /**
   * Find every diagram element whose id set intersects either the old or new layer, then
   * recompute its className. Avoids a full sweep — only elements whose state is actually
   * changing get touched.
   */
  private applyLayerChange(oldLayer: HighlightLayer, newLayer: HighlightLayer): void {
    const touched = new Set<HTMLElement | SVGElement>();
    for (const id of oldLayer.ids) {
      const set = this.idToElements.get(id);
      if (set) for (const el of set) touched.add(el);
    }
    for (const id of newLayer.ids) {
      const set = this.idToElements.get(id);
      if (set) for (const el of set) touched.add(el);
    }
    for (const el of touched) {
      const info = this.elementToInfo.get(el);
      if (!info) continue;
      this.refreshElement(el, info);
    }
  }

  /**
   * Recompute the highlight classes for one diagram element. Reads the controller's current
   * preview/sticky/stepActive layers and diffs the resulting class list against what's on the
   * element so we only touch what changed.
   */
  private refreshElement(el: HTMLElement | SVGElement, info: ElementInfo): void {
    const preview = this.preview;
    const sticky = this.sticky;
    const stepActive = this.stepActive;

    const inPreview = anyOverlap(preview.ids, info.ids);
    const inSticky = anyOverlap(sticky.ids, info.ids);
    const inStep = anyOverlap(stepActive.ids, info.ids);

    const next: string[] = [];
    if (inPreview) next.push("preview");
    if (inSticky) next.push("highlight");
    if (inStep) next.push("step-active");
    const colorType = inStep
      ? null
      : inSticky
        ? sticky.theoremType
        : inPreview
          ? preview.theoremType
          : null;
    if (colorType) next.push(`color-${colorType}`);

    const prev = info.applied;
    for (const c of prev) if (!next.includes(c)) el.classList.remove(c);
    for (const c of next) if (!prev.includes(c)) el.classList.add(c);
    info.applied = next;
  }
}

function sameLayer(a: HighlightLayer, b: HighlightLayer): boolean {
  if (a.theoremType !== b.theoremType) return false;
  if (a.ids.size !== b.ids.size) return false;
  for (const id of a.ids) if (!b.ids.has(id)) return false;
  return true;
}

function anyOverlap(a: ReadonlySet<string>, b: ReadonlySet<string>): boolean {
  if (a.size === 0 || b.size === 0) return false;
  const [small, large] = a.size < b.size ? [a, b] : [b, a];
  for (const v of small) if (large.has(v)) return true;
  return false;
}

/** Module-singleton instance used everywhere in the app. */
export const highlightController = new HighlightControllerImpl();
