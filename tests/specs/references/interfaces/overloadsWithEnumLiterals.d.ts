export declare enum SyntaxKind {
    SuperKeyword = 108,
    ThisKeyword = 110
}

export interface SuperExpression {
    readonly kind: SyntaxKind.SuperKeyword;
}

export interface ThisExpression {
    readonly kind: SyntaxKind.ThisKeyword;
}

export interface NodeFactory {
    createToken(token: SyntaxKind.SuperKeyword): SuperExpression;
    createToken(token: SyntaxKind.ThisKeyword): ThisExpression;
}
