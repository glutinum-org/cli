export type TypeLiteral = | { kind: "point"; x: number };

export type FunctionType = | ((a: string, b: number) => void);

export interface Props {
    shape: | { kind: "point"; x: number };
}
