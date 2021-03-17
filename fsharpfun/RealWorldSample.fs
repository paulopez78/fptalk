module FSharpFun.Samples

open System
open System.Net.Http
open FSharpFun.Extensions

type UriContent = UriContent of System.Uri * string

type UriContentSize = UriContentSize of System.Uri * int

let getUriContent (client: HttpClient) (uri: Uri) =
    async {
        try
            let! response = client.GetAsync(uri) |> Async.AwaitTask
            response.EnsureSuccessStatusCode |> ignore

            let! content =
                response.Content.ReadAsStringAsync()
                |> Async.AwaitTask

            return Ok(UriContent(uri, content))
        with ex -> return Error [ ex.Message ]
    }

let showContentResult =
    function
    | Ok (UriContent (uri, html)) -> printfn $"Success: {uri.Host} First 100 chars: {html.Substring(0, 100)}"
    | Error x -> printfn $"Failure: {x}"

let makeContentSize (UriContent (uri, html)) =
    if System.String.IsNullOrEmpty(html) then
        Error [ "empty page" ]
    else
        let uriContentSize = UriContentSize(uri, html.Length)
        Ok uriContentSize

let showContentSizeResult result =
    match result with
    | Ok (UriContentSize (uri, len)) -> printfn "SUCCESS: [%s] Content size is %i" uri.Host len
    | Error errs -> printfn "FAILURE: %A" errs

let getUriContentSize client uri =
    uri
    |> getUriContent client
    |> (Result.bind makeContentSize |> Async.map)

let maxContentSize list =
    let contentSize (UriContentSize (_, len)) = len
    list |> List.maxBy contentSize

let largestPageSizeA client urls =
    urls
    |> List.map Uri
    |> List.map (getUriContentSize client)
    |> List.sequenceAsyncA
    |> Async.map List.sequenceResultA
    |> Async.map (Result.map maxContentSize)

let largestPageSizeA' client urls =
    urls
    |> List.traverseAsyncA (Uri >> getUriContentSize client)
    |> Async.map (List.sequenceResultA >> Result.map maxContentSize)

let largestPageSizeA'' client urls =
    urls
    |> List.map (Uri >> getUriContentSize client)
    |> List.sequenceAsyncResultA
    |> AsyncResult.map maxContentSize

let largestPageSizeM client urls =
    urls
    |> List.map Uri
    |> List.map (getUriContentSize client)
    |> List.sequenceAsyncA
    |> Async.map List.sequenceResultA
    |> Async.map (Result.map maxContentSize)

let largestPageSizeM' client urls =
    urls
    |> List.map Uri
    |> List.map (getUriContentSize client)
    |> List.sequenceAsyncA
    |> Async.map List.sequenceResultA
    |> Async.map (Result.map maxContentSize)

let largestPageSizeM'' client urls =
    urls
    |> List.map (Uri >> getUriContentSize client)
    |> List.sequenceAsyncResultM
    |> AsyncResult.map maxContentSize
