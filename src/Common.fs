[<AutoOpen>]
module Common

open System.Text.Json

type FilePath = FilePath of string

let defaultof<'T> = Unchecked.defaultof<'T>
type root = HKLM = 0 | HKCU = 1
type dict<'T, 'U> = System.Collections.Generic.Dictionary<'T, 'U>
type pair = System.Collections.Generic.KeyValuePair<string, obj> 
type regs = seq<root * string>
let inline wait<'T> (task: System.Threading.Tasks.Task<'T>) = System.Threading.Tasks.Task.WaitAll (task)
let inline toJson<'T> (object: 'T) = JsonSerializer.Serialize(object, JsonSerializerOptions(WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping, IgnoreNullValues = true))
let inline clear () = System.Console.Clear()
let empty'task = System.Threading.Tasks.Task.Run<unit>(fun () -> ())