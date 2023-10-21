interface ConfigTypeMap {
    default: string | number;
}

export type ConfigType = ConfigTypeMap[keyof ConfigTypeMap];
