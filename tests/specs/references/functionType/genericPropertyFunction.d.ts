export interface Story {
    id: string;
}

export interface Args {
    // The function declares its own type parameters, a property can't be generic
    mount: <Component = never, Id extends string = string>(id: Id, props?: Component, options?: Story) => Promise<Story>;
}
