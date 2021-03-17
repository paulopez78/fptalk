//https://fsharpforfunandprofit.com/posts/elevated-world-5/
module Program

open System
open System.Net.Http

module Async =
    let map f xAsync =
        async {
            let! x = xAsync
            return f x
        }

    let retn x = async { return x }

    let apply fAsync xAsync =
        async {
            let! fChild = Async.StartChild fAsync
            let! xChild = Async.StartChild xAsync

            let! f = fChild
            let! x = xChild

            return f x
        }

    let bind f xAsync =
        async {
            let! x = xAsync
            return! f x
        }

module Result =
    let retn x = Ok x

    let apply fResult xResult =
        match fResult, xResult with
        | Ok f, Ok x -> Ok(f x)
        | Error errs, Ok _ -> Error errs
        | Ok _, Error errs -> Error errs
        | Error errs1, Error errs2 -> Error(List.concat [ errs1; errs2 ])

/// type alias (optional)
type AsyncResult<'a, 'b> = Async<Result<'a, 'b>>

/// functions for AsyncResult
module AsyncResult =

    let map f = f |> Result.map |> Async.map

    let retn x = x |> Result.retn |> Async.retn

    let apply fAsyncResult xAsyncResult =
        fAsyncResult
        |> Async.bind (fun fResult ->
            xAsyncResult
            |> Async.map (fun xResult -> Result.apply fResult xResult))

    let bind f xAsyncResult =
        async {
            let! xResult = xAsyncResult

            match xResult with
            | Ok x -> return! f x
            | Error err -> return (Error err)
        }

module List =
    let cons head tail = head :: tail

    let rec traverseAsyncA f list =
        let (<*>) = Async.apply
        let retn = Async.retn

        let initState = retn []
        let folder head tail = retn cons <*> (f head) <*> tail

        List.foldBack folder list initState

    let sequenceAsyncA x = traverseAsyncA id x

    let rec traverseResultA f list =
        let (<*>) = Result.apply
        let retn = Result.Ok

        let initState = retn []
        let folder head tail = retn cons <*> (f head) <*> tail

        List.foldBack folder list initState

    let sequenceResultA x = traverseResultA id x

    let rec traverseAsyncResultM f list =
        let (>>=) x f = AsyncResult.bind f x
        let retn = AsyncResult.retn

        // right fold over the list
        let initState = retn []

        let folder head tail =
            f head
            >>= (fun h -> tail >>= (fun t -> retn (cons h t)))

        List.foldBack folder list initState

    let sequenceAsyncResultM x = traverseAsyncResultM id x

    let rec traverseAsyncResultA f list =
        let (<*>) = AsyncResult.apply
        let retn = AsyncResult.retn

        let initState = retn []
        let folder head tail = retn cons <*> (f head) <*> tail

        List.foldBack folder list initState

    let sequenceAsyncResultA x = traverseAsyncResultA id x

[<Measure>]
type ms

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

[<EntryPoint>]
let main _ =
    let client = new HttpClient()
    client.Timeout <- TimeSpan.FromMilliseconds(2000.0)

    let goodUrls =
        [ "https://google.es"
          "https://microsoft.com"
          "https://bing.com" ]

    let badUrls =
        [ "https://google.txt"
          "https://microsoft.docx"
          "https://bing.pdf" ]

    let good f = goodUrls |> largestPageSizeM'' client
    let goodA = goodUrls |> largestPageSizeA'' client
    
    let badM = badUrls |> largestPageSizeM'' client
    let badA = badUrls |> largestPageSizeA'' client
               
    let run result = result |> Async.RunSynchronously |> showContentSizeResult

    good largestPageSizeM''
    |> run
    
    badM
    |> run
    
    goodA
    |> run
    
    badA
    |> run

    0
