type Callback<T> = (event: T) => void;

type Observable<T> = {
    subscribe(callback: Callback<T>): void;
    waitUntil(predicate?: (event: T) => boolean): Promise<T>;
};

type Room<P> = {
    readonly id: string;
    readonly events: {
        readonly presence: Observable<P>;
        readonly connection: Observable<void>;
    };
};

export type OpaqueRoom = Room<string>;
