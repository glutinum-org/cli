/**
 * Called for each event
 */
export interface EventListener {
    (evt: Event): void;
}

export interface Mapper<T> {
    <U>(value: T, index: number): U;
}

export interface Overloaded {
    (value: string): number;
    (): number;
}

export interface WithProperty {
    (value: string): number;
    displayName: string;
}
