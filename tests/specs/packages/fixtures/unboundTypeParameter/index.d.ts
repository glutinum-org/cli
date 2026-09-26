export interface Register {}

export type QueryKey = Register extends {
    queryKey: infer TQueryKey;
} ? TQueryKey extends ReadonlyArray<unknown> ? TQueryKey : ReadonlyArray<unknown> : ReadonlyArray<unknown>;

export type QueryFunctionContext<TQueryKey extends QueryKey = QueryKey, TPageParam = never> = [TPageParam] extends [never] ? {
    queryKey: TQueryKey;
} : {
    queryKey: TQueryKey;
    pageParam: TPageParam;
};

export type QueryFunction<T = unknown, TQueryKey extends QueryKey = QueryKey, TPageParam = never> = (context: QueryFunctionContext<TQueryKey, TPageParam>) => T;

export type QueryPersister<T = unknown, TQueryKey extends QueryKey = QueryKey, TPageParam = never> = [TPageParam] extends [never] ? (queryFn: QueryFunction<T, TQueryKey, never>) => T : (queryFn: QueryFunction<T, TQueryKey, TPageParam>) => T;

export type OmitKeyof<TObject, TKey extends keyof TObject> = Omit<TObject, TKey>;

export interface Options<TQueryFnData = unknown, TQueryKey extends QueryKey = QueryKey, TPageParam = never> {
    queryKey: TQueryKey;
    persister?: QueryPersister<TQueryFnData, NoInfer<TQueryKey>, TPageParam>;
}

export interface Client {
    setDefaults<TQueryFnData = unknown>(options: Partial<OmitKeyof<Options<TQueryFnData>, 'queryKey'>>): void;
}
