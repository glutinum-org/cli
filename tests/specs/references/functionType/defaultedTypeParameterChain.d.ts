export interface Context<E, P extends string> { path: P; }

export declare const a: <P2 extends string = string>(c: Context<any, P2>) => void;
export declare const b: <P extends string, P2 extends string = P>(c: Context<any, P2>) => void;
