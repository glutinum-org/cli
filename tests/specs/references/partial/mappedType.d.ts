type Keys = 'Accept' | 'Location';

export type Headers = Partial<{ [Key in Keys]: string[] }>;
