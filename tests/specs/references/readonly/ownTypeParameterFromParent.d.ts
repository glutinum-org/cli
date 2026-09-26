export interface Registry<Value> {
    config: Readonly<{
        value: Value
    }>
}
