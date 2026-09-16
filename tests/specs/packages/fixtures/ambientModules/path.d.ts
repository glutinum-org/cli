declare module "path" {
    namespace path {
        interface PlatformPath {
            join(...paths: string[]): string;
            sep: string;
        }
    }
    const path: path.PlatformPath;
    export = path;
}
declare module "node:path" {
    import path = require("path");
    export = path;
}
