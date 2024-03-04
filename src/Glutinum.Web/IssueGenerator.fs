module Glutinum.Web.IssueGenerator

open Browser
open Glutinum.Converter

let private baseUrl = "https://github.com/glutinum-org/cli/issues/new"

let createUrl (typeScriptCode: string) (fSharpCode: string) =
    let hashUrl =
        typeScriptCode |> Some |> Router.Route.Editors |> Router.toHash

    let toolUrl =
        [
            window.location.protocol
            window.location.host
            window.location.pathname
            hashUrl
        ]
        |> String.concat ""

    let issueBody =
        $"""
Issue created from [Glutinum Tool](%s{toolUrl})

**Glutinum version -** %s{Prelude.VERSION}

**TypeScript**

```ts
%s{typeScriptCode}
```

**FSharp**

```fs
%s{fSharpCode}
```

**Problem description**

<!-- Please describe the problem you are facing below this line -->

        """
        // Remove leading and trailing whitespaces
        |> _.Trim()
        // Add new lines making it easier to write the problem description
        |> (fun str -> str + "\n\n")
        |> window.encodeURIComponent

    $"{baseUrl}?body=%s{issueBody}"
