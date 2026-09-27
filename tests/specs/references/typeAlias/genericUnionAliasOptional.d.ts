export interface UpdateOutputFileStampsProject {
    kind: 0;
}

export interface BuildInvalidedProject<T> {
    kind: 1;
    program: T;
}

export type InvalidatedProject<T> = UpdateOutputFileStampsProject | BuildInvalidedProject<T>;

export interface SolutionBuilder<T> {
    getNextInvalidatedProject(): InvalidatedProject<T> | undefined;
}
