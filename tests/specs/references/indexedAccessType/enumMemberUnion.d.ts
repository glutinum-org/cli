export declare enum SyntaxKind {
    ImportKeyword = 102,
    NewKeyword = 105
}

export interface MetaProperty {
    readonly keywordToken: SyntaxKind.NewKeyword | SyntaxKind.ImportKeyword;
}

export declare function createMetaProperty(keywordToken: MetaProperty["keywordToken"]): void;
