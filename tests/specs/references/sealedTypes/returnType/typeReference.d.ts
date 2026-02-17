declare interface Thenable<T> {}

export function showInformationMessage<T extends string>(...items: T[]): Thenable<T | undefined>;
