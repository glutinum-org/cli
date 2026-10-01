export interface Choice {
    None: string;
    Some: string;
}

export declare function pick(choice: Choice): void;
