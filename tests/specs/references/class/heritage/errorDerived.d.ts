export declare class CancellationError extends Error {
}

export declare class LSPCancellationError extends CancellationError {
    readonly data: string;
}
