export interface Test {
    callback: ((params: {
        table: string;
    }) => void)
}
