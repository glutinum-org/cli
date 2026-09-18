export interface LayerGroup<P = any> {
    count: number;
}

export function layerGroup<P = any>(layers?: string[]): LayerGroup<P>;

export function empty<P = any>(): LayerGroup<P>;
