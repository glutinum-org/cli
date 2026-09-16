declare namespace compiler {
    enum Kind {
        A = 1,
        B = 2
    }

    interface Node {
        kind: Kind;
    }
}

export = compiler;
