newtype Parser a = P(String -> [(a, String)])

parse:: Parser a -> String -> [(a, String)]
parse (P p) inp = p inp

item:: Parser Char
item = P (\inp -> case inp of
        []     -> []
        (x:xs) -> [(x,xs)]
    )

instance Functor Parser where
    -- fmap :: (a -> b) -> Parser a -> Parser b
    fmap f px = P ( \inp -> case parse px inp of 
        [] -> []
        [(y,out)] -> [(f y,out)]
        )

instance Applicative Parser where
    -- pure :: a -> Parser a
    pure v = P(\inp -> [(v,inp)])

    -- <*> :: Parser(a -> b) -> Parser a -> Parser b
    pg <*> px = P (\inp -> case parse pg inp of
                    [] -> []
                    [(g,out)] -> parse(fmap g px) out)

-- (['H',"ola"])

three:: Parser(Char,Char)
three = g <$> item <*> item <*> item
    where g x y z = (x,z)

instance Monad Parser where
     -- (>>=) :: Parser a -> (a -> Parser b) -> Parser b
     p >>= f = P(\inp -> case parse p inp of
                    [] -> []
                    [(v,out)] -> parse (f v) out
         )

threeM :: Parser(Char,Char)
threeM = do 
    x <- item
    y <- item
    z <- item
    return (x,y)
