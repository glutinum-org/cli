import { ResolveBody, Schema } from "./resolve";

export interface Request<S extends Schema> {
    body: ResolveBody<S>;
}
