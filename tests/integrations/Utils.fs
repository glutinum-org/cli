module rec Glutinum.Vitest

open Fable.Core

type Error = System.Exception
type RegExp = System.Text.RegularExpressions.Regex


[<Erase>]
type Exports =
    [<Import("expect", "vitest")>]
    static member expect: ExpectStatic = nativeOnly

    [<Import("test", "vitest")>]
    static member test (name: string, ?fn: unit -> unit ) = nativeOnly

type [<AllowNullLiteral>] ExpectStatic =
    // inherit Chai.ExpectStatic
    // inherit AsymmetricMatchersContaining
    [<Emit("$0($1...)")>] abstract Invoke: actual: 'T * ?message: string -> Assertion<'T>
    // abstract unreachable: ((string) option -> obj) with get, set
    // abstract soft: ('T -> (string) option -> Assertion<'T>) with get, set
    // abstract extend: (MatchersObject -> unit) with get, set
    // abstract addEqualityTesters: (Array<Tester> -> unit) with get, set
    // abstract assertions: (float -> unit) with get, set
    // abstract hasAssertions: (unit -> unit) with get, set
    // abstract anything: (unit -> obj option) with get, set
    // abstract any: (obj -> obj option) with get, set
    // abstract getState: (unit -> MatcherState) with get, set
    // abstract setState: (obj -> unit) with get, set
    // abstract not: AsymmetricMatchersContaining with get, set

type [<AllowNullLiteral>] Assertion<'T> =
    // inherit Jest.Matchers<unit, 'T>
    abstract toEqual: ('T -> unit) with get, set
    abstract toStrictEqual: ('T -> unit) with get, set
    abstract toBe: ('T -> unit) with get, set
    abstract toMatch: (U2<string, RegExp> -> unit) with get, set
    abstract toMatchObject: ('T -> unit) with get, set
    abstract toContain: ('T -> unit) with get, set
    abstract toContainEqual: ('T -> unit) with get, set
    abstract toBeTruthy: (unit -> unit) with get, set
    abstract toBeFalsy: (unit -> unit) with get, set
    abstract toBeGreaterThan: (U2<float, obj> -> unit) with get, set
    abstract toBeGreaterThanOrEqual: (U2<float, obj> -> unit) with get, set
    abstract toBeLessThan: (U2<float, obj> -> unit) with get, set
    abstract toBeLessThanOrEqual: (U2<float, obj> -> unit) with get, set
    abstract toBeNaN: (unit -> unit) with get, set
    abstract toBeUndefined: (unit -> unit) with get, set
    abstract toBeNull: (unit -> unit) with get, set
    abstract toBeDefined: (unit -> unit) with get, set
    abstract toBeInstanceOf: ('T -> unit) with get, set
    abstract toBeCalledTimes: (float -> unit) with get, set
    abstract toHaveLength: (float -> unit) with get, set
    abstract toHaveProperty: (U2<string, ResizeArray<U2<string, float>>> -> ('T) option -> unit) with get, set
    abstract toBeCloseTo: (float -> (float) option -> unit) with get, set
    abstract toHaveBeenCalledTimes: (float -> unit) with get, set
    abstract toHaveBeenCalled: (unit -> unit) with get, set
    abstract toBeCalled: (unit -> unit) with get, set
    abstract toHaveBeenCalledWith: ('T -> unit) with get, set
    abstract toBeCalledWith: ('T -> unit) with get, set
    abstract toHaveBeenNthCalledWith: (float -> 'T -> unit) with get, set
    abstract nthCalledWith: (float -> 'T -> unit) with get, set
    abstract toHaveBeenLastCalledWith: ('T -> unit) with get, set
    abstract lastCalledWith: ('T -> unit) with get, set
    // abstract toThrow: ((U4<string, Constructable, RegExp, Error>) option -> unit) with get, set
    // abstract toThrowError: ((U4<string, Constructable, RegExp, Error>) option -> unit) with get, set
    abstract toThrow: string -> unit
    abstract toThrowError: string -> unit
    abstract toThrow: RegExp -> unit
    abstract toThrowError: RegExp -> unit
    abstract toThrow: Error -> unit
    abstract toThrowError: Error -> unit
    abstract toThrow: unit -> unit
    abstract toThrowError: unit -> unit
    abstract toReturn: (unit -> unit) with get, set
    abstract toHaveReturned: (unit -> unit) with get, set
    abstract toReturnTimes: (float -> unit) with get, set
    abstract toHaveReturnedTimes: (float -> unit) with get, set
    abstract toReturnWith: ('T -> unit) with get, set
    abstract toHaveReturnedWith: ('T -> unit) with get, set
    abstract toHaveLastReturnedWith: ('T -> unit) with get, set
    abstract lastReturnedWith: ('T -> unit) with get, set
    abstract toHaveNthReturnedWith: (float -> 'T -> unit) with get, set
    abstract nthReturnedWith: (float -> 'T -> unit) with get, set
