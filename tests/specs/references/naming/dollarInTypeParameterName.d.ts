export interface Box<$T> {
    value: $T
}

export declare function unwrap<$T>(box: Box<$T>): $T;
