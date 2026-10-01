namespace hook {
    interface Event {
        next(): void
    }
}
namespace fs {
    type _alias = hook.Event
    interface DeleteEvent extends _alias {
        key: string
    }
}
