declare function openTextDocument(options?: {
    readonly encoding?: string;
    }
): any;

declare function openTextDocument(prefix: string, options?: {
    readonly permissions?: string[];
    }
): any;
