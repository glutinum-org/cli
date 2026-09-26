export interface Picker {
    pick(path: [string, number]): void
    pickMany(paths: [string, number][], flag: boolean): void
}
