declare function widget(): { id: string };

declare namespace widget {
    const version: string;
}

export = widget;
