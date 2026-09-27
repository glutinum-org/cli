export interface FormDataIterator<T> extends IteratorObject<T, BuiltinIteratorReturn, unknown> {
    [Symbol.iterator](): FormDataIterator<T>;
}

export interface FormData {
    entries(): FormDataIterator<[string, string]>;
}
