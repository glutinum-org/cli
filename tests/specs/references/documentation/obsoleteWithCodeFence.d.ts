export interface App {
    /**
     * @deprecated Use `fire` instead.
     * ```ts
     * import { fire } from 'hono/service-worker'
     * fire(app)
     * ```
     */
    fire(): void;
}
