namespace core {
    interface Model {
        id: string
    }

    interface App {
        modelQuery(m: Model): string
        onBoot(): void
    }
}

type withoutBoot<T> = Omit<T, "onBoot">

type CoreApp = withoutBoot<core.App>
