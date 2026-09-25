export interface MyListenerOptions extends AddEventListenerOptions {
    label: string;
}

export interface MyTarget extends EventTarget {
    id: string;
}
