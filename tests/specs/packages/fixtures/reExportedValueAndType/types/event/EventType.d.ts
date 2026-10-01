export declare const EventType: {
    readonly START: "start";
    readonly END: "end";
};

export type EventType = (typeof EventType)[keyof typeof EventType];
