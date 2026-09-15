export type Event =
  | { type: "click"; x: number; y: number; target?: string }
  | { type: "key-down"; key: string }
  | { type: "blur" };
