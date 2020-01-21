module todo_bolero.Client.Main

open System
open Elmish
open Bolero
open Bolero.Html
open Bolero.Templating.Client

/// Routing endpoints definition.
type Page =
    | [<EndPoint "/">] Home

/// The Elmish application's model.
type Entry = 
  {
    Id: Guid
    Value: string
  }

type Model =
  {
      Page: Page
      Name: string
      Entries: Entry list
  }

let initModel =
  {
      Page = Home
      Name = "test list"
      Entries = [
        { 
          Id = Guid.NewGuid()
          Value = "first"
        }
        {
          Id = Guid.NewGuid()
          Value = "second"
        }
        {
          Id = Guid.NewGuid()
          Value = "third"
        }
      ]
  }

/// The Elmish application's update messages.
type Message =
  | SetPage of Page
  | SetName of string
  | AddEntry of string
  | RemoveEntry of Guid

let update message model =
  match message with
  | SetPage page ->
      { model with Page = page }
  | SetName str ->
      { model with Name = str }
  | AddEntry str ->
      { model with Entries = 
                   model.Entries @ 
                    [{
                      Id = Guid.NewGuid()
                      Value = str
                    }] }
  | RemoveEntry id -> 
      { model with Entries =
                    model.Entries |> List.filter (fun x -> x.Id <> id) }

/// Connects the routing system to the Elmish application.
let router = Router.infer SetPage (fun model -> model.Page)

type Main = Template<"wwwroot/main.html">

let showEntry dispatch (entry: Entry) =
  Main.Entry()
    .Id(entry.Id)
    .Value(entry.Value)
    .RemoveEntry(fun _ -> dispatch (RemoveEntry entry.Id))
    .Elt()

let view (model:Model) dispatch =
  Main()
    .ListName(model.Name)
    .Entries(forEach model.Entries <| fun e -> showEntry dispatch e)
    .AddEntry("", fun str -> dispatch (AddEntry str))
    .Elt()

type MyApp() =
  inherit ProgramComponent<Model, Message>()

  override this.Program =
    Program.mkSimple (fun _ -> initModel) update view
    |> Program.withRouter router
    |> Program.withHotReload
