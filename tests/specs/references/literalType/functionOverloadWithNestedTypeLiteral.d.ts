declare function openTextDocument(options?: {
    readonly encoding?: string;
    readonly clamp: {
        readonly min: number;
        readonly max: number;
    }
}
): any;

declare function openTextDocument(prefix: string, options?: {
    readonly encoding?: string;
    readonly clamp: {
        readonly min: number;
    }
}
): any;
