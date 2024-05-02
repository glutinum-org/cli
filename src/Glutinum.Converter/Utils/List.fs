[<RequireQualifiedAccess>]
module List

open System

let removeConsecutiveEmptyLines list =
    let rec loop acc =
        function
        | [] -> List.rev acc
        | "" :: "" :: rest -> loop acc rest
        | x :: rest -> loop (x :: acc) rest

    loop [] list

let trimEmptyLines list =
    list
    |> List.skipWhile String.IsNullOrEmpty
    |> List.rev
    |> List.skipWhile String.IsNullOrEmpty
    |> List.rev
