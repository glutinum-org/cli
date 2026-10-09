export interface Internals<T> {
    input: T;
}
export interface Schema<Input> {
    parse(input: Input): Input;
}
export interface BaseString<T extends Internals<unknown>> extends Schema<T["input"]> {
    _zod: T;
}
export interface MiniString<Input> extends BaseString<Internals<Input>>, Schema<Input> {
}
