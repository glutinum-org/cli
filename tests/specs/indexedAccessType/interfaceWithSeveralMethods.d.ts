interface ConfigTypeMap {
    methodA: string | number;
    methodB: boolean;
    methodC: any;
    methodD: any;
}

export type ConfigType = ConfigTypeMap[keyof ConfigTypeMap];
