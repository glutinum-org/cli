interface CreateArtistBioBase {
    artistID: string;
    thirdParty?: boolean;
}

type CreateArtistBioRequest = CreateArtistBioBase & ({ html: string } | { markdown: string });
