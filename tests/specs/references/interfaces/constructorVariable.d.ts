// The instance side is the interface, the static side a variable with a construct signature,
// as the DOM declares its classes
export interface Node {
    nodeName: string;
}

export declare var Node: {
    prototype: Node;
    new(): Node;
    readonly ELEMENT_NODE: 1;
};

// A variable without a constructor is not a class
export interface Settings {
    debug: boolean;
}

export declare var Settings: {
    prototype: Settings;
};
