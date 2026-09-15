export type Geometry =
  | { kind: "point"; at: { x: number; y: number } }
  | { kind: "line"; from: { x: number }; to: { x: number } };
