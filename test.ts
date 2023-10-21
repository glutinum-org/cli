type OptionsFlags<Type> = {
    [Property in keyof Type]: boolean;
};

type Features = {
    darkMode: () => void;
    newUserProfile: () => void;
};

type FeatureOptions = OptionsFlags<Features>;


interface ConfigTypeMap {
    default: string | number | Date
}

export type ConfigType = ConfigTypeMap[keyof ConfigTypeMap]
