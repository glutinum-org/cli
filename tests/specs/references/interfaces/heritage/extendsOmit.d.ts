interface X {
    a: string
    b: number
}

interface Y extends Omit<X, "a"> {}
