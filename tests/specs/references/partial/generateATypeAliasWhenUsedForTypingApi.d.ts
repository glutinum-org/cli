interface Todo {
    title: string;
    description: string;
}

declare function UpdateTodo(todo: Partial<Todo>) : any;
