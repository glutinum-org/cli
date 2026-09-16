export interface A { a: string }
export interface B { b: string }
export interface C { c: string }
export interface D { d: string }
export interface E { e: string }
export interface F { f: string }
export interface G { g: string }
export interface H { h: string }
export interface I { i: string }
export interface J { j: string }

export type Big = A | B | C | D | E | F | G | H | I | J;

export declare function f(value: A | B | C | D | E | F | G | H | I | J | undefined): void;
