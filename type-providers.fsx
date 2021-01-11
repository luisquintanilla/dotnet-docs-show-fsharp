// Install NuGet package
#I @"C:\Users\luquinta.REDMOND\.nuget\packages"
#r "fsharp.data/3.3.3/lib/netstandard2.0/FSharp.Data.dll"

// No Type Providers

open System.Net.Http
open System.Text.Json

[<Literal>]
let capsulesEndpoint = "https://api.spacexdata.com/v4/capsules"

let getData (url:string) = 
    async {
        use client = new HttpClient()
        let! data = client.GetStringAsync url |> Async.AwaitTask
        return data
    }

type Capsule = {
    ``type``: string
    status: string
}

getData capsulesEndpoint
|> Async.RunSynchronously
|> JsonSerializer.Deserialize<Capsule seq>
|> Seq.filter(fun capsule -> capsule.status = "active")
|> Seq.distinctBy(fun capsule -> capsule.``type``)

// With TypeProviders

open FSharp.Data

type Capsules = JsonProvider<capsulesEndpoint>

let capsules = Capsules.Load(capsulesEndpoint)

capsules
|> Seq.filter(fun capsule -> capsule.Status = "active")
|> Seq.distinctBy(fun capsule -> capsule.Type)
|> Seq.map(fun capsule -> {|Type=capsule.Type; Status= capsule.Status|})