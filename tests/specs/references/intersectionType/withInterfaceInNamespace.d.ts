interface ArtworksData {
    artworks: string[];
}

export namespace Error {
    export interface ErrorHandling {
        success: boolean;
        error?: string;
    }
}

export type ArtworksResponse = ArtworksData & Error.ErrorHandling;
