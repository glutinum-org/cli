export interface Stdio {
    command: string;
}

export interface Http {
    uri: string;
}

export type ServerDefinition = Stdio | Http;

export interface ServerDefinitionProvider<T extends ServerDefinition = ServerDefinition> {
    provide(): T[];
}
