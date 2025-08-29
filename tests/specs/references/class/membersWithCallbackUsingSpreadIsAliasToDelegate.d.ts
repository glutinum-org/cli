export class Commander {
    action(fn: (...args: any[]) => Promise<void>);
}
