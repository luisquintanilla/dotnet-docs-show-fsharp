#r "nuget:Plotly.NET, 2.0.0-alpha5"
#r "nuget:Giraffe.ViewEngine"

open Plotly.NET
open Plotly.NET.GenericChart

let sq =
    [ for x in 1.0 .. 100.0 -> (x, x ** 2.0) ]
    |> Chart.Line


sq |> Chart.Show

let chartHtml = toChartHTML sq

open Giraffe
open Giraffe.ViewEngine

let view = 
    html [] [
        head [] [
            title [] [Text "F# Demo"]
        ]
        body [] [
            h1 [] [Text "Launch Site"]
            rawText chartHtml 
        ]
    ]

let htmlOutput = view |> RenderView.AsString.htmlNode

open System.IO

File.WriteAllText("index.html",htmlOutput)


// With data


open System.Net.Http
open System.Text.Json

//https://docs.spacexdata.com/?version=latest#5fc4c846-c373-43df-a10a-e9faf80a8b0a

let launchEndpoint = "https://api.spacexdata.com/v3/launches"

type Launch = {
    flight_number:int
    launch_year: string
    rocket_id:string
    rocket_name:string
    land_success: bool
}

let getData (url:string) = 
    async {
        use client = new HttpClient()
        let! data = client.GetStringAsync(url) |> Async.AwaitTask
        let json = JsonSerializer.Deserialize<Launch array>(data)
        return json
    } 

let launches = 
    getData launchEndpoint
    |> Async.RunSynchronously

let years, launchCount = 
    launches
    |> Array.countBy (fun launch -> launch.launch_year)
    |> Array.sortBy (fst)
    |> Array.unzip

let launchChart = 
    Chart.Column(years, launchCount) 
    |> Chart.withTitle "Launches per Year"
    |> Chart.withX_AxisStyle "Year"
    |> Chart.withY_AxisStyle "# of Launches"
    |> toChartHTML

open Giraffe
open Giraffe.ViewEngine

let launchView = 
    html [] [
        head [] [
            title [] [Text "F# SpaceX Launches"]
        ]
        body [] [
            rawText launchChart 
        ]
    ]

let launchPageHtml = launchView |> RenderView.AsString.htmlNode

open System.IO

File.WriteAllText("index.html", launchPageHtml)