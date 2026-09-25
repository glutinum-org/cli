export class Logger {
    log(value: string, prefix?: string): void
    log(value: string, prefix?: string, time?: Boolean): void

    // The shorter one takes calls the longer one does not: it is kept
    warn(value: string, code?: number): void
    warn(value: string, code: number, fatal: boolean): void

    // A different return type is a different API: both are kept
    read(path: string): string
    read(path: string, encoding?: string): number
}
