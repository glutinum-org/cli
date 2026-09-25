export interface Link {
    href: string;
    title?: string | null;
}

export interface Image {
    href: string;
    title: string | null;
}

export type Links = Record<string, Pick<Link | Image, "href" | "title">>;

export declare function use(value: Links): void;
