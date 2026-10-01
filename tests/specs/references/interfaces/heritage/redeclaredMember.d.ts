export interface Base {
    name: string;
    find(selector: string, options?: { deep?: boolean }): Base;
    on<K extends keyof BaseEventMap>(type: K, listener: (ev: BaseEventMap[K]) => void): void;
}

export interface BaseEventMap {
    change: string;
}

export interface Derived extends Base {
    find(selector: string, options?: { deep?: boolean; limit?: number }): Derived;
}

export interface DerivedEventMap extends BaseEventMap {
    click: number;
}

export interface Sibling extends Base {
    on<K extends keyof DerivedEventMap>(type: K, listener: (ev: DerivedEventMap[K]) => void): void;
}
