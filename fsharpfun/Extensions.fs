namespace FSharpFun.Extensions

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

