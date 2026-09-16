/// <reference types="node" />

import type { Stats } from "fs";

export interface Widget {
    element: HTMLElement;
    stats: Stats;
    onClick(listener: EventListener): void;
    open(): Promise<Response>;
}

export declare class Panel extends EventTarget {
    render(): void;
}
