export interface MyObject {
    instance: () => this;

    instance1: (a : Boolean) => this;

    instance2: (a : Boolean, b : number) => this;
}
