export interface Options {
    required: number | string;
    trackVisibility?: boolean | string;
    delay?: number;
}

export declare function useInView(options: Options): void;
