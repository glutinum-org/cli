interface ErrorHandling {
    success: boolean;
    error?: string;
}

interface ArtworksData {
    artworks: string[];
}

interface Pagination {
    page: number;
    pageSize: number;
}

type ArtworksResponse = ArtworksData & ErrorHandling;
type PaginationArtworksResponse = ArtworksResponse & Pagination;
