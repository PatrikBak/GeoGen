import { useMemo } from "react";
import type { ArgumentNodeT, ConfigurationT, ConstructedObjectT } from "../types";
import { useRowHighlight } from "../highlight/useHighlight";

interface Props {
  configuration: ConfigurationT;
}

/**
 * Render the configuration block as a series of clickable lines. Each line and each individual
 * object name is independently clickable: clicking a name highlights just that one object on the
 * diagram; clicking the line highlights every object the line references.
 */
export function ConfigurationSection({ configuration }: Props) {
  // Build an objectId → label lookup once so argument tokens don't have to scan on every render.
  const labels = useMemo(() => {
    const m = new Map<string, string>();
    for (const o of configuration.looseObjects) m.set(o.id, o.label);
    for (const o of configuration.constructed) m.set(o.id, o.label);
    return m;
  }, [configuration]);

  return (
    <section>
      <h2>Configuration</h2>
      <div className="config">
        <LayoutLine configuration={configuration} />
        {configuration.constructed.map((c) => (
          <ConstructedLine key={c.id} object={c} labels={labels} />
        ))}
      </div>
    </section>
  );
}

function LayoutLine({ configuration }: { configuration: ConfigurationT }) {
  const ids = useMemo(() => configuration.looseObjects.map((o) => o.id), [configuration]);
  const labelText = `${configuration.layout}: ${configuration.looseObjects.map((o) => o.label).join(", ")}`;
  const { onMouseEnter, onMouseLeave, onClick, setRef } = useRowHighlight(ids, null, labelText);

  return (
    <div
      ref={setRef}
      data-row=""
      className="config-line"
      onMouseEnter={onMouseEnter}
      onMouseLeave={onMouseLeave}
      onClick={onClick}
    >
      <span className="config-keyword">{configuration.layout}</span>: {configuration.looseObjects.map((o, i) => (
        <span key={o.id}>
          {i > 0 ? ", " : null}
          <ObjectToken id={o.id} label={o.label} />
        </span>
      ))}
    </div>
  );
}

function ConstructedLine({ object, labels }: { object: ConstructedObjectT; labels: Map<string, string> }) {
  const ids = useMemo(() => [object.id, ...object.referencedObjectIds], [object]);
  const labelText = `${object.label} = ${object.constructionName}(...)`;
  const { onMouseEnter, onMouseLeave, onClick, setRef } = useRowHighlight(ids, null, labelText);

  return (
    <div
      ref={setRef}
      data-row=""
      className="config-line"
      onMouseEnter={onMouseEnter}
      onMouseLeave={onMouseLeave}
      onClick={onClick}
    >
      <ObjectToken id={object.id} label={object.label} /> ={" "}
      <span className="config-construction">{object.constructionName}</span>
      (
      {object.arguments.map((arg, i) => (
        <span key={i}>
          {i > 0 ? ", " : null}
          <ArgumentToken arg={arg} labels={labels} />
        </span>
      ))}
      )
    </div>
  );
}

function ArgumentToken({ arg, labels }: { arg: ArgumentNodeT; labels: Map<string, string> }) {
  if (arg.kind === "object" && arg.objectId) {
    const label = labels.get(arg.objectId) ?? arg.objectId;
    return <ObjectToken id={arg.objectId} label={label} />;
  }
  if (arg.kind === "set" && arg.items) {
    return (
      <>
        {"{"}
        {arg.items.map((child, i) => (
          <span key={i}>
            {i > 0 ? ", " : null}
            <ArgumentToken arg={child} labels={labels} />
          </span>
        ))}
        {"}"}
      </>
    );
  }
  return null;
}

function ObjectToken({ id, label }: { id: string; label: string }) {
  const ids = useMemo(() => [id], [id]);
  const { onMouseEnter, onMouseLeave, onClick, setRef } = useRowHighlight(ids, null, label);
  return (
    <span
      ref={setRef as (el: HTMLSpanElement | null) => void}
      data-row=""
      className="config-token"
      onMouseEnter={onMouseEnter}
      onMouseLeave={onMouseLeave}
      onClick={onClick}
    >
      {label}
    </span>
  );
}
