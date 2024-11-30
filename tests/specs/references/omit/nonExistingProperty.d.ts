export interface Todo {
    completed: boolean;
    createdAt: number;
}

export type TodoInfo = Omit<Todo, "completed" | "remove-me-but-i-don-t-exist">;
