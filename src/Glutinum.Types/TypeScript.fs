module Glutinum.Types.TypeScript

open System
open Fable.Core

[<AllowNullLiteral>]
[<Interface>]
type ConcatArray<'T> =
    abstract member length: int with get

    [<EmitIndexer>]
    abstract member Item: n: int -> 'T with get

    abstract member join: ?separator: string -> string
    abstract member slice: ?start: int * ?``end``: int -> ResizeArray<'T>

[<AllowNullLiteral>]
[<Interface>]
type ReadonlyArray<'T> =
    /// <summary>
    /// Gets the length of the array. This is a number one higher than the highest element defined in an array.
    /// </summary>
    abstract member length: int with get
    /// <summary>
    /// Returns a string representation of an array.
    /// </summary>
    abstract member toString: unit -> string
    /// <summary>
    /// Returns a string representation of an array. The elements are converted to string using their toLocaleString methods.
    /// </summary>
    abstract member toLocaleString: unit -> string

    /// <summary>
    /// Combines two or more arrays.
    /// </summary>
    /// <param name="items">
    /// Additional items to add to the end of array1.
    /// </param>
    abstract member concat:
        [<ParamArray>] items: ConcatArray<'T>[] -> ResizeArray<'T>

    /// <summary>
    /// Combines two or more arrays.
    /// </summary>
    /// <param name="items">
    /// Additional items to add to the end of array1.
    /// </param>
    abstract member concat:
        [<ParamArray>] items: U2<'T, ConcatArray<'T>>[] -> ResizeArray<'T>

    /// <summary>
    /// Adds all the elements of an array separated by the specified separator string.
    /// </summary>
    /// <param name="separator">
    /// A string used to separate one element of an array from the next in the resulting String. If omitted, the array elements are separated with a comma.
    /// </param>
    abstract member join: ?separator: string -> string
    /// <summary>
    /// Returns a section of an array.
    /// </summary>
    /// <param name="start">
    /// The beginning of the specified portion of the array.
    /// </param>
    /// <param name="end">
    /// The end of the specified portion of the array. This is exclusive of the element at the index 'end'.
    /// </param>
    abstract member slice: ?start: int * ?``end``: int -> ResizeArray<'T>
    /// <summary>
    /// Returns the index of the first occurrence of a value in an array.
    /// </summary>
    /// <param name="searchElement">
    /// The value to locate in the array.
    /// </param>
    /// <param name="fromIndex">
    /// The array index at which to begin the search. If fromIndex is omitted, the search starts at index 0.
    /// </param>
    abstract member indexOf: searchElement: 'T * ?fromIndex: int -> int
    /// <summary>
    /// Returns the index of the last occurrence of a specified value in an array.
    /// </summary>
    /// <param name="searchElement">
    /// The value to locate in the array.
    /// </param>
    /// <param name="fromIndex">
    /// The array index at which to begin the search. If fromIndex is omitted, the search starts at the last index in the array.
    /// </param>
    abstract member lastIndexOf: searchElement: 'T * ?fromIndex: int -> int

    /// <summary>
    /// Determines whether all the members of an array satisfy the specified test.
    /// </summary>
    /// <param name="predicate">
    /// A function that accepts up to three arguments. The every method calls
    /// the predicate function for each element in the array until the predicate returns a value
    /// which is coercible to the Boolean value false, or until the end of the array.
    /// </param>
    /// <param name="thisArg">
    /// An object to which the this keyword can refer in the predicate function.
    /// If thisArg is omitted, undefined is used as the this value.
    /// </param>
    abstract member every:
        predicate: ('T -> int -> ReadonlyArray<'T> -> bool) * ?thisArg: obj ->
            bool

    /// <summary>
    /// Determines whether all the members of an array satisfy the specified test.
    /// </summary>
    /// <param name="predicate">
    /// A function that accepts up to three arguments. The every method calls
    /// the predicate function for each element in the array until the predicate returns a value
    /// which is coercible to the Boolean value false, or until the end of the array.
    /// </param>
    /// <param name="thisArg">
    /// An object to which the this keyword can refer in the predicate function.
    /// If thisArg is omitted, undefined is used as the this value.
    /// </param>
    abstract member every:
        predicate: ('T -> int -> ReadonlyArray<'T> -> obj) * ?thisArg: obj ->
            bool

    /// <summary>
    /// Determines whether the specified callback function returns true for any element of an array.
    /// </summary>
    /// <param name="predicate">
    /// A function that accepts up to three arguments. The some method calls
    /// the predicate function for each element in the array until the predicate returns a value
    /// which is coercible to the Boolean value true, or until the end of the array.
    /// </param>
    /// <param name="thisArg">
    /// An object to which the this keyword can refer in the predicate function.
    /// If thisArg is omitted, undefined is used as the this value.
    /// </param>
    abstract member some:
        predicate: ('T -> int -> ReadonlyArray<'T> -> obj) * ?thisArg: obj ->
            bool

    /// <summary>
    /// Performs the specified action for each element in an array.
    /// </summary>
    /// <param name="callbackfn">
    /// A function that accepts up to three arguments. forEach calls the callbackfn function one time for each element in the array.
    /// </param>
    /// <param name="thisArg">
    /// An object to which the this keyword can refer in the callbackfn function. If thisArg is omitted, undefined is used as the this value.
    /// </param>
    abstract member forEach:
        callbackfn: ('T -> int -> ReadonlyArray<'T> -> unit) * ?thisArg: obj ->
            unit

    /// <summary>
    /// Calls a defined callback function on each element of an array, and returns an array that contains the results.
    /// </summary>
    /// <param name="callbackfn">
    /// A function that accepts up to three arguments. The map method calls the callbackfn function one time for each element in the array.
    /// </param>
    /// <param name="thisArg">
    /// An object to which the this keyword can refer in the callbackfn function. If thisArg is omitted, undefined is used as the this value.
    /// </param>
    abstract member map:
        callbackfn: ('T -> int -> ReadonlyArray<'T> -> 'U) * ?thisArg: obj ->
            ResizeArray<'U>

    /// <summary>
    /// Returns the elements of an array that meet the condition specified in a callback function.
    /// </summary>
    /// <param name="predicate">
    /// A function that accepts up to three arguments. The filter method calls the predicate function one time for each element in the array.
    /// </param>
    /// <param name="thisArg">
    /// An object to which the this keyword can refer in the predicate function. If thisArg is omitted, undefined is used as the this value.
    /// </param>
    abstract member filter:
        predicate: ('T -> int -> ReadonlyArray<'T> -> bool) * ?thisArg: obj ->
            ResizeArray<'S>

    /// <summary>
    /// Returns the elements of an array that meet the condition specified in a callback function.
    /// </summary>
    /// <param name="predicate">
    /// A function that accepts up to three arguments. The filter method calls the predicate function one time for each element in the array.
    /// </param>
    /// <param name="thisArg">
    /// An object to which the this keyword can refer in the predicate function. If thisArg is omitted, undefined is used as the this value.
    /// </param>
    abstract member filter:
        predicate: ('T -> int -> ReadonlyArray<'T> -> obj) * ?thisArg: obj ->
            ResizeArray<'T>

    /// <summary>
    /// Calls the specified callback function for all the elements in an array. The return value of the callback function is the accumulated result, and is provided as an argument in the next call to the callback function.
    /// </summary>
    /// <param name="callbackfn">
    /// A function that accepts up to four arguments. The reduce method calls the callbackfn function one time for each element in the array.
    /// </param>
    /// <param name="initialValue">
    /// If initialValue is specified, it is used as the initial value to start the accumulation. The first call to the callbackfn function provides this value as an argument instead of an array value.
    /// </param>
    abstract member reduce:
        callbackfn: ('T -> 'T -> int -> ReadonlyArray<'T> -> 'T) -> 'T

    /// <summary>
    /// Calls the specified callback function for all the elements in an array. The return value of the callback function is the accumulated result, and is provided as an argument in the next call to the callback function.
    /// </summary>
    abstract member reduce:
        callbackfn: ('T -> 'T -> int -> ReadonlyArray<'T> -> 'T) *
        initialValue: 'T ->
            'T

    /// <summary>
    /// Calls the specified callback function for all the elements in an array. The return value of the callback function is the accumulated result, and is provided as an argument in the next call to the callback function.
    /// </summary>
    /// <param name="callbackfn">
    /// A function that accepts up to four arguments. The reduce method calls the callbackfn function one time for each element in the array.
    /// </param>
    /// <param name="initialValue">
    /// If initialValue is specified, it is used as the initial value to start the accumulation. The first call to the callbackfn function provides this value as an argument instead of an array value.
    /// </param>
    abstract member reduce:
        callbackfn: ('U -> 'T -> int -> ReadonlyArray<'T> -> 'U) *
        initialValue: 'U ->
            'U

    /// <summary>
    /// Calls the specified callback function for all the elements in an array, in descending order. The return value of the callback function is the accumulated result, and is provided as an argument in the next call to the callback function.
    /// </summary>
    /// <param name="callbackfn">
    /// A function that accepts up to four arguments. The reduceRight method calls the callbackfn function one time for each element in the array.
    /// </param>
    /// <param name="initialValue">
    /// If initialValue is specified, it is used as the initial value to start the accumulation. The first call to the callbackfn function provides this value as an argument instead of an array value.
    /// </param>
    abstract member reduceRight:
        callbackfn: ('T -> 'T -> int -> ReadonlyArray<'T> -> 'T) -> 'T

    /// <summary>
    /// Calls the specified callback function for all the elements in an array, in descending order. The return value of the callback function is the accumulated result, and is provided as an argument in the next call to the callback function.
    /// </summary>
    abstract member reduceRight:
        callbackfn: ('T -> 'T -> int -> ReadonlyArray<'T> -> 'T) *
        initialValue: 'T ->
            'T

    /// <summary>
    /// Calls the specified callback function for all the elements in an array, in descending order. The return value of the callback function is the accumulated result, and is provided as an argument in the next call to the callback function.
    /// </summary>
    /// <param name="callbackfn">
    /// A function that accepts up to four arguments. The reduceRight method calls the callbackfn function one time for each element in the array.
    /// </param>
    /// <param name="initialValue">
    /// If initialValue is specified, it is used as the initial value to start the accumulation. The first call to the callbackfn function provides this value as an argument instead of an array value.
    /// </param>
    abstract member reduceRight:
        callbackfn: ('U -> 'T -> int -> ReadonlyArray<'T> -> 'U) *
        initialValue: 'U ->
            'U

    [<EmitIndexer>]
    abstract member Item: n: int -> 'T with get
