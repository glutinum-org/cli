export interface Context { id: number }

export declare function withThis(this: Context, count: number): void;

export interface Api {
    run(this: Context): void;
}
