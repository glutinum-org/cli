export class Class {
    static extend(props: any): { new (...args: any[]): any } & typeof Class;
}
