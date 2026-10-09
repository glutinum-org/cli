export interface Table<T> {
    name: string;
    get(key: string): Promise<T>;
}
export interface Dexie {
    tables: Table<any>[];
    Table: {
        prototype: Table<any>;
    };
    table(name: string): Table<any>;
}
export interface Transaction {
    db: Dexie;
    table(name: string): Table<any>;
}
