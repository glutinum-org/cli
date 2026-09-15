export type OtherKeys<T, K extends keyof T> = Exclude<keyof T, K>;
