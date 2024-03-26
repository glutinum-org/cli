// Test that we don't end up in an infinite loop when handling `typeof Class`
export class Class {
    static extend(props: any): { new (...args: any[]): any } & typeof Class;
}
