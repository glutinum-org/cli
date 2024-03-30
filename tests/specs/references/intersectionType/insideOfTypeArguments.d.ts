export type RecordEntryObject = {
    v: string
    n: number
}

export type RecordEntryArrayItem = ReadonlyArray<
    RecordEntryObject & { i: number }
>
