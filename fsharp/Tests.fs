module Tests

open Xunit

type Maybe<'a> =
    | Just of 'a
    | Nothing

let map fn opt =
    match opt with
    | Just x -> Just(fn x)
    | Nothing -> Nothing

let bind fn opt =
    match opt with
    | Just x -> fn x
    | Nothing -> Nothing

let apply fn opt=
    match (fn, opt) with
    | Just fn, Just opt ->  Just(fn opt)
    | _ , Nothing -> Nothing
    | Nothing, _ -> Nothing

let getCustomer =
    function
    | 0 -> Nothing
    | x -> Just(sprintf "Me with id %d" x)

let parseName =
    function
    | x when x = "Me" -> Just("You with id")
    | _ -> Nothing

let getCustomer' = bind getCustomer
let parseName' = bind parseName

// let save = printf "Saving name %s"
// let error = printf "Error saving name %s"

// let result =
//     getCustomer 1
//     |> function
//     | Just x -> save
//     | Nothing -> error

[<Fact>]
let ``My test`` () = Assert.True(true)
