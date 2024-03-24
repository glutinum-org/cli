interface CreateArtistBioBase {
    artistID: string;
    thirdParty?: boolean;
}

interface Pagination {
    page: number;
    pageSize: number;
}

type CreateArtistBioRequest =
    CreateArtistBioBase
    & { html: string }
    & { markdown: string }
    & Pagination;
