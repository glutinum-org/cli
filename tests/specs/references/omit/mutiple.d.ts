export interface Todo {
    title: string;
    description: string;
    completed: boolean;
    createdAt: number;
}

export type TodoInfo = Omit<Todo, "completed" | "createdAt">;
