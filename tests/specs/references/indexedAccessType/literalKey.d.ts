export interface CodeActionProviderMetadata {
    readonly documentation: ReadonlyArray<{ readonly kind: string; readonly command: string }>;
    readonly providedCodeActionKinds?: ReadonlyArray<string>;
}

export type Documentation = CodeActionProviderMetadata['documentation'];

export declare function ensure<T, K extends keyof T>(target: T, key: K): T[K];

export declare function asDocumentation(value: CodeActionProviderMetadata['documentation']): void;
