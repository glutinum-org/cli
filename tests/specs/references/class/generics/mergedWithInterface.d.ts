export interface EventMap {
    [key: string]: any[];
}

export interface EventEmitter<T extends EventMap = EventMap> {
    on(event: string): this;
}

export declare class EventEmitter<T extends EventMap = EventMap> {
    constructor();
    emit(event: string): boolean;
}
