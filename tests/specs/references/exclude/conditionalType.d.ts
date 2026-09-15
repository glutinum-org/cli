export type NonFunction<T> = Exclude<T, T extends () => void ? T : never>;
