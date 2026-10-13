export type FormatRelativeFn = <DateType extends Date>(token: string, date: DateType) => string;
export type Mapper<T> = <U>(value: T) => U;
