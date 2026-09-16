export interface DiffieHellman {
    setPublicKey(publicKey: string): void;
    setPublicKey(publicKey: string, encoding: string): void;
}

export declare class DiffieHellman {
    setPrivateKey(privateKey: string): void;
    getPrime(): string;
}

export type DiffieHellmanGroup = Omit<DiffieHellman, "setPublicKey" | "setPrivateKey">;
