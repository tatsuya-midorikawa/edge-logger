open System.Text.Json
open Microsoft.Win32
open System.Web

let inline wait<'T> (task: System.Threading.Tasks.Task<'T>) = System.Threading.Tasks.Task.WaitAll (task)
let inline toJson<'T> (object: 'T) = JsonSerializer.Serialize(object, JsonSerializerOptions(WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping, IgnoreNullValues = true))
let output_to_winsrv = Logger.output Logger.winsrv'filepath

//let winsrv_json = 
//  Winsrv.getServices()
//  |> Winsrv.collect
//  |> toJson

//task {
//  do! output_to_winsrv winsrv_json
//}
//|> wait
  

//let HKLM'Edge = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\Microsoft\Edge", false)
//let values = HKLM'Edge.GetValueNames()
//let edge'key = @"SOFTWARE\Policies\Microsoft\Edge"

//HKLM'Edge.GetValueNames()
//|> Array.map (fun name -> { Reg.Name = name; Reg.Value = Registry.GetValue(HKLM'Edge.Name, name, "") })
//|> toJson
//|> printfn "%s"

//HKLM'Edge.GetValueNames()
//|> Array.map (fun name -> { Reg.Name = name; Reg.Type = HKLM'Edge.GetValueKind(name) |> string; Reg.Value = Registry.GetValue(HKLM'Edge.Name, name, "") })

Reg.getEdgeRegistries ()
|> toJson
|> printfn "%s"


//let HKLM'Edge = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\Microsoft\Edge", false)
//HKLM'Edge.GetSubKeyNames() |> Array.iter (printfn "%s")

//HKLM'Edge.Dispose()