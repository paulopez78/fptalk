module Samples

open Xunit
open System.Collections


let add2 x = x + 2
let add5 x = x + 5
let isEven x = x % 2 = 0

let yearsOfExperience = [ 8; 5; 2; 2; 10; 0]

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
    | x        -> x 


let years = function 
    | "Rockstar" -> 12
    | "Ninja"    -> 10
    | "Senior"   -> 8
    | "Mid"      -> 5
    | "Junior"   -> 2
    | _          -> 0 


[<Fact>]
let ``Composition tests`` () =

    let isEvenIfAdd2Then5 = add5 >> add2 >> isEven

    Assert.Equal(
        isEven(add5(add2(2))), 
        isEvenIfAdd2Then5 2
    )

    Assert.Equal(
        isEvenIfAdd2Then5 2,
        2 |> add5 |> add2 |> isEven
    )


[<Fact>]
let ``List map tests`` () =

    let isSenior x = x > 5
    let isSenior' = List.map isSenior

    Assert.Equal<IEnumerable>(
        isSenior' yearsOfExperience, 
        List.map isSenior yearsOfExperience
    )

    Assert.Equal<IEnumerable>(
        List.map isSenior yearsOfExperience,
        yearsOfExperience |> List.map isSenior
    )

    Assert.Equal<IEnumerable>(
        yearsOfExperience |> List.map isSenior,
        yearsOfExperience |> List.map (fun x -> x > 5)
    )

    Assert.Equal<IEnumerable>(
        yearsOfExperience |> List.map level |> List.map promote |> List.map years |> List.map isSenior,
        yearsOfExperience |> List.map (level >> promote >> years >> isSenior)
    )

