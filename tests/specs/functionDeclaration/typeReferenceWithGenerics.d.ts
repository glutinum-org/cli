export type PluginFunc<T = unknown> = () => void

export function extend<T>(plugin: PluginFunc<T>): void
