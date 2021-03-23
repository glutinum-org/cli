module rec Node.Assert

open System
open Fable.Core
open Fable.Core.JS

type Error = System.Exception
type Function = System.Action
type RegExp = System.Text.RegularExpressions.Regex

type [<AllowNullLiteral>] IExports =
    abstract AssertionError: AssertionErrorStatic
    abstract fail: ?message: Error -> unit
    abstract fail: ?message: string -> unit
    [<Obsolete("since v10.0.0 - use fail([message]) or other assert functions instead.")>]
    abstract fail: actual: obj * expected: obj option * ?message: U2<string, Error> * ?operator: string * ?stackStartFn: Function -> unit
    abstract ok: value: obj * ?message: U2<string, Error> -> unit
    [<Obsolete("since v9.9.0 - use strictEqual() instead.")>]
    abstract equal: actual: obj * expected: obj option * ?message: U2<string, Error> -> unit
    [<Obsolete("since v9.9.0 - use notStrictEqual() instead.")>]
    abstract notEqual: actual: obj * expected: obj option * ?message: U2<string, Error> -> unit
    [<Obsolete("since v9.9.0 - use deepStrictEqual() instead.")>]
    abstract deepEqual: actual: obj * expected: obj option * ?message: U2<string, Error> -> unit
    [<Obsolete("since v9.9.0 - use notDeepStrictEqual() instead.")>]
    abstract notDeepEqual: actual: obj * expected: obj option * ?message: U2<string, Error> -> unit
    abstract strictEqual: actual: obj * expected: 'T * ?message: U2<string, Error> -> unit
    abstract notStrictEqual: actual: obj * expected: 'T * ?message: U2<string, Error> -> unit
    abstract deepStrictEqual: actual: obj * expected: 'T * ?message: U2<string, Error> -> unit
    abstract notDeepStrictEqual: actual: obj * expected: obj option * ?message: U2<string, Error> -> unit
    abstract throws: block: (unit -> 'T) * ?message: string -> unit
    abstract throws: block: (unit -> obj) * ?message: Error -> unit
    abstract throws: block: (unit -> obj) -> unit
    abstract throws: block: (unit -> obj option) * error: AssertPredicate * ?message: U2<string, Error> -> unit
    abstract doesNotThrow: block: (unit -> obj option) * ?message: U2<string, Error> -> unit
    abstract doesNotThrow: block: (unit -> obj option) * error: U2<RegExp, Function> * ?message: U2<string, Error> -> unit
    abstract ifError: value: obj option -> bool
    abstract rejects: block: U2<(unit -> Promise<obj option>), Promise<obj option>> * ?message: U2<string, Error> -> Promise<unit>
    abstract rejects: block: U2<(unit -> Promise<obj option>), Promise<obj option>> * error: AssertPredicate * ?message: U2<string, Error> -> Promise<unit>
    abstract doesNotReject: block: U2<(unit -> Promise<obj option>), Promise<obj option>> * ?message: U2<string, Error> -> Promise<unit>
    abstract doesNotReject: block: U2<(unit -> Promise<obj option>), Promise<obj option>> * error: U2<RegExp, Function> * ?message: U2<string, Error> -> Promise<unit>
    abstract ``match``: value: string * regExp: RegExp * ?message: U2<string, Error> -> unit
    abstract doesNotMatch: value: string * regExp: RegExp * ?message: U2<string, Error> -> unit
    abstract strict: obj

type [<AllowNullLiteral;AbstractClass>] AssertionError =
    inherit Error
    abstract name: string with get, set
    abstract message: string with get, set
    abstract actual: obj with get, set
    abstract expected: obj option with get, set
    abstract operator: string with get, set
    abstract generatedMessage: bool with get, set
    abstract code: string with get, set

type [<AllowNullLiteral>] AssertionErrorStatic =
    [<EmitConstructor>] abstract Create: ?options: AssertionErrorStaticOptions -> AssertionError

type [<AllowNullLiteral>] AssertionErrorStaticOptions =
    abstract message: string option with get, set
    abstract actual: obj with get, set
    abstract expected: obj option with get, set
    abstract operator: string option with get, set
    abstract stackStartFn: Function option with get, set

type AssertPredicate =
    U5<RegExp, obj, (obj option -> bool), obj, Error>
