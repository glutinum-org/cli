export interface Foo {
    a: string;
}

export type WithNumber = "auto" | number;

export type WithString = "black" | "red" | string;

export type WithInterface = "none" | Foo;

export type WithTypeLiteral = "none" | { x: string };

export type WithBooleanLiteralAndUndefined = "auto" | true | string[] | undefined;

export interface Props {
    colorScale?: "grayscale" | "blue" | string[];
}

export declare function scale(value: "grayscale" | "blue" | number): void;
