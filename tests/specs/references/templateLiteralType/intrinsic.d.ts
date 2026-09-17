export type Upper = Uppercase<"abc">;

export type Events = Capitalize<"click" | "key">;

// The argument is not known
export type Loose<T extends string> = Uppercase<T>;

export declare function id(value: NoInfer<string>): void;
