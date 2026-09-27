interface X {
    a: string
    b: number
    c: boolean
}

interface Y extends Pick<X, "a" | "c"> {}
