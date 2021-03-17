add :: Int -> Int -> Int
add x y = x + y

zeroto:: Int -> [Int]
zeroto n = [0..n]

isEven:: Int -> Bool
isEven n = n `mod` 2 == 0

isSenior:: Int -> Bool
isSenior y = y >= 5

level:: Int -> String
level y 
    | y >= 10 = "Rockstar"
    | y >= 8 = "Ninja"
    | y >= 5 = "Senior"
    | y >= 3 = "Mid"
    | y >= 0 = "Junior"
    | otherwise = "No Idea"

odds:: Int -> (Int -> Int)-> [Int]
odds n f = map f [0..n-1]

mymap:: (a -> b) -> [a] -> [b]
mymap f xs = [f x | x <- xs]

type Capacity = Int

data Option a = Some a | None deriving Show

instance Functor Option where
    fmap _ None = None
    fmap f (Some a) = Some (f a)

instance Applicative Option where
    pure = Some
    --(<*>) :: Option (a->b) -> Option a -> Option b
    None <*> _ = None
    (Some f) <*> ox = fmap f ox


getYearsOfExperience:: String -> Option Int
getYearsOfExperience "alice@test.com" = Some 10
getYearsOfExperience _ = None
