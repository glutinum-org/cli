interface ErrorHandling {
    success: boolean;
    error?: string;
}

interface ArtworksData {
    artworks: string[];
}

type ArtworksResponse = ArtworksData & ErrorHandling;
