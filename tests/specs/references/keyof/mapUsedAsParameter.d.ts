export interface Options {
    animation?: number;
    group?: string;
    setData?: (dataTransfer: string, draggedElement: number) => void;
}

export declare function create(options?: Options): void;

export interface Sortable {
    option<K extends keyof Options>(name: K, value: Options[K]): void;
    option<K extends keyof Options>(name: K): Options[K];
}
