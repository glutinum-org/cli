// Test that when handling multiple declarations in a properties of Intersection type
// we generate obj to not crash
export class Class {
    static extend(props: any): { new(...args: any[]): any } & typeof Class;
    static include(props: any): any & typeof Class;
    static mergeOptions(props: any): any & typeof Class;

    static addInitHook(initHookFn: () => void): any & typeof Class;
    static addInitHook(methodName: string, ...args: any[]): any & typeof Class;

    static callInitHooks(): void;
}
