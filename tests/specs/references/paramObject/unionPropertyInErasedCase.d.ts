export interface Text {
    length: number;
}

export type ChangeSpec = {
    from: number;
    insert?: string | Text;
} | readonly ChangeSpec[];

export interface TransactionSpec {
    changes?: ChangeSpec;
}

export declare function update(spec: TransactionSpec): void;
