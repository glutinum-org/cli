[<RequireQualifiedAccess>]
module Router

open System
open type Glutinum.LzString.Exports

// Makes writing url segments more natural
let inline (</>) a b = a + "/" + string b
let inline (<?>) a b = a + "?" + string b

[<RequireQualifiedAccess>]
type Route = Editors of typeScriptCode: string option

let toHash (route: Route) =
    let segmentPath =
        match route with
        | Route.Editors typeScriptCode ->
            match typeScriptCode with
            | Some code ->
                "editors"
                <?> "typeScriptCode=" + compressToEncodedURIComponent code
            | None -> "editors"

    "#" </> segmentPath

// Needs to be open here otherwise </> is shadowed by our inline operator
open Elmish.UrlParser
open Elmish.Navigation
open Feliz

let lzEncodedURIComponentParam name =
    customParam name (Option.map decompressFromEncodedURIComponent)

let routeParser: Parser<Route -> Route, Route> =
    oneOf [
        map
            Route.Editors
            (s "editors" <?> lzEncodedURIComponentParam "typeScriptCode")

        // Default route
        map (Route.Editors None) top
    ]

/// <summary>Generate React <c>href</c> properties</summary>
/// <param name="route">Route to transform</param>
/// <returns></returns>
let href route = prop.href (toHash route)

let modifyUrl route = route |> toHash |> Navigation.modifyUrl

let newUrl route = route |> toHash |> Navigation.newUrl

let modifyLocation route =
    Browser.Dom.window.location.href <- toHash route
