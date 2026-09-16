export interface WithMethodBase {
    run(): void;
}

export interface ExtendsMethodBase extends WithMethodBase {
    name: string;
}

export declare function f(options: ExtendsMethodBase): void;
