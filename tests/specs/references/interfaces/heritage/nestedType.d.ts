interface IAge<A> {
    years: A;
}

declare class User<Bag, Age> {
    bag: Bag;
    age: IAge<Age>;
}

interface IUser<Bag> extends User<Bag, number> {}
