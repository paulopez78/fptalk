module Composition

open Xunit

let add2 x = x + 2
let add5 x = x + 5
let isEven x = x % 2 = 0

let isEvenIfAdd2Then5 = add5 >> add2 >> isEven

[<Fact>]
let ``Composition tests`` () =
    Assert.Equal(
        isEven(add5(add2(2))), 
        isEvenIfAdd2Then5 2
    )

    Assert.Equal(
        isEvenIfAdd2Then5 2,
        2 |> add5 |> add2 |> isEven
    )