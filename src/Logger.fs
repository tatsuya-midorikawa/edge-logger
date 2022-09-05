module Logger
open System.IO

let combine = Path.Combine >> FilePath
let now = System.DateTime.Now.ToString("yyyyMMdd_HHmmss")
let root'dir'path dir = Path.Combine(Path.GetFullPath dir, now)
let root'dir dir = FilePath (root'dir'path dir)
let err'filepath dir = [| root'dir'path dir; "err.log" |] |> combine
let winsrv'filepath dir = [| root'dir'path dir; "srv"; "winsrv.log" |] |> combine
let edge'filepath dir = [| root'dir'path dir; "edge"; "edge.log" |] |> combine
let edge'installer'filepath dir = [| root'dir'path dir; "msedge_installer.log" |] |> combine
let edge'update'filepath dir = [| root'dir'path dir; "MicrosoftEdgeUpdate.log" |] |> combine
let ie'filepath dir = [| root'dir'path dir; "ie"; "ie.log" |] |> combine
let dsregcmd'filepath dir = [| root'dir'path dir; "os"; "dsregcmd.log" |] |> combine
let whoami'filepath dir = [| root'dir'path dir; "os"; "whoami.log" |] |> combine
let cmdkey'filepath dir = [| root'dir'path dir; "os"; "cmdkey.log" |] |> combine
let hotfix'filepath dir = [| root'dir'path dir; "os"; "hotfix.log" |] |> combine
let schtasks'filepath dir = [| root'dir'path dir; "srv"; "schtasks.log" |] |> combine

let inline exists (FilePath path) = path |> (Path.GetFullPath >> File.Exists)
let inline exists' (path: string) = path |> (Path.GetFullPath >> File.Exists)

let inline output (FilePath path) (msg: string) =
  let path = Path.GetFullPath path
  let dir = Path.GetDirectoryName path
  if not (Directory.Exists dir) then
    Directory.CreateDirectory(dir) |> ignore
    
  if not (exists' path) then 
    File.Create(path).Dispose()

  task {
    do! File.AppendAllLinesAsync(path, seq { msg })
  }

let inline copy (FilePath dst) (FilePath src) =
  let dst = dst |> (Path.GetFullPath >> Path.GetDirectoryName)
  let src = src |> (Path.GetFullPath >> Path.GetDirectoryName)
  let dir = Path.GetDirectoryName dst
  if not (Directory.Exists dir) then
    Directory.CreateDirectory(dir) |> ignore

  if File.Exists src
  then File.Copy(src, dst)
  else File.Create(dst).Dispose()

let inline log dir msg = 
  let path = err'filepath dir
  output path msg