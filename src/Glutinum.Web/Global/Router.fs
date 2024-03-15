[<RequireQualifiedAccess>]
module Router

open System
open type Glutinum.LzString.Exports

// Makes writing url segments more natural
let inline (</>) a b = a + "/" + string b
let inline (<?>) a b = a + "?" + string b

[<RequireQualifiedAccess>]
type EditorsRoute =
    | FSharpCode of typeScriptCode: string option
    | GlueAST of typeScriptCode: string option

    static member ToSegments(route: EditorsRoute) =
        match route with
        | FSharpCode typeScriptCode ->
            match typeScriptCode with
            | Some code ->
                "fsharp-code"
                <?> "typeScriptCode=" + compressToEncodedURIComponent code
            | None -> "fsharp-code"

        | GlueAST typeScriptCode ->
            match typeScriptCode with
            | Some code ->
                "glue-ast"
                <?> "typeScriptCode=" + compressToEncodedURIComponent code
            | None -> "glue-ast"

[<RequireQualifiedAccess>]
type Route =
    | Editors of EditorsRoute

    static member ToSegments(route: Route) =
        match route with
        | Editors typeScriptCode ->
            "editors" </> EditorsRoute.ToSegments typeScriptCode

let toHash (route: Route) = "#" </> Route.ToSegments route

// Needs to be open here otherwise </> is shadowed by our inline operator
open Elmish.UrlParser
open Elmish.Navigation
open Feliz

let lzEncodedURIComponentParam name =
    customParam name (Option.map decompressFromEncodedURIComponent)

let routeParser: Parser<Route -> Route, Route> =
    oneOf [
        map
            (EditorsRoute.FSharpCode >> Route.Editors)
            (s "editors" </> s "fsharp-code"
             <?> lzEncodedURIComponentParam "typeScriptCode")

        map
            (EditorsRoute.GlueAST >> Route.Editors)
            (s "editors" </> s "glue-ast"
             <?> lzEncodedURIComponentParam "typeScriptCode")

        // Default route
        map (None |> EditorsRoute.FSharpCode |> Route.Editors) top
    ]

/// <summary>Generate React <c>href</c> properties</summary>
/// <param name="route">Route to transform</param>
/// <returns></returns>
let href route = prop.href (toHash route)

let modifyUrl route = route |> toHash |> Navigation.modifyUrl

let newUrl route = route |> toHash |> Navigation.newUrl

let modifyLocation route =
    Browser.Dom.window.location.href <- toHash route
