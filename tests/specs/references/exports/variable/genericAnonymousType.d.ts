export interface Holder<T> {
    value: T;
}

export declare namespace ns {
    var make: {
        new <T>(value: T): Holder<T>;
        wrap<T>(value: T): Holder<T>;
    };
}
