interface Todo {
    title: string;
    description: string;
}

interface TodoExtra {
    author: string;
    date: Date
}

export type TodoPreview = Partial<Todo & TodoExtra>;
