module Samples

open Xunit
open System.Collections
open System.Net


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


let isSenior x = x > 5
let isSenior' = List.map isSenior

[<Fact>]
let ``List map tests`` () =


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

type Maybe<'a> = 
    | Nothing
    | Just of 'a

let map f opt =
    match opt with
    | Nothing -> Nothing
    | Just x -> Just(f x)

let map' f =
    function
    | Nothing -> Nothing
    | Just x -> f x

let bind f =
    function
    | Just x -> f x
    | Nothing -> Nothing

let getYearsOfExperience email= 
    match email with
    | "bob@abax.no"   ->  Just 3
    | "alice@abax.no" ->  Just 4
    |  _               -> Nothing

let getYearsOfExperience' = 
    function
    |  "bob@abax.no"   ->  Just 3
    |  "alice@abax.no" ->  Just 4
    |  _               -> Nothing

let getYearsOfExperience'' = 
    function
    |  "bob@abax.no"   ->  Some 3
    |  "alice@abax.no" ->  Some 4
    |  _               ->  None
        
let isAuthorized email =
    email 
    |> getYearsOfExperience 
    |> map level |> map promote |> map years |> map isSenior
    |> function
    | Nothing    -> HttpStatusCode.NotFound
    | Just false -> HttpStatusCode.Forbidden
    | Just true  -> HttpStatusCode.OK

[<Fact>]
let ``Maybe functor tests`` () =
    Assert.Equal(
        Just false,
        "bob@abax.no" |> getYearsOfExperience |> map level |> map promote |> map years |> map isSenior
    )

    Assert.Equal(
        Just true,
        "alice@abax.no" |> getYearsOfExperience |> map level |> map promote |> map years |> map isSenior
    )

    Assert.Equal(
        Nothing,
        "gary@abax.no" |> getYearsOfExperience |> map level |> map promote |> map years |> map isSenior
    )

    Assert.Equal(
        HttpStatusCode.Forbidden,
        "bob@abax.no" |> isAuthorized
    )

    Assert.Equal(
        HttpStatusCode.OK,
        "alice@abax.no" |> isAuthorized
    )

    Assert.Equal(
        HttpStatusCode.NotFound,
        "gary@abax.no" |> isAuthorized
    )

let skills = function 
       | "devops"    -> [ "docker"; "azure" ]
       | "backend"   -> [ "csharp"; "fsharp" ]
       | "fullstack" -> [ "js" ]
       | "frontend"  -> [ "js"; "css" ]
       |  _          -> []

let upgrade = function 
       | "docker" -> "k8s"
       | "azure"  -> "gcp"
       | "csharp" -> "fsharp" 
       | "fsharp" -> "haskell"
       |  x       -> x

[<Fact>]
let ``List collect test`` () =

    let bind = List.collect
    let bindedSkills = List.collect skills

    let groups = [ "devops"; "backend" ]

    Assert.Equal<IEnumerable>(
        [ "k8s"; "gcp"; "fsharp"; "haskell"],
        groups |> List.collect skills |> List.map upgrade
    )

    Assert.Equal<IEnumerable>(
        groups |> List.collect skills |> List.map upgrade,
        groups |> bindedSkills |> List.map upgrade
    )

let level' = function
    | x when x > 80 -> Nothing
    | x when x > 10 -> Just "Rockstar"
    | x when x > 8  -> Just "Ninja"
    | x when x > 5  -> Just "Senior"
    | x when x > 3  -> Just "Mid"
    | x when x > 0  -> Just "Junior"
    | _             -> Nothing

let promote' = function 
    | "Rockstar"-> Nothing
    | "Ninja"   -> Just "Rockstar"
    | "Senior"  -> Just "Ninja"
    | "Mid"     -> Just "Senior"
    | "Junior"  -> Just "Mid"
    | _         -> Nothing

let years' = function 
    | "Rockstar" -> Some 12
    | "Ninja"    -> Some 10
    | "Senior"   -> Some 8
    | "Mid"      -> Some 5
    | "Junior"   -> Some 2
    | _          -> None

let level'' = function
    | x when x > 80 -> None
    | x when x > 10 -> Some "Rockstar"
    | x when x > 8  -> Some "Ninja"
    | x when x > 5  -> Some "Senior"
    | x when x > 3  -> Some "Mid"
    | x when x > 0  -> Some "Junior"
    | _             -> None

let promote'' = function 
    | "Rockstar"-> None
    | "Ninja"   -> Some "Rockstar"
    | "Senior"  -> Some "Ninja"
    | "Mid"     -> Some "Senior"
    | "Junior"  -> Some "Mid"
    | _         -> None

let years'' = function 
    | "Rockstar" -> Some 12
    | "Ninja"    -> Some 10
    | "Senior"   -> Some 8
    | "Mid"      -> Some 5
    | "Junior"   -> Some 2
    | _          -> None

[<Fact>]
let ``maybe monad test`` () =
     Assert.Equal( level' -5 |> bind promote', Nothing)
     Assert.Equal( level' 10 |> bind promote', Just "Rockstar")
     Assert.Equal( level' 90 |> bind promote', Nothing)