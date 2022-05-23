module Reg

open Microsoft.Win32

type Reg = { Path: string; Name: string; Type: string; Value: obj; Values: Reg[]}

[<Literal>]
let edge'key = @"SOFTWARE\Policies\Microsoft\Edge"

let inline getEdgeRegistries () =
  let rec loop(key: RegistryKey) =
    if key = null
    then [||]
    else
      let values =
        key.GetValueNames()
        |> Array.map (fun name -> { Path = key.Name; Name = name; Type = key.GetValueKind(name) |> string; Value = Registry.GetValue(key.Name, name, ""); Values = null })
      let children = 
        key.GetSubKeyNames()
        |> Array.map (fun name -> 
          use key = key.OpenSubKey(name)
          let xs = loop (key)
          { Path = null; Name = name; Type = null; Value = null; Values = xs} )
      Array.append values children
    
  use HKLM'Edge = Registry.LocalMachine.OpenSubKey(edge'key, false)
  let hklm = loop(HKLM'Edge)

  use HKCU'Edge = Registry.CurrentUser.OpenSubKey(edge'key, false)
  let hkcu = loop(HKCU'Edge)

  Array.append hklm hkcu