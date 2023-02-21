namespace Foo

module Bar =
    type Metrics =
        { Increment : string -> string -> string -> unit }
        interface System.IDisposable with
            member this.Dispose() = ()

    let construct () =
        { Increment = fun a b c -> printfn "%s %s %s" a b c }

