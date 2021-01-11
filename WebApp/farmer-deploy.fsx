#r "nuget:Farmer"

open Farmer
open Farmer.Builders

let fsApp = webApp {
    name "the-best-fsharp-web-app-2021"
    zip_deploy @"C:\Dev\dotnet-docs-show-fsharp\WebApp\publish"
}

let deployment = arm {
    location Location.EastUS
    add_resource fsApp
}

deployment
|> Deploy.execute "luquinta-farmer-fsharp-rg" Deploy.NoParameters