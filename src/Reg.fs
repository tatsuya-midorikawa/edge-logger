module Reg

open Microsoft.Win32

type Reg = { Path: string; Name: string; Type: string; Value: obj; Values: Reg[]}

[<Literal>]
let edge'key = @"SOFTWARE\Policies\Microsoft\Edge"

[<Literal>]
let ie'key = @"SOFTWARE\Microsoft\Internet Explorer"
[<Literal>]
let ie'policy'key = @"SOFTWARE\Policies\Microsoft\Internet Explorer"
[<Literal>]
let ie'iesettings'key = @"SOFTWARE\Policies\Microsoft\Windows\CurrentVersion\Internet Settings"
//[<Literal>]
//let ie'currentver'key = @"Software\Microsoft\Windows\CurrentVersion"
[<Literal>]
let ie'bho'key = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Browser Helper Objects"
[<Literal>]
let ie'setup'key = @"Software\Microsoft\Active Setup\Installed Components"
[<Literal>]
let ie'wow64'key = @"SOFTWARE\Wow6432Node\Microsoft\Internet Explorer"
[<Literal>]
let ie'wow64'currentver'key = @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion"

let rec dig(key: RegistryKey) =
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
        let xs = dig (key)
        { Path = null; Name = name; Type = null; Value = null; Values = xs} )
    Array.append values children

let inline getEdgeRegistries () =
  use HKLM'Edge = Registry.LocalMachine.OpenSubKey(edge'key, false)
  let hklm = dig(HKLM'Edge)

  use HKCU'Edge = Registry.CurrentUser.OpenSubKey(edge'key, false)
  let hkcu = dig(HKCU'Edge)

  Array.append hklm hkcu

let inline getIeRegistries () =
  use HKLM'BHO = Registry.LocalMachine.OpenSubKey(ie'bho'key, false)
  let hklm_bho = dig(HKLM'BHO)

  use HKCU'BHO = Registry.CurrentUser.OpenSubKey(ie'bho'key, false)
  let hkcu_bho = dig(HKCU'BHO)

  Array.append hklm_bho hkcu_bho