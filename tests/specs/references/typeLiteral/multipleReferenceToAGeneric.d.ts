export interface WorkspaceConfiguration {
    inspect<T>(section: string): {
        key: string;
        defaultValue?: T;
        globalValue?: T;
    }
}
