class IAge<A> {
    years: A;
}

declare class User<Bag, Age> {
    bag: Bag;
    age: IAge<Age>;
}

class IUser<Bag> extends User<Bag, number> {}
