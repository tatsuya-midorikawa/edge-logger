﻿module Reg
let a = ()

//open Microsoft.FSharp.Core.CompilerServices
//open Microsoft.Win32
//open System.Text.Json.Serialization
//open System.IO

//type regkey = { 
//  [<JsonPropertyOrder(1)>] name: string; 
//  [<JsonPropertyOrder(2)>] value: obj; }
//  with 
//    static member create (key: RegistryKey) name =
//      let inline get (key: RegistryKey, name) = Registry.GetValue(key.Name, name, "")
//      { name = name; value = get(key, name) } :> obj

//let read (root: root, path: string) =
//  let inline combine (s1, s2) = Path.Combine(s1, s2)
  
//  let rec read (root: root, path: string) =
//    use r = match root with root.HKLM -> Registry.LocalMachine | root.HKCU -> Registry.CurrentUser | _ -> Registry.LocalMachine
//    use key = r.OpenSubKey(path)
//    if key <> defaultof<_> then
//      let mutable c = ArrayCollector<obj>()
//      let regs = key.GetValueNames() |> Array.map (regkey.create key)
//      c.AddMany(regs)

//      for sub in key.GetSubKeyNames() do
//        let v = read (root, combine(path, sub))
//        c.Add(dict<string, obj>([| pair(sub, v) |]))
//      c.Close()
//    else
//      [||]

//  let value = read (root, path)
//  use r = match root with root.HKLM -> Registry.LocalMachine | root.HKCU -> Registry.CurrentUser | _ -> Registry.LocalMachine
//  use key = r.OpenSubKey(path)
//  let acc = dict<string, obj>()
//  if key <> defaultof<_> then
//    acc.TryAdd(key.Name, value) |> ignore
//  acc

//let reads (ls: regs) =
//  let inline combine (s1, s2) = Path.Combine(s1, s2)
  
//  let rec read (root: root, path: string) =
//    use r = match root with root.HKLM -> Registry.LocalMachine | root.HKCU -> Registry.CurrentUser | _ -> Registry.LocalMachine
//    use key = r.OpenSubKey(path)
//    if key <> defaultof<_> then
//      let mutable c = ArrayCollector<obj>()
//      let regs = key.GetValueNames() |> Array.map (regkey.create key)
//      c.AddMany(regs)

//      for sub in key.GetSubKeyNames() do
//        let v = read (root, combine(path, sub))
//        c.Add(dict<string, obj>([| pair(sub, v) |]))
//      c.Close()
//    else
//      [||]

//  let acc = dict<string, obj>()
//  for (root, path) in ls do 
//    let value = read (root, path)
//    use r = match root with root.HKLM -> Registry.LocalMachine | root.HKCU -> Registry.CurrentUser | _ -> Registry.LocalMachine
//    use key = r.OpenSubKey(path)
//    if key <> defaultof<_> then
//      acc.TryAdd(key.Name, value) |> ignore
//  acc