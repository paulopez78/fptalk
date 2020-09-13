module Basics

open Xunit

let add2 x = x + 2
let add5 x = x + 5
let isEven x = x % 2 = 0

let result = add5(add2(1))

let composedFn= add5 >> add2 >> isEven

2 |> add5 |> add2 |> isEven


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

let yearsOfExperience = [ 8; 5; 2; 2; 10; 0]
let elevatedIsSenior = List.map isSenior

elevatedIsSenior [ 8; 5; 2; 2; 10; 0]

List.map isSenior [ 8; 5; 2; 2; 10; 0]

[ 8; 5; 2; 2; 10; 0] |> List.map isSenior

[ 8; 5; 2; 2; 10; 0] |> List.map (fun x -> x > 5)

let a = 0 |> level |> promote |> years |> isSenior

let b = List.map (level >> promote >> years >> isSenior) [ 8; 5; 2; 2; 10; 0]

let c = [ 8; 5; 2; 2; 10; 0] |> List.map (level >> promote >> years >> isSenior)




