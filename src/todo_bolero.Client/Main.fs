module todo_bolero.Client.Main

open System
open Elmish
open Bolero
open Bolero.Html
open Bolero.Json
open Bolero.Remoting
open Bolero.Remoting.Client
open Bolero.Templating.Client

/// Routing endpoints definition.
type Page =
    | [<EndPoint "/">] Home

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

let update message model =
   match message with
    | SetPage page ->
        { model with page = page }

/// Connects the routing system to the Elmish application.
let router = Router.infer SetPage (fun model -> model.page)

type Main = Template<"wwwroot/main.html">

let view model dispatch =
    Main()
        .Name("testing!")
        .Elt()

type MyApp() =
    inherit ProgramComponent<Model, Message>()

    override this.Program =
        Program.mkSimple (fun _ -> initModel) update view
        |> Program.withRouter router
#if DEBUG
        |> Program.withHotReload
#endif