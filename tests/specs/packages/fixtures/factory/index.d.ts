import type { Hidden } from "./plugins/hidden";

export { default as animator } from "./core/animator";
export * from "./plugins/index";

export interface Instance {
    run(hidden?: Hidden): void;
}

declare const _instanceFactory: (args?: ReadonlyArray<string> | string) => Instance;

export default _instanceFactory;
