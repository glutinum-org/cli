export interface RowData {
    test : string
}

export interface AccessorKeyColumnDefBase {
    accessorKey: keyof RowData;
}
