interface ConfigTypeMap {
    methodA: string | number;
    methodB: boolean;
    methodC: boolean;
}

export type ConfigType = ConfigTypeMap[keyof ConfigTypeMap];
