declare class User<A, B> {
    a: A;
    b: B;
}

class IUser<A> extends User<A, string> {}
