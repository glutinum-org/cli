export interface BaseOptions {
    debug?: boolean;
}

export interface ExtendedOptions extends BaseOptions {
    level: number;
}

export declare function configure(base: BaseOptions, extended: ExtendedOptions): void;
