export interface BlockquoteToken {
    type: "blockquote";
}
export interface CodespanToken {
    type: "codespan";
}
export interface DefinitionToken {
    type: "def";
}
export interface EmphasisToken {
    type: "em";
}
export interface HeadingToken {
    type: "heading";
}
export interface ParagraphToken {
    type: "paragraph";
}
export interface StrongToken {
    type: "strong";
}
export declare function lex(src: string): BlockquoteToken | CodespanToken | DefinitionToken | EmphasisToken | HeadingToken | ParagraphToken | StrongToken;
export type Lexed = ReturnType<typeof lex>;
