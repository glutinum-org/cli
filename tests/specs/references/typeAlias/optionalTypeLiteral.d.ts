export type Documentation = { readonly kind: string } | undefined;

export type Docs = readonly { readonly kind: string }[] | undefined;

export interface Named {
    name: string;
}

export type MaybeNamed = Named | undefined;
