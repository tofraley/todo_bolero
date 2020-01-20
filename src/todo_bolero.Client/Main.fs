module todo_bolero.Client.Main

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
    Id: int
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
            Id = 0
            Value = "first"
          }
          {
            Id = 1
            Value = "second"
          }
          {
            Id = 2
            Value = "third"
          }
        ]
    }

/// The Elmish application's update messages.
type Message =
    | SetPage of Page
    | SetName of string
    | AddEntry
    | RemoveEntry of int

let update message model =
   match message with
    | SetPage page ->
        { model with Page = page }
    | SetName str ->
        { model with Name = str }
    | AddEntry ->
        { model with Entries = 
                     model.Entries @ 
                      [{
                        Id = model.Entries.Length
                        Value = "another item!" 
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
      .AddEntry(fun _ -> dispatch AddEntry)
      .Elt()

type MyApp() =
    inherit ProgramComponent<Model, Message>()

    override this.Program =
        Program.mkSimple (fun _ -> initModel) update view
        |> Program.withRouter router
#if DEBUG
        |> Program.withHotReload
#endif