//https://fsharpforfunandprofit.com/posts/elevated-world-5/

module fsharpfun.Program

open System
open System.Net.Http
open FSharpFun.Samples

[<EntryPoint>]
let main args =
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
