import { useCallback, useEffect, useState } from "react";
import { ReportPage } from "./report/ReportPage";
import { HighlightProvider } from "./highlight/HighlightContext";
import { ManifestView } from "./upload/ManifestView";
import { UploadZone } from "./upload/UploadZone";
import type { UploadedReports } from "./types";

export function App() {
  const [uploaded, setUploaded] = useState<UploadedReports | null>(null);
  // Bumped on every URL change so render is recomputed without us tracking the parsed query
  // string in state. Using a counter keeps the dep array honest.
  const [urlVersion, setUrlVersion] = useState(0);

  // Sync with browser back/forward: re-render when the user pops between scenarios.
  useEffect(() => {
    const onPop = () => setUrlVersion((v) => v + 1);
    window.addEventListener("popstate", onPop);
    return () => window.removeEventListener("popstate", onPop);
  }, []);

  const selectedScenario = readScenarioFromUrl();

  const handleSelect = useCallback((name: string) => {
    const url = new URL(window.location.href);
    url.searchParams.set("scenario", name);
    window.history.pushState({}, "", url);
    setUrlVersion((v) => v + 1);
  }, []);

  const handleBack = useCallback(() => {
    const url = new URL(window.location.href);
    url.searchParams.delete("scenario");
    window.history.pushState({}, "", url);
    setUrlVersion((v) => v + 1);
  }, []);

  const handleClear = useCallback(() => {
    const url = new URL(window.location.href);
    url.searchParams.delete("scenario");
    window.history.replaceState({}, "", url);
    setUploaded(null);
    setUrlVersion((v) => v + 1);
  }, []);

  // Reference urlVersion to satisfy exhaustive-deps without a real dependency: each bump invalidates.
  void urlVersion;

  if (uploaded === null) {
    return (
      <div className="app">
        <UploadZone onUploaded={setUploaded} />
      </div>
    );
  }

  if (selectedScenario && uploaded.scenarios.has(selectedScenario)) {
    const report = uploaded.scenarios.get(selectedScenario)!;
    return (
      <div className="app">
        <HighlightProvider>
          <ReportPage report={report} onBack={handleBack} />
        </HighlightProvider>
      </div>
    );
  }

  return (
    <div className="app">
      <ManifestView uploaded={uploaded} onSelect={handleSelect} onClear={handleClear} />
    </div>
  );
}

function readScenarioFromUrl(): string | null {
  if (typeof window === "undefined") return null;
  const url = new URL(window.location.href);
  return url.searchParams.get("scenario");
}
