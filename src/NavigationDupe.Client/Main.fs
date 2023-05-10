module NavigationDupe.Client.Main

open System
open System.Runtime.InteropServices
open Elmish
open Bolero
open Bolero.Html
open Bolero.Remoting
open Bolero.Remoting.Client
open Bolero.Templating.Client

/// Routing endpoints definition.
type Page =
    | [<EndPoint "/">] Home
    | [<EndPoint "/secondPage">] NewPage    

/// The Elmish application's model.
type Model =
    {
        page: Page
    }

let initModel =
    {
        page = Home
    }

/// The Elmish application's update messages.
type Message =
    | SetPage of Page
    | DoAThing


let update message model =
    Console.WriteLine($"{message}")
    match message with
    | SetPage page ->
        {model with page = page}, Cmd.none
    | DoAThing ->
        model, Cmd.ofMsg (SetPage NewPage)

/// Connects the routing system to the Elmish application.
let router = Router.infer SetPage (fun model -> model.page)

type Main = Template<"wwwroot/main.html">

let homePage model dispatch =
    Main.Home()
        .ClickMe(fun _ -> dispatch DoAThing)
        .Elt()

let newPage model dispatch =
    Main.NewPage().Elt()

let view model dispatch =
    Main()        
        .Body(
            cond model.page <| function
            | Home -> homePage model dispatch
            | NewPage -> newPage model dispatch
        )        
        .Elt()

type MyApp() =
    inherit ProgramComponent<Model, Message>()

    override this.Program =        
        let update = update 
        Program.mkProgram (fun _ -> initModel, Cmd.none) update view
        |> Program.withRouter router
#if DEBUG
        |> Program.withHotReload
#endif
