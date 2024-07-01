declare class User<A, B> {
    a: A;
    b: B;
}

interface IUser<A> extends User<A, string> {}
