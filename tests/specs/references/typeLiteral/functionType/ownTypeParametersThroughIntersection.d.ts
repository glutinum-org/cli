export interface Base {
    kind: string;
}

export interface Req<G, S> {
    g: G;
    s: S;
}

export interface ServerOptions<Server, Logger = boolean> {
    frameworkErrors?: <G extends Base = Base>(error: string, req: Req<G, Server>, res: Req<G, Server>) => void;
}

export type HttpOptions<Server, Logger = boolean> = ServerOptions<Server, Logger> & {
    http: true;
};
