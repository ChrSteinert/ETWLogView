open System.Diagnostics.Tracing



type TestSource () =
    inherit EventSource ()

    [<Event(1, Message = "A test event.")>]
    member this.TestEvent () = this.WriteEvent 1


let log = new TestSource () 
Async.Parallel 
    [| 
        async { log.TestEvent () } 
        async { log.TestEvent () } 
        async { log.TestEvent () } 
        async { log.TestEvent () } 
        async { log.TestEvent () } 
        async { log.TestEvent () } 
        async { log.TestEvent () } 
    |]
|> Async.RunSynchronously
