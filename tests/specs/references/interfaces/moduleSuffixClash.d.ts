export interface Module {
    type: string;
}

export interface Formatter {
    add(name: string, fc: (value: any, lng: string | undefined, options: any) => string): void;
}

export interface FormatterModule extends Module, Formatter {
    type: "formatter";
}
