module Env
let a = ()


//open System.IO

//// PowerShell command: get-hotfix
//let private hotfix'log dir = [| Logger.root'dir'path dir; "env"; "hotfix.log" |] |> combine
//let private hotfix'cmd = [| "get-hotfix" |]

//let output'hotfix dir =
//  let dst = hotfix'log dir
//  dst |> (get'dir >> create'dir)
//  hotfix'cmd |> Pwsh.exec |> Logger.output dst

//// Cmd command: systeminfo
//let private systeminfo'log dir = [| Logger.root'dir'path dir; "env"; "systeminfo.log" |] |> combine
//let private systeminfo'cmd = [| "systeminfo" |]

//let output'systeminfo dir =
//  let dst = systeminfo'log dir
//  dst |> (get'dir >> create'dir)
//  systeminfo'cmd |> Cmd.exec |> Logger.output dst

//// Cmd command: wmic qfe list
//let private qfelist'log dir = [| Logger.root'dir'path dir; "env"; "qfelist.log" |] |> combine
//let private qfelist'cmd = [| "wmic qfe list" |]

//let output'qfe dir =
//  let dst = qfelist'log dir
//  dst |> (get'dir >> create'dir)
//  qfelist'cmd |> Cmd.exec |> Logger.output dst

//// Cmd command: dsregcmd /status
//let private dsregcmd'log dir = [| Logger.root'dir'path dir; "env"; "dsregcmd.log" |] |> combine
//let private dsregcmd'cmd = [| "dsregcmd /status" |]

//let output'dsregcmd dir =
//  let dst = dsregcmd'log dir
//  dst |> (get'dir >> create'dir)
//  dsregcmd'cmd |> Cmd.exec |> Logger.output dst

//// Cmd command: whoami
//let private whoami'log dir = [| Logger.root'dir'path dir; "env"; "whoami.log" |] |> combine
//let private whoami'cmd = [| "whoami" |]

//let output'whoami dir =
//  let dst = whoami'log dir
//  dst |> (get'dir >> create'dir)
//  whoami'cmd |> Cmd.exec |> Logger.output dst

//// Cmd command: cmdkey /list
//let private cmdkey'log dir = [| Logger.root'dir'path dir; "env"; "cmdkey.log" |] |> combine
//let private cmdkey'cmd = [| "cmdkey /list" |]

//let output'cmdkey dir =
//  let dst = cmdkey'log dir
//  dst |> (get'dir >> create'dir)
//  cmdkey'cmd |> Cmd.exec |> Logger.output dst