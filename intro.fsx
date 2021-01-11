open System
open System.IO

// Functions

// Simple function
let double n = n * 2

double 3

// Partial application
let makeSound animal = 
    let call output =
        printfn "The %s says %s" animal output
    call

let makeDogSound = makeSound "Dog"
let makeCatSound = makeSound "Cat"

makeDogSound "Woof!"
makeCatSound "Meow!"

// Collections
[1..10]
seq {1..10}

// Naive
seq {1..10}
|> Seq.map(fun x -> x * 2)
|> Seq.map(fun x -> (float x) ** 2.)

// With Functions
let triple n = n * 3
let square n = (float n) ** 2.

seq {1..10}
|> Seq.map(triple)
|> Seq.map(square)

// Composition
seq {1..10}
|> Seq.map(triple >> square)

let tripleAndSquare = triple >> square

seq {1..10}
|> Seq.map(tripleAndSquare)

//Other operations
seq {1..10}
|> Seq.sumBy(tripleAndSquare)