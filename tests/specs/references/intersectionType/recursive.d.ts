export interface Event {
    readonly bubbles: boolean;
}

export declare class ProgressEvent {
    __proto__: Event & ProgressEvent;
    readonly loaded: number;
}
