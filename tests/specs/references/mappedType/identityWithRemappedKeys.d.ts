export type Arguments<T> = T & {
    _: string[];
    [argName: string]: unknown;
};

export interface Parser<T> {
    parseSync(): { [key in keyof Arguments<T> as key | Uppercase<key & string>]: Arguments<T>[key] };
    parsePlain(): { [key in keyof Arguments<T>]: Arguments<T>[key] };
}
