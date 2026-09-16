export interface Event {
    type: string;
}

export type EventListener = (evt: Event) => void;

export interface EventListenerObject {
    handleEvent(object: Event): void;
}

export type EventListenerOrEventListenerObject = EventListener | EventListenerObject;

export interface AddEventListenerOptions {
    once?: boolean;
}

export interface EventTarget {
    addEventListener(type: string, callback: EventListenerOrEventListenerObject | null, options?: AddEventListenerOptions | boolean): void;
    // 27 combinations, too many: the last union stays
    tooMany(a: string | number | boolean, b: string | number | boolean, c: string | number | boolean): void;
}

export declare function write(chunk: string | Uint8Array, encoding?: string): void;

export declare class Reader {
    constructor(source: string | Uint8Array);
}
