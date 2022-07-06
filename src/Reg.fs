module Reg

open Microsoft.Win32
open System.Text.Json.Serialization
open System.IO

type regkey = { 
  [<JsonPropertyOrder(1)>] name: string; 
  [<JsonPropertyOrder(2)>] value: obj; }
  with 
    static member create (key: RegistryKey) name =
      let inline get (key: RegistryKey, name) = Registry.GetValue(key.Name, name, "")
      { name = name; value = get(key, name) }

type root = HKLM = 0 | HKCU = 1
type dict<'T, 'U> = System.Collections.Generic.Dictionary<'T, 'U>
type pair = System.Collections.Generic.KeyValuePair<string, obj> 

let read (root: root, path: string) =
  let inline combine (s1, s2) = Path.Combine(s1, s2)
  let inline replace (s: string) = s.Replace(@"\", @"/")
  let gen = combine >> replace
  use root = match root with root.HKLM -> Registry.LocalMachine | root.HKCU -> Registry.CurrentUser | _ -> Registry.LocalMachine
  use key = root.OpenSubKey(path)
  let regs = key.GetValueNames() |> Array.map (regkey.create key)
  dict<string, obj>([| pair(gen(root.Name, path), regs) |])