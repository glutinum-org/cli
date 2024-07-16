class MyType<TResult1> {
    a : TResult1
}

type T<TResult1> = TResult1 | MyType<TResult1>
