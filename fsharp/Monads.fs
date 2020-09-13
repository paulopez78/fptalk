module Functors

open Xunit

type Maybe<'a> = 
    | Nothing 
    | Just of 'a

let lift f =
    function
    | Just x -> Just(f x)
    | Nothing -> Nothing


let map f opt =
    match opt with 
    | Just x -> Just(f x)
    | Nothing -> Nothing




let level = function
    | x when x > 10 -> "Rockstar"
    | x when x > 8  -> "Ninja"
    | x when x > 5  -> "Senior"
    | x when x > 3  -> "Mid"
    | x when x > 0  -> "Junior"
    | _             -> "No idea what I'm doing"

let level' = function
    | x when x > 80 -> None
    | x when x > 10 -> Some "Rockstar"
    | x when x > 8  -> Some "Ninja"
    | x when x > 5  -> Some "Senior"
    | x when x > 3  -> Some "Mid"
    | x when x > 0  -> Some "Junior"
    | _             -> None

let promote' = function 
    | "Rockstar"-> None
    | "Ninja"   -> Some "Rockstar"
    | "Senior"  -> Some "Ninja"
    | "Mid"     -> Some "Senior"
    | "Junior"  -> Some "Mid"
    | _         -> None

let years' = function 
    | "Rockstar" -> Some 12
    | "Ninja"    -> Some 10
    | "Senior"   -> Some 8
    | "Mid"      -> Some 5
    | "Junior"   -> Some 2
    | _          -> None

let skills = function 
    | "devops"  -> [ "docker"; "azure" ]
    | "backend" -> [ "csharp"; "visual studio"]
    | _         -> []

let upgrade = function 
    | "docker"         -> "k8s"
    | "azure"          ->  "gcp"
    | "visualstudio"   ->  "rider"
    | x                -> x

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

let getYearsOfExperience email = 
    if email = "box.abax.no" then
        Just 4
    else    
        Nothing

let getYearsOfExperience email= 
    match email with
    | "bob@abax.no"   ->  Just 4
    | "alice@abax.no" ->  Just 8
    |  _               -> Nothing

let getYearsOfExperience = 
    function
    |  "bob@abax.no"   ->  Just 4
    |  "alice@abax.no" ->  Just 8
    |  _               -> Nothing
        

let isSeniorDeveloper email = 
    email 
    |> getYearsOfExperience 
    |> map level |> map promote |> map years |> map isSenior 
    |> function 
    | Just result -> result 
    | Nothing -> false

let result = isSeniorDeveloper "bob@abax.no"

// Some 3 |> Option.map level |> Option.map promote 


module Async =
    let map f op = async {
        let! x    = op
        return f x
    }

let yearsOfExperience =  async { return 6 }




[<Fact>]
let ``Monads results`` () = printf "%b" (isSeniorDeveloper "bob")
