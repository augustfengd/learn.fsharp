module X =
    type IBar =
        abstract x : _ -> unit
        abstract y : _ -> unit

    type ILogger =
        abstract Info : string -> unit
        abstract Debug : string -> unit

module Y =
    type Reader<'env, 'a> = Reader of action:('env -> 'a)

    let run env (Reader action) =
        action env

    let ask = Reader id

    // ('a -> 'b) -> (E<'a>) -> (E<'b>)
    let map f (Reader action) =
        Reader (action >> f)

    // ('a ->E<'b>) -> E<'a> -> E<'b>
    let bind (f : _ -> Reader<_,_>) (Reader action): Reader<_,_> =
        Reader (fun env ->
                    let (Reader a) = f (action env)
                    a env)

    // ('a ->E<'b>) -> E<'a> -> E<'b>
    let bind' (f : 'a -> Reader<'b,'c>) (action : Reader<'b,'a>) : Reader<'b,'c> =
        let a env =
            let r = run env action
            run env (f r)
        Reader a

    type ReaderBuilder() =
        member _.Return(x) = Reader (fun _ -> x)
        member _.Bind(x,f) = bind f x
        member _.Zero() = Reader (fun _ -> ())


module A =
    let foo a b (c : X.IBar) =
        let r = "helloworld"
        c.x r
        r

    let fuu a b =
        let r = "helloworld"
        fun (c : X.IBar) ->
            c.x r
            r

module B =
    let foo a b : Y.Reader<X.IBar,_> =
        let r = "helloworld"
        Y.Reader (fun c -> c.x r)

module C =
    let reader = Y.ReaderBuilder()

    let compareTwoStrings (a : string) (b : string) =
        reader {
            let! (logger : X.ILogger) = Y.ask
            logger.Debug "compareTwoStrings: starting"

            let result = compare a b

            logger.Info (sprintf "compareTwoStrings: result=%A" result)
            logger.Debug "compareTwoStrings: Finished"
            return result
        }


[<EntryPoint>]
let main _ =
    let logger =
        { new X.ILogger with
          member _.Info(s) = printfn "Info: %s" s
          member _.Debug(s) = printfn "Debug: %s" s }
    let program = C.compareTwoStrings "foo" "bar"
    Y.run logger program |> ignore
    0
