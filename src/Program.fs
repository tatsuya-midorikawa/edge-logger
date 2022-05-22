let wait<'T> (task: System.Threading.Tasks.Task<'T>) = System.Threading.Tasks.Task.WaitAll (task)

//task {
//  do! Logger.output Logger.winsrv'filepath "test"
//}
//|> wait

Winsrv.getServices()
|> Array.map (fun s -> printfn $"    {s.ServiceName} : {s.Status}")

