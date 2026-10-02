export interface MyListenerOptions extends AddEventListenerOptions {
    label: string;
}

export interface MyTarget extends EventTarget {
    id: string;
}

export declare function listen(target: MyTarget, options: MyListenerOptions): void;
