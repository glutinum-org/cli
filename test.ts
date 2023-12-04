type OptionsFlags<Type> = {
    [Property in keyof Type]: boolean;
};

type Features = {
    darkMode: () => void;
    newUserProfile: () => void;
};

type FeatureOptions = OptionsFlags<Features>;

interface ConfigTypeMap {
    default: string | number | Date;
}

export type ConfigType = ConfigTypeMap[keyof ConfigTypeMap];

interface User {
    name: string;
}

interface StringArray {
    [index: number]: User;
}

let myArray: StringArray = {
    0: {
        name: "John",
    },
    1: {
        name: "Bob",
    },
};

console.log(myArray["0"].name);
