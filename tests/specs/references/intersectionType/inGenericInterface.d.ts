export type KeyFormat = "pem" | "der" | "jwk";

export interface BasePrivateKeyEncodingOptions<T extends KeyFormat> {
    format: T;
    cipher?: string;
}

export interface RSAKeyPairOptions<PubF extends KeyFormat, PrivF extends KeyFormat> {
    modulusLength: number;
    privateKeyEncoding: BasePrivateKeyEncodingOptions<PrivF> & {
        type: "pkcs1" | "pkcs8";
    };
}
