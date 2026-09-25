export interface Context<E, P extends string> { path: P; }

export declare const validator: <
    P extends string,
    P2 extends string = P,
    VF extends (value: unknown, c: Context<any, P2>) => any = (value: unknown, c: Context<any, P2>) => any
>(target: string, validationFunc: VF) => void;
