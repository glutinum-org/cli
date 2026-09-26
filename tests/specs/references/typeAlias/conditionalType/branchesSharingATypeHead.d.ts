export interface Destination<T> {
    run(): Promise<T>
}

export type Result<S> = S extends Destination<infer P> ? Promise<P> : Promise<void>

export declare function pipeline<B>(destination: B): Result<B>;
