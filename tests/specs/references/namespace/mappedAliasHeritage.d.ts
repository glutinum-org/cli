type excludeHooks<Type> = {
    [Property in keyof Type as Exclude<Property, `on${string}`>]: Type[Property]
};

namespace core {
    interface App {
        pb(): pb.PocketBase
        onBoot(): void
    }
}

type CoreApp = excludeHooks<core.App>

namespace pb {
    interface PocketBase extends CoreApp {
        start(): void
    }
}

interface PocketBase extends excludeHooks<pb.PocketBase> {}
