import { EventType } from "../event/EventType.js";

export declare class App {
    on(eventType: EventType): void;
    last(): EventType;
}
