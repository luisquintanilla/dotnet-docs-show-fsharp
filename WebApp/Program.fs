// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open Saturn
open Giraffe
open Giraffe.GiraffeViewEngine
open Plotly.NET

module Types = 

    type Launch = {
        flight_number:int
        launch_year: string
        rocket_id:string
        rocket_name:string
        land_success: bool
    }

module Utils = 

    open Types
    open System.Net.Http
    open System.Text.Json

    let getData (url:string) = 
        async {
            use client = new HttpClient()
            let! data = client.GetStringAsync(url) |> Async.AwaitTask
            let json = JsonSerializer.Deserialize<Launch array>(data)
            return json
        }     

module Services = 

    module LaunchService = 

        open Utils

        let getLaunchCountByYear = 

            let launchEndpoint = "https://api.spacexdata.com/v3/launches"

            let launches = 
                getData launchEndpoint
                |> Async.RunSynchronously

            let years, launchCount = 
                launches
                |> Array.countBy (fun launch -> launch.launch_year)
                |> Array.sortBy (fst)
                |> Array.unzip

            years, launchCount

    module ChartService = 

        open Plotly.NET
        open Plotly.NET.GenericChart

        let createColumnChart (data: string [] * int []) = 
            let k,v = data
            Chart.Column(k,v)

        let getChartHtml chart = 
            chart |> toChartHTML


module Views = 

    open Services
    
    let indexView = 
        html [ ] [
            head [] [
                title [] [Text "F# Web App"]
            ]
            body [] [
                div [] [
                    h1 [] [Text "The .NET Docs ❤ F#"]
                    p [] [ Text "Welcome to my page"]
                ]
            ]
        ]

    let launchView = 

        let launchData = LaunchService.getLaunchCountByYear 

        let launchChartHtml = 
            ChartService.createColumnChart launchData
            |> Chart.withTitle "Launches per Year"
            |> Chart.withX_AxisStyle "Year"
            |> Chart.withY_AxisStyle "# of Launches" 
            |> ChartService.getChartHtml 

        let view = 
            html [ ] [
                head [] [
                    title [] [Text "F# SpaceX Launch"]
                ]
                body [] [
                    h1 [] [Text "SpaceX Launches"]
                    div [] [
                        rawText launchChartHtml
                    ]
                ]
            ]    

        view

let appRouter = router {
    get "/" (htmlView Views.indexView)
    get "/launches" (htmlView Views.launchView)
}

let webApp = application {
    use_router appRouter
}

run webApp