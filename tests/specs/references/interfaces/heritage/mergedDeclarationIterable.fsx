module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

type Iterable<'T> = Collections.Generic.IEnumerable<'T>

[<AllowNullLiteral>]
[<Interface>]
type Element =
    /// <summary>
    /// The **<c>tagName</c>** read-only property of the Element interface returns the tag name of the element on which it's called.
    ///
    /// [MDN Reference](https://developer.mozilla.org/docs/Web/API/Element/tagName)
    /// </summary>
    abstract member tagName: string with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (tagName: string) : Element = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type HTMLCollectionBase =
    inherit Iterable<Element>
    /// <summary>
    /// The **<c>HTMLCollection.length</c>** property returns the number of items in a HTMLCollection.
    ///
    /// [MDN Reference](https://developer.mozilla.org/docs/Web/API/HTMLCollection/length)
    /// </summary>
    abstract member length: float with get, set

[<AllowNullLiteral>]
[<Interface>]
type HTMLCollectionOf<'T> =
    inherit HTMLCollectionBase
    /// <summary>
    /// The HTMLCollection method **<c>item()</c>** returns the element located at the specified offset into the collection.
    ///
    /// [MDN Reference](https://developer.mozilla.org/docs/Web/API/HTMLCollection/item)
    /// </summary>
    abstract member item: index: float -> 'T option

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
