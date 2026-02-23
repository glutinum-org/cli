interface Thenable<R> {}

declare function funcA<R>(task: (progress: any, data: any) => Thenable<R>): Thenable<R>;

declare function funcB<R>(task: (progress: any, data: any) => [Thenable<R>, Thenable<R>]): Thenable<R>;
