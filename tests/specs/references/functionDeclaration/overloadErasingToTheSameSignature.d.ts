export type Dim = string | number

export interface Static {
    isEmpty<T>(value?: T): boolean;
    isEmpty(value?: any): boolean;
    pick<T>(values: T[], dims: Dim[]): void;
    pick<T>(values: T[], dims: Array<Dim>): void;
}
