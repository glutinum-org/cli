interface ArtworksData {
    artworks: string[];
}

namespace Error {
    export interface ErrorHandling {
        success: boolean;
        error?: string;
    }
}

type ArtworksResponse = ArtworksData & Error.ErrorHandling;
