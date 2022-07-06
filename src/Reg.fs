module Reg

open Microsoft.Win32
open System.Text.Json.Serialization

type key = { 
  [<JsonPropertyOrder(1)>] name: string; 
  [<JsonPropertyOrder(2)>] value: obj; }
type root = HKLM = 0 | HKCU = 1

let read (root: root, path: string) =
  use root = match root with root.HKLM -> Registry.LocalMachine | root.HKCU -> Registry.CurrentUser | _ -> Registry.LocalMachine
  use key = root.OpenSubKey(path)
  key.GetValueNames()
  |> Array.map (fun name -> { name = name; value = Registry.GetValue(key.Name, name, "") })