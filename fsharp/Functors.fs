module Functors

open Xunit

type Maybe<'a> = 
    | Nothing 
    | Just of 'a

let lift f =
    function
    | Just x -> Just(f x)
    | Nothing -> Nothing

let map = lift 


let getYearsOfExperience = Just 3


let level = function
    | x when x > 10 -> "Rockstar"
    | x when x > 8  -> "Ninja"
    | x when x > 5  -> "Senior"
    | x when x > 3  -> "Mid"
    | x when x > 0  -> "Junior"
    | _      -> "No idea what I'm doing"

let promote = function 
    | "Ninja"  -> "Rockstar"
    | "Senior" -> "Ninja"
    | "Mid"    -> "Senior"
    | "Junior" -> "Mid"
    | _        -> "Junior"


let years = function 
    | "Rockstar" -> 12
    | "Ninja"    -> 10
    | "Senior"   -> 8
    | "Mid"      -> 5
    | "Junior"   -> 2
    | _          -> 0 

let isSenior x = x > 5

let isSenior' = lift isSenior

getYearsOfExperience |> lift level |> lift promote |> lift years |> isSenior' |> ignore


[<Fact>]
let ``My test`` () = Assert.True(true)
