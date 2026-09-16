export interface PluginModuleFactory {
    (mod: { typescript: string }, land?: number): string;
}

export type Quotes = "'" | "`";
