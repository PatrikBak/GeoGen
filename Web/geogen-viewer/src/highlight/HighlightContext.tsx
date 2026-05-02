import { useSyncExternalStore, type ReactNode } from "react";
import { highlightController } from "./HighlightController";

/**
 * Compatibility wrapper retained so existing call sites that wrap the report in a Provider keep
 * working. The Provider used to fan out preview/sticky/step-active state via React context to
 * thousands of consumers — that's now replaced by an imperative controller in
 * `HighlightController.ts`. The Provider here does nothing functional; it's just a no-op
 * container so we don't have to touch every caller.
 */
export function HighlightProvider({ children }: { children: ReactNode }) {
  return <>{children}</>;
}

/**
 * Subscribe to the controller's status text. This is the ONE thing the diagram aside still
 * needs from React: a re-render when the status string changes. By using
 * `useSyncExternalStore`, only this single component re-renders on status changes — not the
 * entire row tree the way the old context-driven hook did.
 */
export function useStatusText(): string {
  return useSyncExternalStore(
    (cb) => highlightController.subscribeStatus(cb),
    () => highlightController.getStatus(),
    () => highlightController.getStatus(),
  );
}
