export type WithString = 1 | 2 | string;

export type WithTypeLiteralAndUndefined = 0 | 1 | { x: string } | undefined;

export interface Props {
    size?: 1 | 2 | number[];
}
