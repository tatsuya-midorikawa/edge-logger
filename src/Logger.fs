module Logger
open System.IO

type FilePath = FilePath of string

let winsrv'filepath = FilePath @"C:\logs\winsrv.log"
let edge'filepath = FilePath @"C:\logs\edge.log"
let ie'filepath = FilePath @"C:\logs\ie.log"

let inline exists (FilePath path) =
  File.Exists path

let inline exists' (path: string) =
  File.Exists path

let inline output (FilePath path) (msg: string) =
  let dir = Path.GetDirectoryName path
  if not (Directory.Exists dir) then
    Directory.CreateDirectory(dir) |> ignore

  if not (exists' path) then 
    use _ = System.IO.File.Create path
    ()

  task {
    //let now = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
    //do! System.IO.File.AppendAllLinesAsync(path, seq { $"[{now}]" })
    do! System.IO.File.AppendAllLinesAsync(path, seq { msg })
  }
