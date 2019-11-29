open System
open System.Diagnostics.Tracing

open Microsoft.Diagnostics.Tracing
open Microsoft.Diagnostics.Tracing.Session

open Argu

let private printEvent = 
    fun (event : TraceEvent) ->
        printfn "[%s][%8i:%8i][%s:%s] %s"
            (event.TimeStamp.ToString "o")
            event.ProcessID
            event.ThreadID
            event.ProviderName
            event.EventName
            event.FormattedMessage

type Arguments =
    | Providers of names : string list
    | Assembly of path : string

    with 
        interface IArgParserTemplate with
            member this.Usage = 
                match this with
                | Providers _ -> "providers"
                | Assembly _ -> "assembly"

[<EntryPoint>]
let main argv =
    let parser = ArgumentParser.Create<Arguments> ()
    let arguments = parser.Parse argv
    match arguments.GetAllResults () with
    | [ Providers c ] -> 

        use session = new TraceEventSession("45bad58a-cbb6-425d-a9cf-0ae3965102eb")
        Console.CancelKeyPress |> Event.add (fun _ -> session.Dispose ())

        session.Source.Dynamic.add_All (Action<_>(printEvent))

        c
        |> List.iter (fun c -> 
            printfn "Enabling provider '%s'" c
            session.EnableProvider c |> ignore
        )

        session.Source.Process () |> ignore

    | [ Assembly c ] ->
        use session = new TraceEventSession("45bad58a-cbb6-425d-a9cf-0ae3965102eb")
        Console.CancelKeyPress |> Event.add (fun _ -> session.Dispose ())

        session.Source.Dynamic.add_All (Action<_>(printEvent))

        let asm = Reflection.Assembly.LoadFrom c
        asm.GetTypes () 
        |> Seq.filter (fun c -> 
            printfn "Looking at '%s'" c.Name
            c.IsSubclassOf typeof<EventSource>) 
        |> Seq.map (fun c -> 
            let att = c.GetCustomAttributes(typeof<EventSourceAttribute>, false).[0] :?> EventSourceAttribute
            att.Name
        )
        |> Seq.iter (fun c -> 
            printfn "Enabling provider '%s'" c
            session.EnableProvider c |> ignore
        )
        session.Source.Process () |> ignore
    | _ -> parser.PrintUsage () |> printfn "%s"


    |> ignore
    0 
