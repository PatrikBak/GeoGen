import type { ProofStep } from "../../proof/stepSequence";

interface Props {
  steps: ProofStep[] | null;
  idx: number;
  isPlaying: boolean;
  onPrev: () => void;
  onPlayPause: () => void;
  onNext: () => void;
  onStop: () => void;
}

export function StepControls({ steps, idx, isPlaying, onPrev, onPlayPause, onNext, onStop }: Props) {
  if (!steps || steps.length === 0) return null;

  return (
    <div className="step-controls">
      <button type="button" onClick={onPrev} disabled={idx === 0}>
        ◀ Prev
      </button>
      <button type="button" onClick={onPlayPause}>
        {isPlaying ? "⏸ Pause" : "▶ Play"}
      </button>
      <button type="button" onClick={onNext} disabled={idx >= steps.length - 1}>
        Next ▶
      </button>
      <button type="button" onClick={onStop}>
        ✕ Stop
      </button>
      <span className="muted">
        {idx + 1}/{steps.length}
      </span>
    </div>
  );
}
