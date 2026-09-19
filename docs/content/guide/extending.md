---
title: Extending a binding
---

A generated binding is faithful to the declarations. Some APIs are still awkward from F#, and the fix is a hand-written file compiled after the generated one, not an edit of the generated file.

## A second file

Keep the generated file as it is, so it can be regenerated. Put the helpers in a file of their own and compile it after:

```xml title="MyApp.fsproj"
<Compile Include="Glutinum.Express.fs" />
<Compile Include="Glutinum.Express.Extensions.fs" />
```

## An adapter

Express routes take a `RequestHandler` delegate of three parameters, the last one the `next` callback. An adapter accepts the two-parameter lambda most handlers are:

```fsharp title="Glutinum.Express.Extensions.fs"
module Glutinum.Express.Extensions

open System
open Fable.Core
open Glutinum.Express

[<Erase>]
type ExpressAdapter =
    static member inline RequestHandler
        (handle: Func<ExpressServeStaticCore.Request, ExpressServeStaticCore.Response, unit>)
        : Express.RequestHandler
        =
        Express.RequestHandler(fun request response _ -> handle.Invoke(request, response))

    static member inline Middleware (middleware: Connect.createServer_.NextHandleFunction) : Express.RequestHandler =
        unbox middleware
```

```fsharp
app.get ("/users/:id", ExpressAdapter.RequestHandler(fun request response ->
    response.json users |> ignore
))
```

## Extension members

F# extension members add methods to a generated interface without touching it:

```fsharp
type Glutinum.Leaflet.Map with
    member this.CenterOn(latitude: float, longitude: float) =
        this.setView (L.latLng (latitude, longitude), this.getZoom ()) |> ignore
```

## When to change the generator instead

A helper hides a shape that is wrong for everyone. If the generated member cannot be called at all, or a type is `obj` where the declaration names one, [report it](https://github.com/glutinum-org/cli/issues) with the declaration reduced to a few lines.
