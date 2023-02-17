open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open Giraffe
open System.Threading

let foobar : HttpHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task { return Some ctx }

let foobaz: HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        task { // let x = text "foobar" next ctx // Task<HttpContext option>
               let! x = text "foobar" next ctx // HttpContext option
               return x }

let webApp =
    compose (route "/") foobar

let configureApp (app : IApplicationBuilder) =
    app.UseGiraffe webApp

let configureServices (services : IServiceCollection) =
    services.AddGiraffe () |> ignore

[<EntryPoint>]
let main _ =
    Host.CreateDefaultBuilder()
        .ConfigureWebHostDefaults(
            fun webHostBuilder ->
                webHostBuilder
                    .Configure(configureApp)
                    .ConfigureServices(configureServices)
                    |> ignore)
        .Build()
        .Run()
    0
