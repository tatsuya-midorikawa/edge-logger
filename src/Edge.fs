//module Edge

//open System.IO

//let private combine = System.IO.Path.Combine
//let private sysroot'dir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Windows)
//let private allusrprofile'dir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData)

//// Cmd command: copy /y %SystemRoot%\TEMP\msedge_installer.log dist_dir
//let private installer'log = [| sysroot'dir; "TEMP"; "msedge_installer.log" |] |> combine
//let private installer'cmd dst = [| $"copy /y %s{installer'log} %s{dst}" |]

//let output'installer dir =
//  let dir = [| Logger.root'dir'path dir; "edge" |] |> combine
//  dir |> create'dir
//  let cmd = installer'cmd dir
//  dir |> (FilePath >> get'dir >> create'dir)
//  cmd |> Cmd.exec

//// Cmd command: copy /y %ALLUSERSPROFILE%\Microsoft\EdgeUpdate\Log dist_dir
//let private update'log = [| allusrprofile'dir; "Microsoft"; "EdgeUpdate"; "Log"; |] |> combine
//let private update'cmd dst = [| $"copy /y %s{update'log} %s{dst}";  |]

//let output'updatelog dir =
//  let dir = [| Logger.root'dir'path dir; "edge" |] |> combine
//  dir |> create'dir
//  let cmd = update'cmd dir
//  dir |> (FilePath >> get'dir >> create'dir)
//  cmd |> Cmd.exec