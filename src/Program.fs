open System.Text.Json

let inline wait<'T> (task: System.Threading.Tasks.Task<'T>) = System.Threading.Tasks.Task.WaitAll (task)
let inline toJson<'T> (object: 'T) = JsonSerializer.Serialize(object, JsonSerializerOptions(WriteIndented = true))
let output_to_winsrv = Logger.output Logger.winsrv'filepath

let winsrv_json = 
  Winsrv.getServices()
  |> Winsrv.collect
  |> toJson

task {
  do! output_to_winsrv winsrv_json
}
|> wait
  
