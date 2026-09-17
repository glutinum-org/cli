type DefaultEventMap = [never];

export type Key<K, T> = T extends DefaultEventMap ? string | symbol : K | keyof T;

export type Listener<K, T, F> = T extends DefaultEventMap ? F : K extends keyof T ? (...args: any[]) => void : never;

export type Listener1<K, T> = Listener<K, T, (...args: any[]) => void>;

// The conditional types are resolved with the default of `T`
export class EventEmitter<T = DefaultEventMap> {
    on<K>(eventName: Key<K, T>, listener: Listener1<K, T>): this;
    emit<K>(eventName: Key<K, T>, ...args: any[]): boolean;
}
