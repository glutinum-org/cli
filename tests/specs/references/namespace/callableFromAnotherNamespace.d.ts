declare namespace $dbx {
    let not: dbx.not
}

namespace dbx {
    interface Expression {
        build(): string
    }

    interface not {
        (e: Expression): Expression
    }
}
