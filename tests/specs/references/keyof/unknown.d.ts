export type RowData = unknown;

export interface AccessorKeyColumnDefBase {
    id?: string;
    accessorKey: keyof RowData;
}
