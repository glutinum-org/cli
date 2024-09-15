declare const NODES: readonly ["a", "button"];
type Primitives = {
    [E in (typeof NODES)[number]]: boolean;
};
