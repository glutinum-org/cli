interface Database {
    run(sql: string, callback?: (this: string, err: Error | null) => void): this;
}
