namespace rec Glutinum.Types

open Fable.Core
open Fable.Core.JsInterop
open System

type Iterable<'T> = Collections.Generic.IEnumerable<'T>

module TypeScript =

    [<AbstractClass>]
    [<Erase>]
    type Exports =
        [<Global("Symbol")>]
        static member inline Symbol: TypeScript.SymbolConstructor = nativeOnly

        [<Global("SharedArrayBuffer")>]
        static member inline SharedArrayBuffer: TypeScript.SharedArrayBufferConstructor = nativeOnly

        /// <summary>
        /// Provides functionality common to all JavaScript objects.
        /// </summary>
        [<Global("Object")>]
        static member inline Object: TypeScript.ObjectConstructor = nativeOnly

        /// <summary>
        /// Allows manipulation and formatting of text strings and determination and location of substrings within strings.
        /// </summary>
        [<Global("String")>]
        static member inline String: TypeScript.StringConstructor = nativeOnly

        /// <summary>
        /// An object that represents a number of any kind. All JavaScript numbers are 64-bit floating-point numbers.
        /// </summary>
        [<Global("Number")>]
        static member inline Number: TypeScript.NumberConstructor = nativeOnly

        /// <summary>
        /// Enables basic storage and retrieval of dates and times.
        /// </summary>
        [<Global("Date")>]
        static member inline Date: TypeScript.DateConstructor = nativeOnly

        [<Global("Reflect")>]
        static member Reflect: Reflect.Exports = nativeOnly

    [<AllowNullLiteral>]
    [<Interface>]
    type ReadonlyMap<'K, 'V> =
        inherit Iterable<'K * 'V>

        abstract member forEach:
            callbackfn: ReadonlyMap.forEach.callbackfn<'K, 'V> * ?thisArg: obj -> unit

        abstract member get: key: 'K -> 'V option
        abstract member has: key: 'K -> bool
        abstract member size: float with get
        /// <summary>
        /// Returns an iterable of key, value pairs for every entry in the map.
        /// </summary>
        abstract member entries: unit -> TypeScript.IterableIterator<'K * 'V>
        /// <summary>
        /// Returns an iterable of keys in the map
        /// </summary>
        abstract member keys: unit -> TypeScript.IterableIterator<'K>
        /// <summary>
        /// Returns an iterable of values in the map
        /// </summary>
        abstract member values: unit -> TypeScript.IterableIterator<'V>

    [<AllowNullLiteral>]
    [<Interface>]
    type ReadonlySet<'T> =
        inherit Iterable<'T>

        abstract member forEach:
            callbackfn: ReadonlySet.forEach.callbackfn<'T> * ?thisArg: obj -> unit

        abstract member has: value: 'T -> bool
        abstract member size: float with get
        /// <summary>
        /// Returns an iterable of [v,v] pairs for every value <c>v</c> in the set.
        /// </summary>
        abstract member entries: unit -> TypeScript.IterableIterator<'T * 'T>
        /// <summary>
        /// Despite its name, returns an iterable of the values in the set.
        /// </summary>
        abstract member keys: unit -> TypeScript.IterableIterator<'T>
        /// <summary>
        /// Returns an iterable of values in the set.
        /// </summary>
        abstract member values: unit -> TypeScript.IterableIterator<'T>

    [<AllowNullLiteral>]
    [<Interface>]
    type DateConstructor =
        [<EmitConstructor>]
        abstract member Create: value: float -> TypeScript.Date

        [<EmitConstructor>]
        abstract member Create: value: string -> TypeScript.Date

        [<EmitConstructor>]
        abstract member Create: value: TypeScript.Date -> TypeScript.Date

        /// <summary>
        /// Returns the number of milliseconds between midnight, January 1, 1970 Universal Coordinated Time (UTC) (or GMT) and the specified date.
        /// </summary>
        /// <param name="year">
        /// The full year designation is required for cross-century date accuracy. If year is between 0 and 99 is used, then year is assumed to be 1900 + year.
        /// </param>
        /// <param name="monthIndex">
        /// The month as a number between 0 and 11 (January to December).
        /// </param>
        /// <param name="date">
        /// The date as a number between 1 and 31.
        /// </param>
        /// <param name="hours">
        /// Must be supplied if minutes is supplied. A number from 0 to 23 (midnight to 11pm) that specifies the hour.
        /// </param>
        /// <param name="minutes">
        /// Must be supplied if seconds is supplied. A number from 0 to 59 that specifies the minutes.
        /// </param>
        /// <param name="seconds">
        /// Must be supplied if milliseconds is supplied. A number from 0 to 59 that specifies the seconds.
        /// </param>
        /// <param name="ms">
        /// A number from 0 to 999 that specifies the milliseconds.
        /// </param>
        abstract member UTC:
            year: float *
            ?monthIndex: float *
            ?date: float *
            ?hours: float *
            ?minutes: float *
            ?seconds: float *
            ?ms: float ->
                float

        [<EmitConstructor>]
        abstract member Create: unit -> TypeScript.Date

        [<EmitConstructor>]
        abstract member Create:
            year: float *
            monthIndex: float *
            ?date: float *
            ?hours: float *
            ?minutes: float *
            ?seconds: float *
            ?ms: float ->
                TypeScript.Date

        [<Emit("$0($1...)")>]
        abstract member Invoke: unit -> string

        abstract member prototype: TypeScript.Date with get
        /// <summary>
        /// Parses a string containing a date, and returns the number of milliseconds between that date and midnight, January 1, 1970.
        /// </summary>
        /// <param name="s">
        /// A date string
        /// </param>
        abstract member parse: s: string -> float
        /// <summary>
        /// Returns the number of milliseconds elapsed since midnight, January 1, 1970 Universal Coordinated Time (UTC).
        /// </summary>
        abstract member now: unit -> float

    [<AllowNullLiteral>]
    [<Interface>]
    type NumberConstructor =
        /// <summary>
        /// The value of Number.EPSILON is the difference between 1 and the smallest value greater than 1
        /// that is representable as a Number value, which is approximately:
        /// 2.2204460492503130808472633361816 x 10‍−‍16.
        /// </summary>
        abstract member EPSILON: float with get
        /// <summary>
        /// Returns true if passed value is finite.
        /// Unlike the global isFinite, Number.isFinite doesn't forcibly convert the parameter to a
        /// number. Only finite values of the type number, result in true.
        /// </summary>
        /// <param name="number">
        /// A numeric value.
        /// </param>
        abstract member isFinite: number: obj -> bool
        /// <summary>
        /// Returns true if the value passed is an integer, false otherwise.
        /// </summary>
        /// <param name="number">
        /// A numeric value.
        /// </param>
        abstract member isInteger: number: obj -> bool
        /// <summary>
        /// Returns a Boolean value that indicates whether a value is the reserved value NaN (not a
        /// number). Unlike the global isNaN(), Number.isNaN() doesn't forcefully convert the parameter
        /// to a number. Only values of the type number, that are also NaN, result in true.
        /// </summary>
        /// <param name="number">
        /// A numeric value.
        /// </param>
        abstract member isNaN: number: obj -> bool
        /// <summary>
        /// Returns true if the value passed is a safe integer.
        /// </summary>
        /// <param name="number">
        /// A numeric value.
        /// </param>
        abstract member isSafeInteger: number: obj -> bool
        /// <summary>
        /// The value of the largest integer n such that n and n + 1 are both exactly representable as
        /// a Number value.
        /// The value of Number.MAX_SAFE_INTEGER is 9007199254740991 2^53 − 1.
        /// </summary>
        abstract member MAX_SAFE_INTEGER: float with get
        /// <summary>
        /// The value of the smallest integer n such that n and n − 1 are both exactly representable as
        /// a Number value.
        /// The value of Number.MIN_SAFE_INTEGER is −9007199254740991 (−(2^53 − 1)).
        /// </summary>
        abstract member MIN_SAFE_INTEGER: float with get
        /// <summary>
        /// Converts a string to a floating-point number.
        /// </summary>
        /// <param name="string">
        /// A string that contains a floating-point number.
        /// </param>
        abstract member parseFloat: string: string -> float
        /// <summary>
        /// Converts A string to an integer.
        /// </summary>
        /// <param name="string">
        /// A string to convert into a number.
        /// </param>
        /// <param name="radix">
        /// A value between 2 and 36 that specifies the base of the number in <c>string</c>.
        /// If this argument is not supplied, strings with a prefix of '0x' are considered hexadecimal.
        /// All other strings are considered decimal.
        /// </param>
        abstract member parseInt: string: string * ?radix: float -> float

        [<EmitConstructor>]
        abstract member Create: ?value: obj -> float

        [<Emit("$0($1...)")>]
        abstract member Invoke: ?value: obj -> float

        abstract member prototype: float with get
        /// <summary>
        /// The largest number that can be represented in JavaScript. Equal to approximately 1.79E+308.
        /// </summary>
        abstract member MAX_VALUE: float with get
        /// <summary>
        /// The closest number to zero that can be represented in JavaScript. Equal to approximately 5.00E-324.
        /// </summary>
        abstract member MIN_VALUE: float with get
        /// <summary>
        /// A value that is not a number.
        /// In equality comparisons, NaN does not equal any value, including itself. To test whether a value is equivalent to NaN, use the isNaN function.
        /// </summary>
        abstract member NaN: float with get
        /// <summary>
        /// A value that is less than the largest negative number that can be represented in JavaScript.
        /// JavaScript displays NEGATIVE_INFINITY values as -infinity.
        /// </summary>
        abstract member NEGATIVE_INFINITY: float with get
        /// <summary>
        /// A value greater than the largest number that can be represented in JavaScript.
        /// JavaScript displays POSITIVE_INFINITY values as infinity.
        /// </summary>
        abstract member POSITIVE_INFINITY: float with get

    [<AllowNullLiteral>]
    [<Interface>]
    type ObjectConstructor =
        /// <summary>
        /// Copy the values of all of the enumerable own properties from one or more source objects to a
        /// target object. Returns the target object.
        /// </summary>
        /// <param name="target">
        /// The target object to copy to.
        /// </param>
        /// <param name="source">
        /// The source object from which to copy properties.
        /// </param>
        abstract member assign<'T, 'U> : target: 'T * source: 'U -> obj
        /// <summary>
        /// Copy the values of all of the enumerable own properties from one or more source objects to a
        /// target object. Returns the target object.
        /// </summary>
        /// <param name="target">
        /// The target object to copy to.
        /// </param>
        /// <param name="source1">
        /// The first source object from which to copy properties.
        /// </param>
        /// <param name="source2">
        /// The second source object from which to copy properties.
        /// </param>
        abstract member assign<'T, 'U, 'V> : target: 'T * source1: 'U * source2: 'V -> obj

        /// <summary>
        /// Copy the values of all of the enumerable own properties from one or more source objects to a
        /// target object. Returns the target object.
        /// </summary>
        /// <param name="target">
        /// The target object to copy to.
        /// </param>
        /// <param name="source1">
        /// The first source object from which to copy properties.
        /// </param>
        /// <param name="source2">
        /// The second source object from which to copy properties.
        /// </param>
        /// <param name="source3">
        /// The third source object from which to copy properties.
        /// </param>
        abstract member assign<'T, 'U, 'V, 'W> :
            target: 'T * source1: 'U * source2: 'V * source3: 'W -> obj

        /// <summary>
        /// Copy the values of all of the enumerable own properties from one or more source objects to a
        /// target object. Returns the target object.
        /// </summary>
        /// <param name="target">
        /// The target object to copy to.
        /// </param>
        /// <param name="sources">
        /// One or more source objects from which to copy properties
        /// </param>
        abstract member assign: target: obj * [<ParamArray>] sources: obj[] -> obj
        /// <summary>
        /// Returns an array of all symbol properties found directly on object o.
        /// </summary>
        /// <param name="o">
        /// Object to retrieve the symbols from.
        /// </param>
        abstract member getOwnPropertySymbols: o: obj -> ResizeArray<obj>
        /// <summary>
        /// Returns the names of the enumerable string properties and methods of an object.
        /// </summary>
        /// <param name="o">
        /// Object that contains the properties and methods. This can be an object that you created or an existing Document Object Model (DOM) object.
        /// </param>
        abstract member keys: o: obj -> ResizeArray<string>
        /// <summary>
        /// Returns true if the values are the same value, false otherwise.
        /// </summary>
        /// <param name="value1">
        /// The first value.
        /// </param>
        /// <param name="value2">
        /// The second value.
        /// </param>
        abstract member is: value1: obj * value2: obj -> bool
        /// <summary>
        /// Sets the prototype of a specified object o to object proto or null. Returns the object o.
        /// </summary>
        /// <param name="o">
        /// The object to change its prototype.
        /// </param>
        /// <param name="proto">
        /// The value of the new prototype or null.
        /// </param>
        abstract member setPrototypeOf: o: obj * proto: obj option -> obj
        /// <summary>
        /// Returns an array of values of the enumerable properties of an object
        /// </summary>
        /// <param name="o">
        /// Object that contains the properties and methods. This can be an object that you created or an existing Document Object Model (DOM) object.
        /// </param>
        abstract member values<'T> : o: ObjectConstructor.values.o<'T> -> ResizeArray<'T>
        /// <summary>
        /// Returns an array of values of the enumerable properties of an object
        /// </summary>
        /// <param name="o">
        /// Object that contains the properties and methods. This can be an object that you created or an existing Document Object Model (DOM) object.
        /// </param>
        abstract member values<'T> : o: TypeScript.ArrayLike<'T> -> ResizeArray<'T>
        /// <summary>
        /// Returns an array of values of the enumerable properties of an object
        /// </summary>
        /// <param name="o">
        /// Object that contains the properties and methods. This can be an object that you created or an existing Document Object Model (DOM) object.
        /// </param>
        abstract member values: o: obj -> ResizeArray<obj>
        /// <summary>
        /// Returns an array of key/values of the enumerable properties of an object
        /// </summary>
        /// <param name="o">
        /// Object that contains the properties and methods. This can be an object that you created or an existing Document Object Model (DOM) object.
        /// </param>
        abstract member entries<'T> : o: ObjectConstructor.entries.o<'T> -> ResizeArray<string * 'T>
        /// <summary>
        /// Returns an array of key/values of the enumerable properties of an object
        /// </summary>
        /// <param name="o">
        /// Object that contains the properties and methods. This can be an object that you created or an existing Document Object Model (DOM) object.
        /// </param>
        abstract member entries<'T> : o: TypeScript.ArrayLike<'T> -> ResizeArray<string * 'T>
        /// <summary>
        /// Returns an array of key/values of the enumerable properties of an object
        /// </summary>
        /// <param name="o">
        /// Object that contains the properties and methods. This can be an object that you created or an existing Document Object Model (DOM) object.
        /// </param>
        abstract member entries: o: obj -> ResizeArray<string * obj>
        /// <summary>
        /// Returns an object containing all own property descriptors of an object
        /// </summary>
        /// <param name="o">
        /// Object that contains the properties and methods. This can be an object that you created or an existing Document Object Model (DOM) object.
        /// </param>
        abstract member getOwnPropertyDescriptors<'T> : o: 'T -> obj

        /// <summary>
        /// Returns an object created by key-value entries for properties and methods
        /// </summary>
        /// <param name="entries">
        /// An iterable object that contains key-value entries for properties and methods.
        /// </param>
        abstract member fromEntries<'T> :
            entries: Iterable<TypeScript.PropertyKey * 'T> -> ObjectConstructor.fromEntries<'T>

        /// <summary>
        /// Returns an object created by key-value entries for properties and methods
        /// </summary>
        /// <param name="entries">
        /// An iterable object that contains key-value entries for properties and methods.
        /// </param>
        abstract member fromEntries:
            entries: Iterable<TypeScript.PropertyKey * obj> -> ObjectConstructor.fromEntries_1

        /// <summary>
        /// Returns an object created by key-value entries for properties and methods
        /// </summary>
        /// <param name="entries">
        /// An iterable object that contains key-value entries for properties and methods.
        /// </param>
        abstract member fromEntries: entries: Iterable<ReadonlyArray<obj>> -> obj
        /// <summary>
        /// Determines whether an object has a property with the specified name.
        /// </summary>
        /// <param name="o">
        /// An object.
        /// </param>
        /// <param name="v">
        /// A property name.
        /// </param>
        abstract member hasOwn: o: obj * v: string -> bool
        /// <summary>
        /// Determines whether an object has a property with the specified name.
        /// </summary>
        /// <param name="o">
        /// An object.
        /// </param>
        /// <param name="v">
        /// A property name.
        /// </param>
        abstract member hasOwn: o: obj * v: float -> bool
        /// <summary>
        /// Determines whether an object has a property with the specified name.
        /// </summary>
        /// <param name="o">
        /// An object.
        /// </param>
        /// <param name="v">
        /// A property name.
        /// </param>
        abstract member hasOwn: o: obj * v: obj -> bool

        [<EmitConstructor>]
        abstract member Create: ?value: obj -> obj

        [<Emit("$0($1...)")>]
        abstract member Invoke: unit -> obj

        [<Emit("$0($1...)")>]
        abstract member Invoke: value: obj -> obj

        /// <summary>
        /// A reference to the prototype for a class of objects.
        /// </summary>
        abstract member prototype: obj with get
        /// <summary>
        /// Returns the prototype of an object.
        /// </summary>
        /// <param name="o">
        /// The object that references the prototype.
        /// </param>
        abstract member getPrototypeOf: o: obj -> obj

        /// <summary>
        /// Gets the own property descriptor of the specified object.
        /// An own property descriptor is one that is defined directly on the object and is not inherited from the object's prototype.
        /// </summary>
        /// <param name="o">
        /// Object that contains the property.
        /// </param>
        /// <param name="p">
        /// Name of the property.
        /// </param>
        abstract member getOwnPropertyDescriptor:
            o: obj * p: string -> TypeScript.PropertyDescriptor option

        /// <summary>
        /// Gets the own property descriptor of the specified object.
        /// An own property descriptor is one that is defined directly on the object and is not inherited from the object's prototype.
        /// </summary>
        /// <param name="o">
        /// Object that contains the property.
        /// </param>
        /// <param name="p">
        /// Name of the property.
        /// </param>
        abstract member getOwnPropertyDescriptor:
            o: obj * p: float -> TypeScript.PropertyDescriptor option

        /// <summary>
        /// Gets the own property descriptor of the specified object.
        /// An own property descriptor is one that is defined directly on the object and is not inherited from the object's prototype.
        /// </summary>
        /// <param name="o">
        /// Object that contains the property.
        /// </param>
        /// <param name="p">
        /// Name of the property.
        /// </param>
        abstract member getOwnPropertyDescriptor:
            o: obj * p: obj -> TypeScript.PropertyDescriptor option

        /// <summary>
        /// Returns the names of the own properties of an object. The own properties of an object are those that are defined directly
        /// on that object, and are not inherited from the object's prototype. The properties of an object include both fields (objects) and functions.
        /// </summary>
        /// <param name="o">
        /// Object that contains the own properties.
        /// </param>
        abstract member getOwnPropertyNames: o: obj -> ResizeArray<string>
        /// <summary>
        /// Creates an object that has the specified prototype or that has null prototype.
        /// Creates an object that has the specified prototype, and that optionally contains specified properties.
        /// </summary>
        /// <param name="o">
        /// Object to use as a prototype. May be null.
        /// </param>
        abstract member create: o: obj option -> obj
        /// <summary>
        /// Creates an object that has the specified prototype or that has null prototype.
        /// Creates an object that has the specified prototype, and that optionally contains specified properties.
        /// </summary>
        /// <param name="o">
        /// Object to use as a prototype. May be null
        /// </param>
        /// <param name="properties">
        /// JavaScript object that contains one or more property descriptors.
        /// </param>
        abstract member create: o: obj option * properties: obj -> obj

        /// <summary>
        /// Adds a property to an object, or modifies attributes of an existing property.
        /// </summary>
        /// <param name="o">
        /// Object on which to add or modify the property. This can be a native JavaScript object (that is, a user-defined object or a built in object) or a DOM object.
        /// </param>
        /// <param name="p">
        /// The property name.
        /// </param>
        /// <param name="attributes">
        /// Descriptor for the property. It can be for a data property or an accessor property.
        /// </param>
        abstract member defineProperty<'T> :
            o: 'T * p: string * attributes: ObjectConstructor.defineProperty.attributes -> 'T

        /// <summary>
        /// Adds a property to an object, or modifies attributes of an existing property.
        /// </summary>
        /// <param name="o">
        /// Object on which to add or modify the property. This can be a native JavaScript object (that is, a user-defined object or a built in object) or a DOM object.
        /// </param>
        /// <param name="p">
        /// The property name.
        /// </param>
        /// <param name="attributes">
        /// Descriptor for the property. It can be for a data property or an accessor property.
        /// </param>
        abstract member defineProperty<'T> :
            o: 'T * p: float * attributes: ObjectConstructor.defineProperty.attributes -> 'T

        /// <summary>
        /// Adds a property to an object, or modifies attributes of an existing property.
        /// </summary>
        /// <param name="o">
        /// Object on which to add or modify the property. This can be a native JavaScript object (that is, a user-defined object or a built in object) or a DOM object.
        /// </param>
        /// <param name="p">
        /// The property name.
        /// </param>
        /// <param name="attributes">
        /// Descriptor for the property. It can be for a data property or an accessor property.
        /// </param>
        abstract member defineProperty<'T> :
            o: 'T * p: obj * attributes: ObjectConstructor.defineProperty.attributes -> 'T

        /// <summary>
        /// Adds one or more properties to an object, and/or modifies attributes of existing properties.
        /// </summary>
        /// <param name="o">
        /// Object on which to add or modify the properties. This can be a native JavaScript object or a DOM object.
        /// </param>
        /// <param name="properties">
        /// JavaScript object that contains one or more descriptor objects. Each descriptor object describes a data property or an accessor property.
        /// </param>
        abstract member defineProperties<'T> : o: 'T * properties: obj -> 'T
        /// <summary>
        /// Prevents the modification of attributes of existing properties, and prevents the addition of new properties.
        /// </summary>
        /// <param name="o">
        /// Object on which to lock the attributes.
        /// </param>
        abstract member seal<'T> : o: 'T -> 'T
        /// <summary>
        /// Prevents the modification of existing property attributes and values, and prevents the addition of new properties.
        /// </summary>
        /// <param name="f">
        /// Object on which to lock the attributes.
        /// </param>
        abstract member freeze<'T> : f: 'T -> 'T
        /// <summary>
        /// Prevents the addition of new properties to an object.
        /// </summary>
        /// <param name="o">
        /// Object to make non-extensible.
        /// </param>
        abstract member preventExtensions<'T> : o: 'T -> 'T
        /// <summary>
        /// Returns true if existing property attributes cannot be modified in an object and new properties cannot be added to the object.
        /// </summary>
        /// <param name="o">
        /// Object to test.
        /// </param>
        abstract member isSealed: o: obj -> bool
        /// <summary>
        /// Returns true if existing property attributes and values cannot be modified in an object, and new properties cannot be added to the object.
        /// </summary>
        /// <param name="o">
        /// Object to test.
        /// </param>
        abstract member isFrozen: o: obj -> bool
        /// <summary>
        /// Returns a value that indicates whether new properties can be added to an object.
        /// </summary>
        /// <param name="o">
        /// Object to test.
        /// </param>
        abstract member isExtensible: o: obj -> bool

    [<AllowNullLiteral>]
    [<Interface>]
    type ReadonlyArray<'T> =
        inherit Iterable<'T>

        /// <summary>
        /// Returns the value of the first element in the array where predicate is true, and undefined
        /// otherwise.
        /// </summary>
        /// <param name="predicate">
        /// find calls predicate once for each element of the array, in ascending
        /// order, until it finds one where predicate returns true. If such an element is found, find
        /// immediately returns that element value. Otherwise, find returns undefined.
        /// </param>
        /// <param name="thisArg">
        /// If provided, it will be used as the this value for each invocation of
        /// predicate. If it is not provided, undefined is used instead.
        /// </param>
        abstract member find<'S> :
            predicate: ReadonlyArray.find.predicate<'T> * ?thisArg: obj -> 'S option

        /// <summary>
        /// Returns the value of the first element in the array where predicate is true, and undefined
        /// otherwise.
        /// </summary>
        abstract member find:
            predicate: ReadonlyArray.find.predicate_1<'T> * ?thisArg: obj -> 'T option

        /// <summary>
        /// Returns the index of the first element in the array where predicate is true, and -1
        /// otherwise.
        /// </summary>
        /// <param name="predicate">
        /// find calls predicate once for each element of the array, in ascending
        /// order, until it finds one where predicate returns true. If such an element is found,
        /// findIndex immediately returns that element index. Otherwise, findIndex returns -1.
        /// </param>
        /// <param name="thisArg">
        /// If provided, it will be used as the this value for each invocation of
        /// predicate. If it is not provided, undefined is used instead.
        /// </param>
        abstract member findIndex:
            predicate: ReadonlyArray.findIndex.predicate<'T> * ?thisArg: obj -> float

        /// <summary>
        /// Returns an iterable of key, value pairs for every entry in the array
        /// </summary>
        abstract member entries: unit -> TypeScript.IterableIterator<float * 'T>
        /// <summary>
        /// Returns an iterable of keys in the array
        /// </summary>
        abstract member keys: unit -> TypeScript.IterableIterator<float>
        /// <summary>
        /// Returns an iterable of values in the array
        /// </summary>
        abstract member values: unit -> TypeScript.IterableIterator<'T>
        /// <summary>
        /// Determines whether an array includes a certain element, returning true or false as appropriate.
        /// </summary>
        /// <param name="searchElement">
        /// The element to search for.
        /// </param>
        /// <param name="fromIndex">
        /// The position in this array at which to begin searching for searchElement.
        /// </param>
        abstract member includes: searchElement: 'T * ?fromIndex: float -> bool

        /// <summary>
        /// Calls a defined callback function on each element of an array. Then, flattens the result into
        /// a new array.
        /// This is identical to a map followed by flat with depth 1.
        /// </summary>
        /// <param name="callback">
        /// A function that accepts up to three arguments. The flatMap method calls the
        /// callback function one time for each element in the array.
        /// </param>
        /// <param name="thisArg">
        /// An object to which the this keyword can refer in the callback function. If
        /// thisArg is omitted, undefined is used as the this value.
        /// </param>
        abstract member flatMap<'U, 'This> :
            callback: ReadonlyArray.flatMap.callback<'U, 'T> * ?thisArg: 'This -> ResizeArray<'U>

        /// <summary>
        /// Returns a new array with all sub-array elements concatenated into it recursively up to the
        /// specified depth.
        /// </summary>
        /// <param name="depth">
        /// The maximum recursion depth
        /// </param>
        abstract member flat<'D> : ?depth: 'D -> ResizeArray<obj>
        /// <summary>
        /// Returns the item located at the specified index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the desired code unit. A negative index will count back from the last item.
        /// </param>
        abstract member at: index: float -> 'T option

        /// <summary>
        /// Returns the value of the last element in the array where predicate is true, and undefined
        /// otherwise.
        /// </summary>
        /// <param name="predicate">
        /// findLast calls predicate once for each element of the array, in descending
        /// order, until it finds one where predicate returns true. If such an element is found, findLast
        /// immediately returns that element value. Otherwise, findLast returns undefined.
        /// </param>
        /// <param name="thisArg">
        /// If provided, it will be used as the this value for each invocation of
        /// predicate. If it is not provided, undefined is used instead.
        /// </param>
        abstract member findLast<'S> :
            predicate: ReadonlyArray.findLast.predicate<'T> * ?thisArg: obj -> 'S option

        /// <summary>
        /// Returns the value of the last element in the array where predicate is true, and undefined
        /// otherwise.
        /// </summary>
        abstract member findLast:
            predicate: ReadonlyArray.findLast.predicate_1<'T> * ?thisArg: obj -> 'T option

        /// <summary>
        /// Returns the index of the last element in the array where predicate is true, and -1
        /// otherwise.
        /// </summary>
        /// <param name="predicate">
        /// findLastIndex calls predicate once for each element of the array, in descending
        /// order, until it finds one where predicate returns true. If such an element is found,
        /// findLastIndex immediately returns that element index. Otherwise, findLastIndex returns -1.
        /// </param>
        /// <param name="thisArg">
        /// If provided, it will be used as the this value for each invocation of
        /// predicate. If it is not provided, undefined is used instead.
        /// </param>
        abstract member findLastIndex:
            predicate: ReadonlyArray.findLastIndex.predicate<'T> * ?thisArg: obj -> float

        /// <summary>
        /// Copies the array and returns the copied array with all of its elements reversed.
        /// </summary>
        abstract member toReversed: unit -> ResizeArray<'T>

        /// <summary>
        /// Copies and sorts the array.
        /// </summary>
        /// <param name="compareFn">
        /// Function used to determine the order of the elements. It is expected to return
        /// a negative value if the first argument is less than the second argument, zero if they're equal, and a positive
        /// value otherwise. If omitted, the elements are sorted in ascending, ASCII character order.
        /// <code lang="ts">
        /// [11, 2, 22, 1].toSorted((a, b) => a - b) // [1, 2, 11, 22]
        /// </code>
        /// </param>
        abstract member toSorted:
            ?compareFn: ReadonlyArray.toSorted.compareFn<'T> -> ResizeArray<'T>

        /// <summary>
        /// Copies an array and removes elements while, if necessary, inserting new elements in their place, returning the remaining elements.
        /// Copies an array and removes elements while returning the remaining elements.
        /// </summary>
        /// <param name="start">
        /// The zero-based location in the array from which to start removing elements.
        /// </param>
        /// <param name="deleteCount">
        /// The number of elements to remove.
        /// </param>
        /// <param name="items">
        /// Elements to insert into the copied array in place of the deleted elements.
        /// </param>
        /// <returns>
        /// A copy of the original array with the remaining elements.
        /// </returns>
        abstract member toSpliced:
            start: float * deleteCount: float * [<ParamArray>] items: 'T[] -> ResizeArray<'T>

        /// <summary>
        /// Copies an array and removes elements while, if necessary, inserting new elements in their place, returning the remaining elements.
        /// Copies an array and removes elements while returning the remaining elements.
        /// </summary>
        /// <param name="start">
        /// The zero-based location in the array from which to start removing elements.
        /// </param>
        /// <param name="deleteCount">
        /// The number of elements to remove.
        /// </param>
        /// <returns>
        /// A copy of the original array with the remaining elements.
        /// </returns>
        abstract member toSpliced: start: float * ?deleteCount: float -> ResizeArray<'T>
        /// <summary>
        /// Copies an array, then overwrites the value at the provided index with the
        /// given value. If the index is negative, then it replaces from the end
        /// of the array
        /// </summary>
        /// <param name="index">
        /// The index of the value to overwrite. If the index is
        /// negative, then it replaces from the end of the array.
        /// </param>
        /// <param name="value">
        /// The value to insert into the copied array.
        /// </param>
        /// <returns>
        /// A copy of the original array with the inserted value.
        /// </returns>
        abstract member ``with``: index: float * value: 'T -> ResizeArray<'T>
        /// <summary>
        /// Gets the length of the array. This is a number one higher than the highest element defined in an array.
        /// </summary>
        abstract member length: float with get
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
            [<ParamArray>] items: TypeScript.ConcatArray<'T>[] -> ResizeArray<'T>

        /// <summary>
        /// Combines two or more arrays.
        /// </summary>
        /// <param name="items">
        /// Additional items to add to the end of array1.
        /// </param>
        abstract member concat:
            [<ParamArray>] items: U2<'T, TypeScript.ConcatArray<'T>>[] -> ResizeArray<'T>

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
        abstract member slice: ?start: float * ?``end``: float -> ResizeArray<'T>
        /// <summary>
        /// Returns the index of the first occurrence of a value in an array.
        /// </summary>
        /// <param name="searchElement">
        /// The value to locate in the array.
        /// </param>
        /// <param name="fromIndex">
        /// The array index at which to begin the search. If fromIndex is omitted, the search starts at index 0.
        /// </param>
        abstract member indexOf: searchElement: 'T * ?fromIndex: float -> float
        /// <summary>
        /// Returns the index of the last occurrence of a specified value in an array.
        /// </summary>
        /// <param name="searchElement">
        /// The value to locate in the array.
        /// </param>
        /// <param name="fromIndex">
        /// The array index at which to begin the search. If fromIndex is omitted, the search starts at the last index in the array.
        /// </param>
        abstract member lastIndexOf: searchElement: 'T * ?fromIndex: float -> float
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
        abstract member every: predicate: ReadonlyArray.every.predicate<'T> * ?thisArg: obj -> bool

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
            predicate: ReadonlyArray.every.predicate_1<'T> * ?thisArg: obj -> bool

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
        abstract member some: predicate: ReadonlyArray.some.predicate<'T> * ?thisArg: obj -> bool

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
            callbackfn: ReadonlyArray.forEach.callbackfn<'T> * ?thisArg: obj -> unit

        /// <summary>
        /// Calls a defined callback function on each element of an array, and returns an array that contains the results.
        /// </summary>
        /// <param name="callbackfn">
        /// A function that accepts up to three arguments. The map method calls the callbackfn function one time for each element in the array.
        /// </param>
        /// <param name="thisArg">
        /// An object to which the this keyword can refer in the callbackfn function. If thisArg is omitted, undefined is used as the this value.
        /// </param>
        abstract member map<'U> :
            callbackfn: ReadonlyArray.map.callbackfn<'U, 'T> * ?thisArg: obj -> ResizeArray<'U>

        /// <summary>
        /// Returns the elements of an array that meet the condition specified in a callback function.
        /// </summary>
        /// <param name="predicate">
        /// A function that accepts up to three arguments. The filter method calls the predicate function one time for each element in the array.
        /// </param>
        /// <param name="thisArg">
        /// An object to which the this keyword can refer in the predicate function. If thisArg is omitted, undefined is used as the this value.
        /// </param>
        abstract member filter<'S> :
            predicate: ReadonlyArray.filter.predicate<'T> * ?thisArg: obj -> ResizeArray<'S>

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
            predicate: ReadonlyArray.filter.predicate_1<'T> * ?thisArg: obj -> ResizeArray<'T>

        /// <summary>
        /// Calls the specified callback function for all the elements in an array. The return value of the callback function is the accumulated result, and is provided as an argument in the next call to the callback function.
        /// </summary>
        /// <param name="callbackfn">
        /// A function that accepts up to four arguments. The reduce method calls the callbackfn function one time for each element in the array.
        /// </param>
        /// <param name="initialValue">
        /// If initialValue is specified, it is used as the initial value to start the accumulation. The first call to the callbackfn function provides this value as an argument instead of an array value.
        /// </param>
        abstract member reduce: callbackfn: ReadonlyArray.reduce.callbackfn<'T> -> 'T

        /// <summary>
        /// Calls the specified callback function for all the elements in an array. The return value of the callback function is the accumulated result, and is provided as an argument in the next call to the callback function.
        /// </summary>
        abstract member reduce:
            callbackfn: ReadonlyArray.reduce.callbackfn<'T> * initialValue: 'T -> 'T

        /// <summary>
        /// Calls the specified callback function for all the elements in an array. The return value of the callback function is the accumulated result, and is provided as an argument in the next call to the callback function.
        /// </summary>
        /// <param name="callbackfn">
        /// A function that accepts up to four arguments. The reduce method calls the callbackfn function one time for each element in the array.
        /// </param>
        /// <param name="initialValue">
        /// If initialValue is specified, it is used as the initial value to start the accumulation. The first call to the callbackfn function provides this value as an argument instead of an array value.
        /// </param>
        abstract member reduce<'U> :
            callbackfn: ReadonlyArray.reduce.callbackfn_1<'U, 'T> * initialValue: 'U -> 'U

        /// <summary>
        /// Calls the specified callback function for all the elements in an array, in descending order. The return value of the callback function is the accumulated result, and is provided as an argument in the next call to the callback function.
        /// </summary>
        /// <param name="callbackfn">
        /// A function that accepts up to four arguments. The reduceRight method calls the callbackfn function one time for each element in the array.
        /// </param>
        /// <param name="initialValue">
        /// If initialValue is specified, it is used as the initial value to start the accumulation. The first call to the callbackfn function provides this value as an argument instead of an array value.
        /// </param>
        abstract member reduceRight: callbackfn: ReadonlyArray.reduceRight.callbackfn<'T> -> 'T

        /// <summary>
        /// Calls the specified callback function for all the elements in an array, in descending order. The return value of the callback function is the accumulated result, and is provided as an argument in the next call to the callback function.
        /// </summary>
        abstract member reduceRight:
            callbackfn: ReadonlyArray.reduceRight.callbackfn<'T> * initialValue: 'T -> 'T

        /// <summary>
        /// Calls the specified callback function for all the elements in an array, in descending order. The return value of the callback function is the accumulated result, and is provided as an argument in the next call to the callback function.
        /// </summary>
        /// <param name="callbackfn">
        /// A function that accepts up to four arguments. The reduceRight method calls the callbackfn function one time for each element in the array.
        /// </param>
        /// <param name="initialValue">
        /// If initialValue is specified, it is used as the initial value to start the accumulation. The first call to the callbackfn function provides this value as an argument instead of an array value.
        /// </param>
        abstract member reduceRight<'U> :
            callbackfn: ReadonlyArray.reduceRight.callbackfn_1<'U, 'T> * initialValue: 'U -> 'U

        [<EmitIndexer>]
        abstract member Item: n: int -> 'T with get

    [<AllowNullLiteral>]
    [<Interface>]
    type StringConstructor =
        /// <summary>
        /// Return the String value whose elements are, in order, the elements in the List elements.
        /// If length is 0, the empty string is returned.
        /// </summary>
        abstract member fromCodePoint: [<ParamArray>] codePoints: float[] -> string

        /// <summary>
        /// String.raw is usually used as a tag function of a Tagged Template String. When called as
        /// such, the first argument will be a well formed template call site object and the rest
        /// parameter will contain the substitution values. It can also be called directly, for example,
        /// to interleave strings and values from your own tag function, and in this case the only thing
        /// it needs from the first argument is the raw property.
        /// </summary>
        /// <param name="template">
        /// A well-formed template string call site representation.
        /// </param>
        /// <param name="substitutions">
        /// A set of substitution values.
        /// </param>
        abstract member raw:
            template: StringConstructor.raw.template * [<ParamArray>] substitutions: obj[] -> string

        [<EmitConstructor>]
        abstract member Create: ?value: obj -> string

        [<Emit("$0($1...)")>]
        abstract member Invoke: ?value: obj -> string

        abstract member prototype: string with get
        abstract member fromCharCode: [<ParamArray>] codes: float[] -> string

    [<AllowNullLiteral>]
    [<Interface>]
    type Generator<'T, 'TReturn, 'TNext> =
        inherit TypeScript.Iterator<'T, 'TReturn, 'TNext>
        inherit Iterable<'T>
        abstract member next: [<ParamArray>] args: obj[] -> TypeScript.IteratorResult<'T, 'TReturn>

        abstract member next:
            [<ParamArray>] args: 'TNext[] -> TypeScript.IteratorResult<'T, 'TReturn>

        abstract member ``return``: value: 'TReturn -> TypeScript.IteratorResult<'T, 'TReturn>
        abstract member throw: e: obj -> TypeScript.IteratorResult<'T, 'TReturn>

    [<AllowNullLiteral>]
    [<Interface>]
    type SymbolConstructor =
        /// <summary>
        /// A method that returns the default iterator for an object. Called by the semantics of the
        /// for-of statement.
        /// </summary>
        abstract member iterator: obj with get
        /// <summary>
        /// A reference to the prototype.
        /// </summary>
        abstract member prototype: obj with get

        [<Emit("$0($1...)")>]
        abstract member Invoke: ?description: U2<string, float> -> obj

        /// <summary>
        /// Returns a Symbol object from the global symbol registry matching the given key if found.
        /// Otherwise, returns a new symbol with this key.
        /// </summary>
        /// <param name="key">
        /// key to search for.
        /// </param>
        abstract member ``for``: key: string -> obj
        /// <summary>
        /// Returns a key from the global symbol registry matching the given Symbol if found.
        /// Otherwise, returns a undefined.
        /// </summary>
        /// <param name="sym">
        /// Symbol to find the key for.
        /// </param>
        abstract member keyFor: sym: obj -> string option
        /// <summary>
        /// A method that determines if a constructor object recognizes an object as one of the
        /// constructor’s instances. Called by the semantics of the instanceof operator.
        /// </summary>
        abstract member hasInstance: obj with get
        /// <summary>
        /// A Boolean value that if true indicates that an object should flatten to its array elements
        /// by Array.prototype.concat.
        /// </summary>
        abstract member isConcatSpreadable: obj with get
        /// <summary>
        /// A regular expression method that matches the regular expression against a string. Called
        /// by the String.prototype.match method.
        /// </summary>
        abstract member ``match``: obj with get
        /// <summary>
        /// A regular expression method that replaces matched substrings of a string. Called by the
        /// String.prototype.replace method.
        /// </summary>
        abstract member replace: obj with get
        /// <summary>
        /// A regular expression method that returns the index within a string that matches the
        /// regular expression. Called by the String.prototype.search method.
        /// </summary>
        abstract member search: obj with get
        /// <summary>
        /// A function valued property that is the constructor function that is used to create
        /// derived objects.
        /// </summary>
        abstract member species: obj with get
        /// <summary>
        /// A regular expression method that splits a string at the indices that match the regular
        /// expression. Called by the String.prototype.split method.
        /// </summary>
        abstract member split: obj with get
        /// <summary>
        /// A method that converts an object to a corresponding primitive value.
        /// Called by the ToPrimitive abstract operation.
        /// </summary>
        abstract member toPrimitive: obj with get
        /// <summary>
        /// A String value that is used in the creation of the default string description of an object.
        /// Called by the built-in method Object.prototype.toString.
        /// </summary>
        abstract member toStringTag: obj with get
        /// <summary>
        /// An Object whose truthy properties are properties that are excluded from the 'with'
        /// environment bindings of the associated objects.
        /// </summary>
        abstract member unscopables: obj with get
        /// <summary>
        /// A method that returns the default async iterator for an object. Called by the semantics of
        /// the for-await-of statement.
        /// </summary>
        abstract member asyncIterator: obj with get
        /// <summary>
        /// A regular expression method that matches the regular expression against a string. Called
        /// by the String.prototype.matchAll method.
        /// </summary>
        abstract member matchAll: obj with get
        abstract member metadata: obj with get
        /// <summary>
        /// A method that is used to release resources held by an object. Called by the semantics of the <c>using</c> statement.
        /// </summary>
        abstract member dispose: obj with get
        /// <summary>
        /// A method that is used to asynchronously release resources held by an object. Called by the semantics of the <c>await using</c> statement.
        /// </summary>
        abstract member asyncDispose: obj with get

    [<AllowNullLiteral>]
    [<Interface>]
    type IteratorYieldResult<'TYield> =
        abstract member ``done``: bool option with get, set
        abstract member value: 'TYield with get, set

    [<AllowNullLiteral>]
    [<Interface>]
    type IteratorReturnResult<'TReturn> =
        abstract member ``done``: bool with get, set
        abstract member value: 'TReturn with get, set

    type IteratorResult<'T, 'TReturn> =
        U2<TypeScript.IteratorYieldResult<'T>, TypeScript.IteratorReturnResult<'TReturn>>

    [<AllowNullLiteral>]
    [<Interface>]
    type Iterator<'T, 'TReturn, 'TNext> =
        abstract member next: [<ParamArray>] args: obj[] -> TypeScript.IteratorResult<'T, 'TReturn>

        abstract member next:
            [<ParamArray>] args: 'TNext[] -> TypeScript.IteratorResult<'T, 'TReturn>

        abstract member ``return``: ?value: 'TReturn -> TypeScript.IteratorResult<'T, 'TReturn>
        abstract member throw: ?e: obj -> TypeScript.IteratorResult<'T, 'TReturn>

    [<AllowNullLiteral>]
    [<Interface>]
    type IterableIterator<'T> =
        inherit TypeScript.Iterator<'T>
        inherit Iterable<'T>

    [<AllowNullLiteral>]
    [<Interface>]
    type ProxyHandler<'T> =
        /// <summary>
        /// A trap method for a function call.
        /// </summary>
        /// <param name="target">
        /// The original callable object which is being proxied.
        /// </param>
        abstract member apply: target: 'T * thisArg: obj * argArray: ResizeArray<obj> -> obj

        /// <summary>
        /// A trap for the <c>new</c> operator.
        /// </summary>
        /// <param name="target">
        /// The original object which is being proxied.
        /// </param>
        /// <param name="newTarget">
        /// The constructor that was originally called.
        /// </param>
        abstract member construct:
            target: 'T * argArray: ResizeArray<obj> * newTarget: Action -> obj

        /// <summary>
        /// A trap for <c>Object.defineProperty()</c>.
        /// </summary>
        /// <param name="target">
        /// The original object which is being proxied.
        /// </param>
        /// <returns>
        /// A <c>Boolean</c> indicating whether or not the property has been defined.
        /// </returns>
        abstract member defineProperty:
            target: 'T * property: string * attributes: TypeScript.PropertyDescriptor -> bool

        /// <summary>
        /// A trap for <c>Object.defineProperty()</c>.
        /// </summary>
        /// <param name="target">
        /// The original object which is being proxied.
        /// </param>
        /// <returns>
        /// A <c>Boolean</c> indicating whether or not the property has been defined.
        /// </returns>
        abstract member defineProperty:
            target: 'T * property: obj * attributes: TypeScript.PropertyDescriptor -> bool

        /// <summary>
        /// A trap for the <c>delete</c> operator.
        /// </summary>
        /// <param name="target">
        /// The original object which is being proxied.
        /// </param>
        /// <param name="p">
        /// The name or <c>Symbol</c> of the property to delete.
        /// </param>
        /// <returns>
        /// A <c>Boolean</c> indicating whether or not the property was deleted.
        /// </returns>
        abstract member deleteProperty: target: 'T * p: string -> bool
        /// <summary>
        /// A trap for the <c>delete</c> operator.
        /// </summary>
        /// <param name="target">
        /// The original object which is being proxied.
        /// </param>
        /// <param name="p">
        /// The name or <c>Symbol</c> of the property to delete.
        /// </param>
        /// <returns>
        /// A <c>Boolean</c> indicating whether or not the property was deleted.
        /// </returns>
        abstract member deleteProperty: target: 'T * p: obj -> bool
        /// <summary>
        /// A trap for getting a property value.
        /// </summary>
        /// <param name="target">
        /// The original object which is being proxied.
        /// </param>
        /// <param name="p">
        /// The name or <c>Symbol</c> of the property to get.
        /// </param>
        /// <param name="receiver">
        /// The proxy or an object that inherits from the proxy.
        /// </param>
        abstract member get: target: 'T * p: string * receiver: obj -> obj
        /// <summary>
        /// A trap for getting a property value.
        /// </summary>
        /// <param name="target">
        /// The original object which is being proxied.
        /// </param>
        /// <param name="p">
        /// The name or <c>Symbol</c> of the property to get.
        /// </param>
        /// <param name="receiver">
        /// The proxy or an object that inherits from the proxy.
        /// </param>
        abstract member get: target: 'T * p: obj * receiver: obj -> obj

        /// <summary>
        /// A trap for <c>Object.getOwnPropertyDescriptor()</c>.
        /// </summary>
        /// <param name="target">
        /// The original object which is being proxied.
        /// </param>
        /// <param name="p">
        /// The name of the property whose description should be retrieved.
        /// </param>
        abstract member getOwnPropertyDescriptor:
            target: 'T * p: string -> TypeScript.PropertyDescriptor option

        /// <summary>
        /// A trap for <c>Object.getOwnPropertyDescriptor()</c>.
        /// </summary>
        /// <param name="target">
        /// The original object which is being proxied.
        /// </param>
        /// <param name="p">
        /// The name of the property whose description should be retrieved.
        /// </param>
        abstract member getOwnPropertyDescriptor:
            target: 'T * p: obj -> TypeScript.PropertyDescriptor option

        /// <summary>
        /// A trap for the <c>[[GetPrototypeOf]]</c> internal method.
        /// </summary>
        /// <param name="target">
        /// The original object which is being proxied.
        /// </param>
        abstract member getPrototypeOf: target: 'T -> obj option
        /// <summary>
        /// A trap for the <c>in</c> operator.
        /// </summary>
        /// <param name="target">
        /// The original object which is being proxied.
        /// </param>
        /// <param name="p">
        /// The name or <c>Symbol</c> of the property to check for existence.
        /// </param>
        abstract member has: target: 'T * p: string -> bool
        /// <summary>
        /// A trap for the <c>in</c> operator.
        /// </summary>
        /// <param name="target">
        /// The original object which is being proxied.
        /// </param>
        /// <param name="p">
        /// The name or <c>Symbol</c> of the property to check for existence.
        /// </param>
        abstract member has: target: 'T * p: obj -> bool
        /// <summary>
        /// A trap for <c>Object.isExtensible()</c>.
        /// </summary>
        /// <param name="target">
        /// The original object which is being proxied.
        /// </param>
        abstract member isExtensible: target: 'T -> bool
        /// <summary>
        /// A trap for <c>Reflect.ownKeys()</c>.
        /// </summary>
        /// <param name="target">
        /// The original object which is being proxied.
        /// </param>
        abstract member ownKeys: target: 'T -> TypeScript.ArrayLike<U2<string, obj>>
        /// <summary>
        /// A trap for <c>Object.preventExtensions()</c>.
        /// </summary>
        /// <param name="target">
        /// The original object which is being proxied.
        /// </param>
        abstract member preventExtensions: target: 'T -> bool
        /// <summary>
        /// A trap for setting a property value.
        /// </summary>
        /// <param name="target">
        /// The original object which is being proxied.
        /// </param>
        /// <param name="p">
        /// The name or <c>Symbol</c> of the property to set.
        /// </param>
        /// <param name="receiver">
        /// The object to which the assignment was originally directed.
        /// </param>
        /// <returns>
        /// A <c>Boolean</c> indicating whether or not the property was set.
        /// </returns>
        abstract member set: target: 'T * p: string * newValue: obj * receiver: obj -> bool
        /// <summary>
        /// A trap for setting a property value.
        /// </summary>
        /// <param name="target">
        /// The original object which is being proxied.
        /// </param>
        /// <param name="p">
        /// The name or <c>Symbol</c> of the property to set.
        /// </param>
        /// <param name="receiver">
        /// The object to which the assignment was originally directed.
        /// </param>
        /// <returns>
        /// A <c>Boolean</c> indicating whether or not the property was set.
        /// </returns>
        abstract member set: target: 'T * p: obj * newValue: obj * receiver: obj -> bool
        /// <summary>
        /// A trap for <c>Object.setPrototypeOf()</c>.
        /// </summary>
        /// <param name="target">
        /// The original object which is being proxied.
        /// </param>
        /// <param name="newPrototype">
        /// The object's new prototype or <c>null</c>.
        /// </param>
        abstract member setPrototypeOf: target: 'T * v: obj option -> bool

    [<AllowNullLiteral>]
    [<Interface>]
    type ProxyConstructor =
        /// <summary>
        /// Creates a revocable Proxy object.
        /// </summary>
        /// <param name="target">
        /// A target object to wrap with Proxy.
        /// </param>
        /// <param name="handler">
        /// An object whose properties define the behavior of Proxy when an operation is attempted on it.
        /// </param>
        abstract member revocable<'T> :
            target: 'T * handler: TypeScript.ProxyHandler<'T> -> ProxyConstructor.revocable<'T>

        [<EmitConstructor>]
        abstract member Create: target: 'T * handler: TypeScript.ProxyHandler<'T> -> 'T

    module Reflect =

        [<AbstractClass>]
        [<Erase>]
        type Exports =
            /// <summary>
            /// Calls the function with the specified object as the this value
            /// and the elements of specified array as the arguments.
            /// </summary>
            /// <param name="target">
            /// The function to call.
            /// </param>
            /// <param name="thisArgument">
            /// The object to be used as the this object.
            /// </param>
            /// <param name="argumentsList">
            /// An array of argument values to be passed to the function.
            /// </param>
            [<Emit("$0.apply($1...)")>]
            abstract member apply<'T, 'A, 'R> :
                target: System.Delegate * thisArgument: 'T * argumentsList: 'A -> 'R

            [<Emit("$0.apply($1...)")>]
            abstract member apply:
                target: Action * thisArgument: obj * argumentsList: TypeScript.ArrayLike<obj> -> obj

            /// <summary>
            /// Constructs the target with the elements of specified array as the arguments
            /// and the specified constructor as the <c>new.target</c> value.
            /// </summary>
            /// <param name="target">
            /// The constructor to invoke.
            /// </param>
            /// <param name="argumentsList">
            /// An array of argument values to be passed to the constructor.
            /// </param>
            /// <param name="newTarget">
            /// The constructor to be used as the <c>new.target</c> object.
            /// </param>
            [<Emit("$0.construct($1...)")>]
            abstract member construct<'A, 'R> :
                target: Exports.construct.target<'R> *
                argumentsList: 'A *
                ?newTarget: Exports.construct.newTarget ->
                    'R

            [<Emit("$0.construct($1...)")>]
            abstract member construct:
                target: Action * argumentsList: TypeScript.ArrayLike<obj> * ?newTarget: Action ->
                    obj

            /// <summary>
            /// Adds a property to an object, or modifies attributes of an existing property.
            /// </summary>
            /// <param name="target">
            /// Object on which to add or modify the property. This can be a native JavaScript object
            /// (that is, a user-defined object or a built in object) or a DOM object.
            /// </param>
            /// <param name="propertyKey">
            /// The property name.
            /// </param>
            /// <param name="attributes">
            /// Descriptor for the property. It can be for a data property or an accessor property.
            /// </param>
            [<Emit("$0.defineProperty($1...)")>]
            abstract member defineProperty:
                target: obj * propertyKey: string * attributes: Exports.defineProperty.attributes ->
                    bool

            /// <summary>
            /// Adds a property to an object, or modifies attributes of an existing property.
            /// </summary>
            /// <param name="target">
            /// Object on which to add or modify the property. This can be a native JavaScript object
            /// (that is, a user-defined object or a built in object) or a DOM object.
            /// </param>
            /// <param name="propertyKey">
            /// The property name.
            /// </param>
            /// <param name="attributes">
            /// Descriptor for the property. It can be for a data property or an accessor property.
            /// </param>
            [<Emit("$0.defineProperty($1...)")>]
            abstract member defineProperty:
                target: obj * propertyKey: float * attributes: Exports.defineProperty.attributes ->
                    bool

            /// <summary>
            /// Adds a property to an object, or modifies attributes of an existing property.
            /// </summary>
            /// <param name="target">
            /// Object on which to add or modify the property. This can be a native JavaScript object
            /// (that is, a user-defined object or a built in object) or a DOM object.
            /// </param>
            /// <param name="propertyKey">
            /// The property name.
            /// </param>
            /// <param name="attributes">
            /// Descriptor for the property. It can be for a data property or an accessor property.
            /// </param>
            [<Emit("$0.defineProperty($1...)")>]
            abstract member defineProperty:
                target: obj * propertyKey: obj * attributes: Exports.defineProperty.attributes ->
                    bool

            /// <summary>
            /// Removes a property from an object, equivalent to <c>delete target[propertyKey]</c>,
            /// except it won't throw if <c>target[propertyKey]</c> is non-configurable.
            /// </summary>
            /// <param name="target">
            /// Object from which to remove the own property.
            /// </param>
            /// <param name="propertyKey">
            /// The property name.
            /// </param>
            [<Emit("$0.deleteProperty($1...)")>]
            abstract member deleteProperty: target: obj * propertyKey: string -> bool

            /// <summary>
            /// Removes a property from an object, equivalent to <c>delete target[propertyKey]</c>,
            /// except it won't throw if <c>target[propertyKey]</c> is non-configurable.
            /// </summary>
            /// <param name="target">
            /// Object from which to remove the own property.
            /// </param>
            /// <param name="propertyKey">
            /// The property name.
            /// </param>
            [<Emit("$0.deleteProperty($1...)")>]
            abstract member deleteProperty: target: obj * propertyKey: float -> bool

            /// <summary>
            /// Removes a property from an object, equivalent to <c>delete target[propertyKey]</c>,
            /// except it won't throw if <c>target[propertyKey]</c> is non-configurable.
            /// </summary>
            /// <param name="target">
            /// Object from which to remove the own property.
            /// </param>
            /// <param name="propertyKey">
            /// The property name.
            /// </param>
            [<Emit("$0.deleteProperty($1...)")>]
            abstract member deleteProperty: target: obj * propertyKey: obj -> bool

            /// <summary>
            /// Gets the property of target, equivalent to <c>target[propertyKey]</c> when <c>receiver === target</c>.
            /// </summary>
            /// <param name="target">
            /// Object that contains the property on itself or in its prototype chain.
            /// </param>
            /// <param name="propertyKey">
            /// The property name.
            /// </param>
            /// <param name="receiver">
            /// The reference to use as the <c>this</c> value in the getter function,
            /// if <c>target[propertyKey]</c> is an accessor property.
            /// </param>
            [<Emit("$0.get($1...)")>]
            abstract member get<'P> : target: obj * propertyKey: 'P * ?receiver: obj -> obj

            /// <summary>
            /// Gets the own property descriptor of the specified object.
            /// An own property descriptor is one that is defined directly on the object and is not inherited from the object's prototype.
            /// </summary>
            /// <param name="target">
            /// Object that contains the property.
            /// </param>
            /// <param name="propertyKey">
            /// The property name.
            /// </param>
            [<Emit("$0.getOwnPropertyDescriptor($1...)")>]
            abstract member getOwnPropertyDescriptor<'P> :
                target: obj * propertyKey: 'P -> TypeScript.TypedPropertyDescriptor<obj> option

            /// <summary>
            /// Returns the prototype of an object.
            /// </summary>
            /// <param name="target">
            /// The object that references the prototype.
            /// </param>
            [<Emit("$0.getPrototypeOf($1...)")>]
            abstract member getPrototypeOf: target: obj -> obj option

            /// <summary>
            /// Equivalent to <c>propertyKey in target</c>.
            /// </summary>
            /// <param name="target">
            /// Object that contains the property on itself or in its prototype chain.
            /// </param>
            /// <param name="propertyKey">
            /// Name of the property.
            /// </param>
            [<Emit("$0.has($1...)")>]
            abstract member has: target: obj * propertyKey: string -> bool

            /// <summary>
            /// Equivalent to <c>propertyKey in target</c>.
            /// </summary>
            /// <param name="target">
            /// Object that contains the property on itself or in its prototype chain.
            /// </param>
            /// <param name="propertyKey">
            /// Name of the property.
            /// </param>
            [<Emit("$0.has($1...)")>]
            abstract member has: target: obj * propertyKey: float -> bool

            /// <summary>
            /// Equivalent to <c>propertyKey in target</c>.
            /// </summary>
            /// <param name="target">
            /// Object that contains the property on itself or in its prototype chain.
            /// </param>
            /// <param name="propertyKey">
            /// Name of the property.
            /// </param>
            [<Emit("$0.has($1...)")>]
            abstract member has: target: obj * propertyKey: obj -> bool

            /// <summary>
            /// Returns a value that indicates whether new properties can be added to an object.
            /// </summary>
            /// <param name="target">
            /// Object to test.
            /// </param>
            [<Emit("$0.isExtensible($1...)")>]
            abstract member isExtensible: target: obj -> bool

            /// <summary>
            /// Returns the string and symbol keys of the own properties of an object. The own properties of an object
            /// are those that are defined directly on that object, and are not inherited from the object's prototype.
            /// </summary>
            /// <param name="target">
            /// Object that contains the own properties.
            /// </param>
            [<Emit("$0.ownKeys($1...)")>]
            abstract member ownKeys: target: obj -> ResizeArray<U2<string, obj>>

            /// <summary>
            /// Prevents the addition of new properties to an object.
            /// </summary>
            /// <param name="target">
            /// Object to make non-extensible.
            /// </param>
            /// <returns>
            /// Whether the object has been made non-extensible.
            /// </returns>
            [<Emit("$0.preventExtensions($1...)")>]
            abstract member preventExtensions: target: obj -> bool

            /// <summary>
            /// Sets the property of target, equivalent to <c>target[propertyKey] = value</c> when <c>receiver === target</c>.
            /// </summary>
            /// <param name="target">
            /// Object that contains the property on itself or in its prototype chain.
            /// </param>
            /// <param name="propertyKey">
            /// Name of the property.
            /// </param>
            /// <param name="receiver">
            /// The reference to use as the <c>this</c> value in the setter function,
            /// if <c>target[propertyKey]</c> is an accessor property.
            /// </param>
            [<Emit("$0.set($1...)")>]
            abstract member set<'P> :
                target: obj * propertyKey: 'P * value: obj * ?receiver: obj -> bool

            [<Emit("$0.set($1...)")>]
            abstract member set:
                target: obj * propertyKey: string * value: obj * ?receiver: obj -> bool

            [<Emit("$0.set($1...)")>]
            abstract member set:
                target: obj * propertyKey: float * value: obj * ?receiver: obj -> bool

            [<Emit("$0.set($1...)")>]
            abstract member set:
                target: obj * propertyKey: obj * value: obj * ?receiver: obj -> bool

            /// <summary>
            /// Sets the prototype of a specified object o to object proto or null.
            /// </summary>
            /// <param name="target">
            /// The object to change its prototype.
            /// </param>
            /// <param name="proto">
            /// The value of the new prototype or null.
            /// </param>
            /// <returns>
            /// Whether setting the prototype was successful.
            /// </returns>
            [<Emit("$0.setPrototypeOf($1...)")>]
            abstract member setPrototypeOf: target: obj * proto: obj option -> bool

        module Exports =

            module construct =

                [<AllowNullLiteral>]
                [<Interface>]
                type target<'R> =
                    [<EmitConstructor>]
                    abstract member Create: [<ParamArray>] args: 'A[] -> 'R

                [<AllowNullLiteral>]
                [<Interface>]
                type newTarget =
                    [<EmitConstructor>]
                    abstract member Create: [<ParamArray>] args: obj[] -> obj

            module defineProperty =

                [<AllowNullLiteral>]
                [<Interface>]
                type attributes =
                    abstract member configurable: bool option with get, set
                    abstract member enumerable: bool option with get, set
                    abstract member value: obj option with get, set
                    abstract member writable: bool option with get, set
                    abstract member get: unit -> obj
                    abstract member set: v: obj -> unit

    [<AllowNullLiteral>]
    [<Interface>]
    type Date =
        /// <summary>
        /// Converts a date and time to a string by using the current or specified locale.
        /// Returns a value as a string value appropriate to the host environment's current locale.
        /// </summary>
        /// <param name="locales">
        /// A locale string, array of locale strings, Intl.Locale object, or array of Intl.Locale objects that contain one or more language or locale tags. If you include more than one locale string, list them in descending order of priority so that the first entry is the preferred locale. If you omit this parameter, the default locale of the JavaScript runtime is used.
        /// </param>
        /// <param name="options">
        /// An object that contains one or more properties that specify comparison options.
        /// </param>
        abstract member toLocaleString: unit -> string
        /// <summary>
        /// Converts a date and time to a string by using the current or specified locale.
        /// Returns a value as a string value appropriate to the host environment's current locale.
        /// </summary>
        /// <param name="locales">
        /// A locale string, array of locale strings, Intl.Locale object, or array of Intl.Locale objects that contain one or more language or locale tags. If you include more than one locale string, list them in descending order of priority so that the first entry is the preferred locale. If you omit this parameter, the default locale of the JavaScript runtime is used.
        /// </param>
        /// <param name="options">
        /// An object that contains one or more properties that specify comparison options.
        /// </param>
        abstract member toLocaleString: locales: obj * ?options: obj -> string
        /// <summary>
        /// Converts a date and time to a string by using the current or specified locale.
        /// Returns a value as a string value appropriate to the host environment's current locale.
        /// </summary>
        /// <param name="locales">
        /// A locale string, array of locale strings, Intl.Locale object, or array of Intl.Locale objects that contain one or more language or locale tags. If you include more than one locale string, list them in descending order of priority so that the first entry is the preferred locale. If you omit this parameter, the default locale of the JavaScript runtime is used.
        /// </param>
        /// <param name="options">
        /// An object that contains one or more properties that specify comparison options.
        /// </param>
        abstract member toLocaleString: locales: ResizeArray<obj> * ?options: obj -> string
        /// <summary>
        /// Converts a date to a string by using the current or specified locale.
        /// Returns a date as a string value appropriate to the host environment's current locale.
        /// </summary>
        /// <param name="locales">
        /// A locale string, array of locale strings, Intl.Locale object, or array of Intl.Locale objects that contain one or more language or locale tags. If you include more than one locale string, list them in descending order of priority so that the first entry is the preferred locale. If you omit this parameter, the default locale of the JavaScript runtime is used.
        /// </param>
        /// <param name="options">
        /// An object that contains one or more properties that specify comparison options.
        /// </param>
        abstract member toLocaleDateString: unit -> string
        /// <summary>
        /// Converts a date to a string by using the current or specified locale.
        /// Returns a date as a string value appropriate to the host environment's current locale.
        /// </summary>
        /// <param name="locales">
        /// A locale string, array of locale strings, Intl.Locale object, or array of Intl.Locale objects that contain one or more language or locale tags. If you include more than one locale string, list them in descending order of priority so that the first entry is the preferred locale. If you omit this parameter, the default locale of the JavaScript runtime is used.
        /// </param>
        /// <param name="options">
        /// An object that contains one or more properties that specify comparison options.
        /// </param>
        abstract member toLocaleDateString: locales: obj * ?options: obj -> string
        /// <summary>
        /// Converts a date to a string by using the current or specified locale.
        /// Returns a date as a string value appropriate to the host environment's current locale.
        /// </summary>
        /// <param name="locales">
        /// A locale string, array of locale strings, Intl.Locale object, or array of Intl.Locale objects that contain one or more language or locale tags. If you include more than one locale string, list them in descending order of priority so that the first entry is the preferred locale. If you omit this parameter, the default locale of the JavaScript runtime is used.
        /// </param>
        /// <param name="options">
        /// An object that contains one or more properties that specify comparison options.
        /// </param>
        abstract member toLocaleDateString: locales: ResizeArray<obj> * ?options: obj -> string
        /// <summary>
        /// Converts a time to a string by using the current or specified locale.
        /// Returns a time as a string value appropriate to the host environment's current locale.
        /// </summary>
        /// <param name="locales">
        /// A locale string, array of locale strings, Intl.Locale object, or array of Intl.Locale objects that contain one or more language or locale tags. If you include more than one locale string, list them in descending order of priority so that the first entry is the preferred locale. If you omit this parameter, the default locale of the JavaScript runtime is used.
        /// </param>
        /// <param name="options">
        /// An object that contains one or more properties that specify comparison options.
        /// </param>
        abstract member toLocaleTimeString: unit -> string
        /// <summary>
        /// Converts a time to a string by using the current or specified locale.
        /// Returns a time as a string value appropriate to the host environment's current locale.
        /// </summary>
        /// <param name="locales">
        /// A locale string, array of locale strings, Intl.Locale object, or array of Intl.Locale objects that contain one or more language or locale tags. If you include more than one locale string, list them in descending order of priority so that the first entry is the preferred locale. If you omit this parameter, the default locale of the JavaScript runtime is used.
        /// </param>
        /// <param name="options">
        /// An object that contains one or more properties that specify comparison options.
        /// </param>
        abstract member toLocaleTimeString: locales: obj * ?options: obj -> string
        /// <summary>
        /// Converts a time to a string by using the current or specified locale.
        /// Returns a time as a string value appropriate to the host environment's current locale.
        /// </summary>
        /// <param name="locales">
        /// A locale string, array of locale strings, Intl.Locale object, or array of Intl.Locale objects that contain one or more language or locale tags. If you include more than one locale string, list them in descending order of priority so that the first entry is the preferred locale. If you omit this parameter, the default locale of the JavaScript runtime is used.
        /// </param>
        /// <param name="options">
        /// An object that contains one or more properties that specify comparison options.
        /// </param>
        abstract member toLocaleTimeString: locales: ResizeArray<obj> * ?options: obj -> string
        /// <summary>
        /// Returns a string representation of a date. The format of the string depends on the locale.
        /// </summary>
        abstract member toString: unit -> string
        /// <summary>
        /// Returns a date as a string value.
        /// </summary>
        abstract member toDateString: unit -> string
        /// <summary>
        /// Returns a time as a string value.
        /// </summary>
        abstract member toTimeString: unit -> string
        /// <summary>
        /// Returns the stored time value in milliseconds since midnight, January 1, 1970 UTC.
        /// </summary>
        abstract member valueOf: unit -> float
        /// <summary>
        /// Returns the stored time value in milliseconds since midnight, January 1, 1970 UTC.
        /// </summary>
        abstract member getTime: unit -> float
        /// <summary>
        /// Gets the year, using local time.
        /// </summary>
        abstract member getFullYear: unit -> float
        /// <summary>
        /// Gets the year using Universal Coordinated Time (UTC).
        /// </summary>
        abstract member getUTCFullYear: unit -> float
        /// <summary>
        /// Gets the month, using local time.
        /// </summary>
        abstract member getMonth: unit -> float
        /// <summary>
        /// Gets the month of a Date object using Universal Coordinated Time (UTC).
        /// </summary>
        abstract member getUTCMonth: unit -> float
        /// <summary>
        /// Gets the day-of-the-month, using local time.
        /// </summary>
        abstract member getDate: unit -> float
        /// <summary>
        /// Gets the day-of-the-month, using Universal Coordinated Time (UTC).
        /// </summary>
        abstract member getUTCDate: unit -> float
        /// <summary>
        /// Gets the day of the week, using local time.
        /// </summary>
        abstract member getDay: unit -> float
        /// <summary>
        /// Gets the day of the week using Universal Coordinated Time (UTC).
        /// </summary>
        abstract member getUTCDay: unit -> float
        /// <summary>
        /// Gets the hours in a date, using local time.
        /// </summary>
        abstract member getHours: unit -> float
        /// <summary>
        /// Gets the hours value in a Date object using Universal Coordinated Time (UTC).
        /// </summary>
        abstract member getUTCHours: unit -> float
        /// <summary>
        /// Gets the minutes of a Date object, using local time.
        /// </summary>
        abstract member getMinutes: unit -> float
        /// <summary>
        /// Gets the minutes of a Date object using Universal Coordinated Time (UTC).
        /// </summary>
        abstract member getUTCMinutes: unit -> float
        /// <summary>
        /// Gets the seconds of a Date object, using local time.
        /// </summary>
        abstract member getSeconds: unit -> float
        /// <summary>
        /// Gets the seconds of a Date object using Universal Coordinated Time (UTC).
        /// </summary>
        abstract member getUTCSeconds: unit -> float
        /// <summary>
        /// Gets the milliseconds of a Date, using local time.
        /// </summary>
        abstract member getMilliseconds: unit -> float
        /// <summary>
        /// Gets the milliseconds of a Date object using Universal Coordinated Time (UTC).
        /// </summary>
        abstract member getUTCMilliseconds: unit -> float
        /// <summary>
        /// Gets the difference in minutes between the time on the local computer and Universal Coordinated Time (UTC).
        /// </summary>
        abstract member getTimezoneOffset: unit -> float
        /// <summary>
        /// Sets the date and time value in the Date object.
        /// </summary>
        /// <param name="time">
        /// A numeric value representing the number of elapsed milliseconds since midnight, January 1, 1970 GMT.
        /// </param>
        abstract member setTime: time: float -> float
        /// <summary>
        /// Sets the milliseconds value in the Date object using local time.
        /// </summary>
        /// <param name="ms">
        /// A numeric value equal to the millisecond value.
        /// </param>
        abstract member setMilliseconds: ms: float -> float
        /// <summary>
        /// Sets the milliseconds value in the Date object using Universal Coordinated Time (UTC).
        /// </summary>
        /// <param name="ms">
        /// A numeric value equal to the millisecond value.
        /// </param>
        abstract member setUTCMilliseconds: ms: float -> float
        /// <summary>
        /// Sets the seconds value in the Date object using local time.
        /// </summary>
        /// <param name="sec">
        /// A numeric value equal to the seconds value.
        /// </param>
        /// <param name="ms">
        /// A numeric value equal to the milliseconds value.
        /// </param>
        abstract member setSeconds: sec: float * ?ms: float -> float
        /// <summary>
        /// Sets the seconds value in the Date object using Universal Coordinated Time (UTC).
        /// </summary>
        /// <param name="sec">
        /// A numeric value equal to the seconds value.
        /// </param>
        /// <param name="ms">
        /// A numeric value equal to the milliseconds value.
        /// </param>
        abstract member setUTCSeconds: sec: float * ?ms: float -> float
        /// <summary>
        /// Sets the minutes value in the Date object using local time.
        /// </summary>
        /// <param name="min">
        /// A numeric value equal to the minutes value.
        /// </param>
        /// <param name="sec">
        /// A numeric value equal to the seconds value.
        /// </param>
        /// <param name="ms">
        /// A numeric value equal to the milliseconds value.
        /// </param>
        abstract member setMinutes: min: float * ?sec: float * ?ms: float -> float
        /// <summary>
        /// Sets the minutes value in the Date object using Universal Coordinated Time (UTC).
        /// </summary>
        /// <param name="min">
        /// A numeric value equal to the minutes value.
        /// </param>
        /// <param name="sec">
        /// A numeric value equal to the seconds value.
        /// </param>
        /// <param name="ms">
        /// A numeric value equal to the milliseconds value.
        /// </param>
        abstract member setUTCMinutes: min: float * ?sec: float * ?ms: float -> float
        /// <summary>
        /// Sets the hour value in the Date object using local time.
        /// </summary>
        /// <param name="hours">
        /// A numeric value equal to the hours value.
        /// </param>
        /// <param name="min">
        /// A numeric value equal to the minutes value.
        /// </param>
        /// <param name="sec">
        /// A numeric value equal to the seconds value.
        /// </param>
        /// <param name="ms">
        /// A numeric value equal to the milliseconds value.
        /// </param>
        abstract member setHours: hours: float * ?min: float * ?sec: float * ?ms: float -> float
        /// <summary>
        /// Sets the hours value in the Date object using Universal Coordinated Time (UTC).
        /// </summary>
        /// <param name="hours">
        /// A numeric value equal to the hours value.
        /// </param>
        /// <param name="min">
        /// A numeric value equal to the minutes value.
        /// </param>
        /// <param name="sec">
        /// A numeric value equal to the seconds value.
        /// </param>
        /// <param name="ms">
        /// A numeric value equal to the milliseconds value.
        /// </param>
        abstract member setUTCHours: hours: float * ?min: float * ?sec: float * ?ms: float -> float
        /// <summary>
        /// Sets the numeric day-of-the-month value of the Date object using local time.
        /// </summary>
        /// <param name="date">
        /// A numeric value equal to the day of the month.
        /// </param>
        abstract member setDate: date: float -> float
        /// <summary>
        /// Sets the numeric day of the month in the Date object using Universal Coordinated Time (UTC).
        /// </summary>
        /// <param name="date">
        /// A numeric value equal to the day of the month.
        /// </param>
        abstract member setUTCDate: date: float -> float
        /// <summary>
        /// Sets the month value in the Date object using local time.
        /// </summary>
        /// <param name="month">
        /// A numeric value equal to the month. The value for January is 0, and other month values follow consecutively.
        /// </param>
        /// <param name="date">
        /// A numeric value representing the day of the month. If this value is not supplied, the value from a call to the getDate method is used.
        /// </param>
        abstract member setMonth: month: float * ?date: float -> float
        /// <summary>
        /// Sets the month value in the Date object using Universal Coordinated Time (UTC).
        /// </summary>
        /// <param name="month">
        /// A numeric value equal to the month. The value for January is 0, and other month values follow consecutively.
        /// </param>
        /// <param name="date">
        /// A numeric value representing the day of the month. If it is not supplied, the value from a call to the getUTCDate method is used.
        /// </param>
        abstract member setUTCMonth: month: float * ?date: float -> float
        /// <summary>
        /// Sets the year of the Date object using local time.
        /// </summary>
        /// <param name="year">
        /// A numeric value for the year.
        /// </param>
        /// <param name="month">
        /// A zero-based numeric value for the month (0 for January, 11 for December). Must be specified if numDate is specified.
        /// </param>
        /// <param name="date">
        /// A numeric value equal for the day of the month.
        /// </param>
        abstract member setFullYear: year: float * ?month: float * ?date: float -> float
        /// <summary>
        /// Sets the year value in the Date object using Universal Coordinated Time (UTC).
        /// </summary>
        /// <param name="year">
        /// A numeric value equal to the year.
        /// </param>
        /// <param name="month">
        /// A numeric value equal to the month. The value for January is 0, and other month values follow consecutively. Must be supplied if numDate is supplied.
        /// </param>
        /// <param name="date">
        /// A numeric value equal to the day of the month.
        /// </param>
        abstract member setUTCFullYear: year: float * ?month: float * ?date: float -> float
        /// <summary>
        /// Returns a date converted to a string using Universal Coordinated Time (UTC).
        /// </summary>
        abstract member toUTCString: unit -> string
        /// <summary>
        /// Returns a date as a string value in ISO format.
        /// </summary>
        abstract member toISOString: unit -> string
        /// <summary>
        /// Used by the JSON.stringify method to enable the transformation of an object's data for JavaScript Object Notation (JSON) serialization.
        /// </summary>
        abstract member toJSON: ?key: obj -> string
        /// <summary>
        /// Converts a date and time to a string by using the current or specified locale.
        /// Returns a value as a string value appropriate to the host environment's current locale.
        /// </summary>
        /// <param name="locales">
        /// A locale string or array of locale strings that contain one or more language or locale tags. If you include more than one locale string, list them in descending order of priority so that the first entry is the preferred locale. If you omit this parameter, the default locale of the JavaScript runtime is used.
        /// </param>
        /// <param name="options">
        /// An object that contains one or more properties that specify comparison options.
        /// </param>
        abstract member toLocaleString: locales: string * ?options: obj -> string
        /// <summary>
        /// Converts a date and time to a string by using the current or specified locale.
        /// Returns a value as a string value appropriate to the host environment's current locale.
        /// </summary>
        /// <param name="locales">
        /// A locale string or array of locale strings that contain one or more language or locale tags. If you include more than one locale string, list them in descending order of priority so that the first entry is the preferred locale. If you omit this parameter, the default locale of the JavaScript runtime is used.
        /// </param>
        /// <param name="options">
        /// An object that contains one or more properties that specify comparison options.
        /// </param>
        abstract member toLocaleString: locales: ResizeArray<string> * ?options: obj -> string
        /// <summary>
        /// Converts a date to a string by using the current or specified locale.
        /// Returns a date as a string value appropriate to the host environment's current locale.
        /// </summary>
        /// <param name="locales">
        /// A locale string or array of locale strings that contain one or more language or locale tags. If you include more than one locale string, list them in descending order of priority so that the first entry is the preferred locale. If you omit this parameter, the default locale of the JavaScript runtime is used.
        /// </param>
        /// <param name="options">
        /// An object that contains one or more properties that specify comparison options.
        /// </param>
        abstract member toLocaleDateString: locales: string * ?options: obj -> string
        /// <summary>
        /// Converts a date to a string by using the current or specified locale.
        /// Returns a date as a string value appropriate to the host environment's current locale.
        /// </summary>
        /// <param name="locales">
        /// A locale string or array of locale strings that contain one or more language or locale tags. If you include more than one locale string, list them in descending order of priority so that the first entry is the preferred locale. If you omit this parameter, the default locale of the JavaScript runtime is used.
        /// </param>
        /// <param name="options">
        /// An object that contains one or more properties that specify comparison options.
        /// </param>
        abstract member toLocaleDateString: locales: ResizeArray<string> * ?options: obj -> string
        /// <summary>
        /// Converts a time to a string by using the current or specified locale.
        /// Returns a time as a string value appropriate to the host environment's current locale.
        /// </summary>
        /// <param name="locales">
        /// A locale string or array of locale strings that contain one or more language or locale tags. If you include more than one locale string, list them in descending order of priority so that the first entry is the preferred locale. If you omit this parameter, the default locale of the JavaScript runtime is used.
        /// </param>
        /// <param name="options">
        /// An object that contains one or more properties that specify comparison options.
        /// </param>
        abstract member toLocaleTimeString: locales: string * ?options: obj -> string
        /// <summary>
        /// Converts a time to a string by using the current or specified locale.
        /// Returns a time as a string value appropriate to the host environment's current locale.
        /// </summary>
        /// <param name="locales">
        /// A locale string or array of locale strings that contain one or more language or locale tags. If you include more than one locale string, list them in descending order of priority so that the first entry is the preferred locale. If you omit this parameter, the default locale of the JavaScript runtime is used.
        /// </param>
        /// <param name="options">
        /// An object that contains one or more properties that specify comparison options.
        /// </param>
        abstract member toLocaleTimeString: locales: ResizeArray<string> * ?options: obj -> string

    [<AllowNullLiteral>]
    [<Interface>]
    type SharedArrayBuffer =
        /// <summary>
        /// Read-only. The length of the ArrayBuffer (in bytes).
        /// </summary>
        abstract member byteLength: float with get
        /// <summary>
        /// Returns a section of an SharedArrayBuffer.
        /// </summary>
        abstract member slice: ``begin``: float * ?``end``: float -> TypeScript.SharedArrayBuffer

    [<AllowNullLiteral>]
    [<Interface>]
    type SharedArrayBufferConstructor =
        abstract member prototype: TypeScript.SharedArrayBuffer with get

        [<EmitConstructor>]
        abstract member Create: byteLength: float -> TypeScript.SharedArrayBuffer

    [<AllowNullLiteral>]
    [<Interface>]
    type ArrayBufferTypes =
        abstract member SharedArrayBuffer: TypeScript.SharedArrayBuffer with get, set
        abstract member ArrayBuffer: JS.ArrayBuffer with get, set

    [<Global>]
    [<AllowNullLiteral>]
    type ErrorOptions [<ParamObject; Emit("$0")>] (?cause: obj) =

        member val cause: obj option = nativeOnly with get, set

    type PropertyKey = U3<string, float, obj>

    [<AllowNullLiteral>]
    [<Interface>]
    type PropertyDescriptor =
        abstract member configurable: bool option with get, set
        abstract member enumerable: bool option with get, set
        abstract member value: obj option with get, set
        abstract member writable: bool option with get, set
        abstract member get: unit -> obj
        abstract member set: v: obj -> unit

    [<AllowNullLiteral>]
    [<Interface>]
    type PropertyDescriptorMap =
        [<EmitIndexer>]
        abstract member Item: key: TypeScript.PropertyKey -> TypeScript.PropertyDescriptor with get, set

    [<AllowNullLiteral>]
    [<Interface>]
    type BooleanConstructor =
        [<EmitConstructor>]
        abstract member Create: ?value: obj -> bool

        [<Emit("$0($1...)")>]
        abstract member Invoke: ?value: 'T -> bool

        abstract member prototype: bool with get

    [<AllowNullLiteral>]
    [<Interface>]
    type TemplateStringsArray =
        inherit TypeScript.ReadonlyArray<string>
        abstract member raw: ReadonlyArray<string> with get

    [<AllowNullLiteral>]
    [<Interface>]
    type ConcatArray<'T> =
        abstract member length: float with get

        [<EmitIndexer>]
        abstract member Item: n: int -> 'T with get

        abstract member join: ?separator: string -> string
        abstract member slice: ?start: float * ?``end``: float -> ResizeArray<'T>

    [<AllowNullLiteral>]
    [<Interface>]
    type TypedPropertyDescriptor<'T> =
        abstract member enumerable: bool option with get, set
        abstract member configurable: bool option with get, set
        abstract member writable: bool option with get, set
        abstract member value: 'T option with get, set
        abstract member get: (unit -> 'T) option with get, set
        abstract member set: ('T -> unit) option with get, set

    [<AllowNullLiteral>]
    [<Interface>]
    type PromiseLike<'T> =
        /// <summary>
        /// Attaches callbacks for the resolution and/or rejection of the Promise.
        /// </summary>
        /// <param name="onfulfilled">
        /// The callback to execute when the Promise is resolved.
        /// </param>
        /// <param name="onrejected">
        /// The callback to execute when the Promise is rejected.
        /// </param>
        /// <returns>
        /// A Promise for the completion of which ever callback is executed.
        /// </returns>
        abstract member ``then``<'TResult1, 'TResult2> :
            ?onfulfilled: ('T -> U2<'TResult1, TypeScript.PromiseLike<'TResult1>>) *
            ?onrejected: (obj -> U2<'TResult2, TypeScript.PromiseLike<'TResult2>>) ->
                TypeScript.PromiseLike<U2<'TResult1, 'TResult2>>

        /// <summary>
        /// Attaches callbacks for the resolution and/or rejection of the Promise.
        /// </summary>
        /// <param name="onfulfilled">
        /// The callback to execute when the Promise is resolved.
        /// </param>
        /// <param name="onrejected">
        /// The callback to execute when the Promise is rejected.
        /// </param>
        /// <returns>
        /// A Promise for the completion of which ever callback is executed.
        /// </returns>
        abstract member ``then``:
            ?onfulfilled: ('T -> U2<'T, TypeScript.PromiseLike<'T>>) *
            ?onrejected: (obj -> U2<obj, TypeScript.PromiseLike<obj>>) ->
                TypeScript.PromiseLike<U2<'T, obj>>

        /// <summary>
        /// Attaches callbacks for the resolution and/or rejection of the Promise.
        /// </summary>
        /// <param name="onfulfilled">
        /// The callback to execute when the Promise is resolved.
        /// </param>
        /// <param name="onrejected">
        /// The callback to execute when the Promise is rejected.
        /// </param>
        /// <returns>
        /// A Promise for the completion of which ever callback is executed.
        /// </returns>
        abstract member ``then``: unit -> TypeScript.PromiseLike<U2<'T, obj>>

    [<AllowNullLiteral>]
    [<Interface>]
    type ArrayLike<'T> =
        abstract member length: float with get

        [<EmitIndexer>]
        abstract member Item: n: int -> 'T with get

    type ArrayBufferLike = U2<TypeScript.SharedArrayBuffer, JS.ArrayBuffer>

    type Generator<'T, 'TReturn> = Generator<'T, 'TReturn, obj>

    type Generator<'T> = Generator<'T, obj, obj>

    type Generator = Generator<obj, obj, obj>

    type IteratorResult<'T> = IteratorResult<'T, obj>

    type Iterator<'T, 'TReturn> = Iterator<'T, 'TReturn, obj>

    type Iterator<'T> = Iterator<'T, obj, obj>

    type ProxyHandler = ProxyHandler<obj>

    module ReadonlyMap =

        module forEach =

            type callbackfn<'K, 'V> =
                delegate of value: 'V * key: 'K * map: TypeScript.ReadonlyMap<'K, 'V> -> unit

    module ReadonlySet =

        module forEach =

            type callbackfn<'T> =
                delegate of value: 'T * value2: 'T * set: TypeScript.ReadonlySet<'T> -> unit

    module ObjectConstructor =

        [<AllowNullLiteral>]
        [<Interface>]
        type fromEntries<'T> =
            [<EmitIndexer>]
            abstract member Item: k: string -> 'T with get, set

        [<AllowNullLiteral>]
        [<Interface>]
        type fromEntries_1 =
            [<EmitIndexer>]
            abstract member Item: k: string -> obj with get, set

        module values =

            [<AllowNullLiteral>]
            [<Interface>]
            type o<'T> =
                [<EmitIndexer>]
                abstract member Item: s: string -> 'T with get, set

        module entries =

            [<AllowNullLiteral>]
            [<Interface>]
            type o<'T> =
                [<EmitIndexer>]
                abstract member Item: s: string -> 'T with get, set

        module defineProperty =

            [<AllowNullLiteral>]
            [<Interface>]
            type attributes =
                abstract member configurable: bool option with get, set
                abstract member enumerable: bool option with get, set
                abstract member value: obj option with get, set
                abstract member writable: bool option with get, set
                abstract member get: unit -> obj
                abstract member set: v: obj -> unit

    module ReadonlyArray =

        module find =

            type predicate<'T> = delegate of value: 'T * index: float * obj: ResizeArray<'T> -> bool

            type predicate_1<'T> =
                delegate of value: 'T * index: float * obj: ResizeArray<'T> -> unit

        module findIndex =

            type predicate<'T> = delegate of value: 'T * index: float * obj: ResizeArray<'T> -> unit

        module flatMap =

            type callback<'U, 'T> =
                delegate of
                    value: 'T * index: float * array: ResizeArray<'T> ->
                        U2<'U, TypeScript.ReadonlyArray<'U>>

        module findLast =

            type predicate<'T> =
                delegate of value: 'T * index: float * array: ResizeArray<'T> -> bool

            type predicate_1<'T> =
                delegate of value: 'T * index: float * array: ResizeArray<'T> -> unit

        module findLastIndex =

            type predicate<'T> =
                delegate of value: 'T * index: float * array: ResizeArray<'T> -> unit

        module toSorted =

            type compareFn<'T> = delegate of a: 'T * b: 'T -> float

        module every =

            type predicate<'T> =
                delegate of value: 'T * index: float * array: ResizeArray<'T> -> bool

            type predicate_1<'T> =
                delegate of value: 'T * index: float * array: ResizeArray<'T> -> unit

        module some =

            type predicate<'T> =
                delegate of value: 'T * index: float * array: ResizeArray<'T> -> unit

        module forEach =

            type callbackfn<'T> =
                delegate of value: 'T * index: float * array: ResizeArray<'T> -> unit

        module map =

            type callbackfn<'U, 'T> =
                delegate of value: 'T * index: float * array: ResizeArray<'T> -> 'U

        module filter =

            type predicate<'T> =
                delegate of value: 'T * index: float * array: ResizeArray<'T> -> bool

            type predicate_1<'T> =
                delegate of value: 'T * index: float * array: ResizeArray<'T> -> unit

        module reduce =

            type callbackfn<'T> =
                delegate of
                    previousValue: 'T *
                    currentValue: 'T *
                    currentIndex: float *
                    array: ResizeArray<'T> ->
                        'T

            type callbackfn_1<'U, 'T> =
                delegate of
                    previousValue: 'U *
                    currentValue: 'T *
                    currentIndex: float *
                    array: ResizeArray<'T> ->
                        'U

        module reduceRight =

            type callbackfn<'T> =
                delegate of
                    previousValue: 'T *
                    currentValue: 'T *
                    currentIndex: float *
                    array: ResizeArray<'T> ->
                        'T

            type callbackfn_1<'U, 'T> =
                delegate of
                    previousValue: 'U *
                    currentValue: 'T *
                    currentIndex: float *
                    array: ResizeArray<'T> ->
                        'U

    module StringConstructor =

        module raw =

            [<Global>]
            [<AllowNullLiteral>]
            type template private () =

                [<ParamObject; Emit("$0")>]
                new(raw: ReadonlyArray<string>) = template ()

                [<ParamObject; Emit("$0")>]
                new(raw: TypeScript.ArrayLike<string>) = template ()

                member val raw: U2<ReadonlyArray<string>, TypeScript.ArrayLike<string>> =
                    nativeOnly with get, set

    module ProxyConstructor =

        [<Global>]
        [<AllowNullLiteral>]
        type revocable<'T> [<ParamObject; Emit("$0")>] (proxy: 'T, revoke: (unit -> unit)) =

            member val proxy: 'T = nativeOnly with get, set
            member val revoke: (unit -> unit) = nativeOnly with get, set
