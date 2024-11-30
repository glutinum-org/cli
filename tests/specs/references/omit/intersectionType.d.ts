interface Todo {
    title: string;
    description: string;
}

interface TodoExtra {
    completed: boolean;
    createdAt: number;
}

type TodoPreview = Omit<Todo & TodoExtra, "description">;
