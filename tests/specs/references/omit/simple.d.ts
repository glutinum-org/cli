export interface Todo {
    title: string;
    description: string;
    completed: boolean;
    createdAt: number;
}

export type TodoPreview = Omit<Todo, "description">;
