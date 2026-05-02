import { useMemo } from "react";
import type { DiagramT } from "../../types";
import { useDiagramElementRef } from "../../highlight/useHighlight";

interface Props {
  diagram: DiagramT;
}

const PIXEL_SIZE = 480;

/**
 * Render the configuration's geometry as an inline SVG. Y-axis is flipped at render time
 * (world y-up → SVG y-down) by negating Y on every coordinate write — keeps text labels
 * right-side-up without an explicit transform.
 *
 * Each element registers itself with the imperative highlight controller via a `ref` callback.
 * The controller toggles class names directly on the DOM, so a hover anywhere in the page
 * doesn't re-render any of these components.
 */
export function DiagramSvg({ diagram }: Props) {
  if (!diagram) {
    return <p className="muted">(diagram unavailable for this scenario)</p>;
  }

  const { bounds } = diagram;
  const width = bounds.maxX - bounds.minX;
  const height = bounds.maxY - bounds.minY;
  const viewBox = `${bounds.minX} ${-bounds.maxY} ${width} ${height}`;
  const diagonal = Math.sqrt(width * width + height * height);
  const pointR = diagonal * 0.008;
  const labelOffset = diagonal * 0.018;
  const labelFontSize = diagonal * 0.035;

  return (
    <svg
      className="diagram"
      viewBox={viewBox}
      width={PIXEL_SIZE}
      height={PIXEL_SIZE}
      preserveAspectRatio="xMidYMid meet"
      xmlns="http://www.w3.org/2000/svg"
    >
      <g className="diagram-strokes">
        {diagram.lines.map((line) => (
          <DiagramLine key={line.id} line={line} />
        ))}
        {diagram.segments.map((seg, i) => (
          <DiagramSegment key={seg.ids.join(",") + ":" + i} seg={seg} />
        ))}
        {diagram.circles.map((c) => (
          <DiagramCircle key={c.id} circle={c} />
        ))}
      </g>
      <g className="diagram-points">
        {diagram.points.map((p) => (
          <DiagramPoint
            key={p.id}
            point={p}
            pointR={pointR}
            labelOffset={labelOffset}
            fontSize={labelFontSize}
          />
        ))}
      </g>
    </svg>
  );
}

function DiagramLine({ line }: { line: NonNullable<DiagramT>["lines"][number] }) {
  const ids = useMemo(() => [line.id], [line.id]);
  const setRef = useDiagramElementRef(ids);
  return (
    <line
      ref={setRef}
      data-object-id={line.id}
      className="diagram-line"
      x1={line.x1}
      y1={-line.y1}
      x2={line.x2}
      y2={-line.y2}
      vectorEffect="non-scaling-stroke"
    >
      <title>{line.label}</title>
    </line>
  );
}

function DiagramSegment({ seg }: { seg: NonNullable<DiagramT>["segments"][number] }) {
  const setRef = useDiagramElementRef(seg.ids);
  return (
    <line
      ref={setRef}
      data-object-id={seg.ids.join(",")}
      className="diagram-segment"
      x1={seg.x1}
      y1={-seg.y1}
      x2={seg.x2}
      y2={-seg.y2}
      vectorEffect="non-scaling-stroke"
    >
      <title>{seg.label}</title>
    </line>
  );
}

function DiagramCircle({ circle }: { circle: NonNullable<DiagramT>["circles"][number] }) {
  const ids = useMemo(() => [circle.id], [circle.id]);
  const setRef = useDiagramElementRef(ids);
  return (
    <circle
      ref={setRef}
      data-object-id={circle.id}
      className="diagram-circle"
      cx={circle.cx}
      cy={-circle.cy}
      r={circle.r}
      vectorEffect="non-scaling-stroke"
      fill="none"
    >
      <title>{circle.label}</title>
    </circle>
  );
}

function DiagramPoint({
  point,
  pointR,
  labelOffset,
  fontSize,
}: {
  point: NonNullable<DiagramT>["points"][number];
  pointR: number;
  labelOffset: number;
  fontSize: number;
}) {
  const ids = useMemo(() => [point.id], [point.id]);
  const setCircleRef = useDiagramElementRef(ids);
  const setTextRef = useDiagramElementRef(ids);
  return (
    <>
      <circle
        ref={setCircleRef}
        data-object-id={point.id}
        className="diagram-point"
        cx={point.x}
        cy={-point.y}
        r={pointR}
        vectorEffect="non-scaling-stroke"
      >
        <title>{point.label}</title>
      </circle>
      <text
        ref={setTextRef}
        data-object-id={point.id}
        className="diagram-label"
        x={point.x + labelOffset}
        y={-point.y - labelOffset}
        fontSize={fontSize}
      >
        {point.label}
      </text>
    </>
  );
}
