// Should default to obj if the type query is not of a supported type
declare function log (): void

export type PluginFunc = (c: typeof log) => void
