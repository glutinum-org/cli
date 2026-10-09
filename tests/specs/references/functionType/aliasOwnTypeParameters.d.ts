export type Getter<TValue> = <TTValue = TValue>() => TTValue;
export type Handler = <T>(body: T) => void;
export type Reducer<S> = <A extends string = string>(state: S, action: A) => S;
export interface Cell<TValue> {
    getValue: Getter<TValue>;
    on: Handler;
    reduce: Reducer<TValue>;
}
