interface Todo {
    title: string;
    description: string;
}

declare function foo() : Omit<Todo, "title">;
