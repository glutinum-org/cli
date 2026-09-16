/** Options controlling the observer. */
export interface IntersectionOptions {
    /** The root element. */
    root?: string;
    threshold?: number;
    delay: number;
}

export declare function useInView(options?: IntersectionOptions): void;
