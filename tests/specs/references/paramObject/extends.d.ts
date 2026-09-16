export interface BaseOptions {
    debug?: boolean;
    level: string | number;
}

export interface PlainExtends extends BaseOptions {
    name: string;
}

export interface PartialExtends extends Partial<BaseOptions> {
    name: string;
}

export interface OmitExtends extends Omit<BaseOptions, "debug"> {
    name: string;
}

export interface Narrows extends Partial<BaseOptions> {
    level: string;
}

export declare function a(options: PlainExtends): void;
export declare function b(options: PartialExtends): void;
export declare function c(options: OmitExtends): void;
export declare function d(options: Narrows): void;
