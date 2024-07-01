declare class User<A, B> {
    a: A;
    b: B;
}

interface IUser<A, B> extends User<A, B> {}
