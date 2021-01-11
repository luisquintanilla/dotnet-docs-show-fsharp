// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open Saturn
open Giraffe

// Handlers
let getValues () = 
    [|1;2;3|]

let getValueByIdHandler (id:int) : HttpHandler = 
    let value = 
        getValues()
        |> Array.tryFind(fun el -> el = id)
        |> (fun v -> 
            match v with
            | Some x -> x
            | None -> -1 )

    text (value.ToString())

// Define router
let apiRouter = router {
    get "/api/values" (getValues() |> json) // Handle in router
    getf "/api/values/%i" getValueByIdHandler // Pass in handler
}

// Application definition
let webApi = application {
    use_router apiRouter
}

run webApi