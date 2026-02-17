declare interface Thenable<T> {}

export function log<T extends string>(data: Thenable<T>) : void;
