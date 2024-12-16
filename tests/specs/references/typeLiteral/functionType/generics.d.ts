export type Callback<Param, AtomType> = (event: {
    type: 'CREATE' | 'REMOVE';
    param: Param;
    atom: AtomType;
}) => void;
