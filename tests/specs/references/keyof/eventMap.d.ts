export interface MouseEvent {
    x: number;
}

export interface KeyboardEvent {
    key: string;
}

export interface ElementEventMap {
    "click": MouseEvent;
    "keydown": KeyboardEvent;
}

export interface Element {
    addEventListener<K extends keyof ElementEventMap>(type: K, listener: (this: Element, ev: ElementEventMap[K]) => any): void;
    addEventListener(type: string, listener: (ev: object) => any): void;
}

export interface TagNameMap {
    "a": Element;
}

export interface Document {
    createElement<K extends keyof TagNameMap>(tagName: K): TagNameMap[K];
    createElement(tagName: string): Element;
}

export interface GlobalEventMap {
    "focus": KeyboardEvent;
}

// The keys of a map include the inherited ones
export interface InputEventMap extends GlobalEventMap {
    "input": MouseEvent;
}

export interface Input {
    addEventListener<K extends keyof InputEventMap>(type: K, listener: (ev: InputEventMap[K]) => any): void;
}

// A map declared as a type alias
export type StreamEvents = {
    close: () => void;
    data: (chunk: string) => void;
};

export interface Stream {
    on<K extends keyof StreamEvents>(event: K, listener: StreamEvents[K]): this;
}

export interface CustomEvents {
    custom: () => void;
}

// A map declared as an intersection
export type SocketEvents = {
    end: () => void;
} & CustomEvents;

export interface Socket {
    on<K extends keyof SocketEvents>(event: K, listener: SocketEvents[K]): this;
}
