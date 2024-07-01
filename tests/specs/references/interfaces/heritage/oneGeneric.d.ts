declare class User<Bag> {
    bag: Bag;
}

interface IUser<Bag> extends User<Bag> {}
